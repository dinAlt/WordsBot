using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WordsBot.Common;
using WordsBot.Common.Models;
using WordsBot.Common.Views;
using WordsBot.Database.Sqlite;
using WordsBot.Translators.YandexTranslate;

namespace WordsBot
{
  class Program
  {
    static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(configHost =>
        {
          configHost
          // .AddJsonFile("config.json", true)
          .AddEnvironmentVariables("WORDSBOT_")
          .AddCommandLine(args);
        })
        .ConfigureServices((context, services) =>
        {
          services.AddSingleton<ITelegramBotClient, TelegramBotClient>(_ =>
          {
            var telegramAccessToken = context.Configuration["TelegramAccessToken"];
            return new TelegramBotClient(telegramAccessToken);
          });
          services.AddHostedService<Worker>();
          services.AddScoped<WordsBotDbContext>(_ => new ProgramDbContext(context.Configuration));
          services.AddTranslator<Translator>();
          services.AddWordsBot();
        });
  }

  // EF cli needs DbContext descendant, to generate migrations.
  class ProgramDbContext : SqliteDbContext
  {
    public ProgramDbContext(IConfiguration configuration)
      => _configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder builder) =>
      builder.UseSqlite($"Data source={_configuration["SqliteDatabasePath"]}");

    readonly IConfiguration _configuration;
  }

  class Translator : YandexTranslate
  {
    public Translator(IConfiguration configuration) :
      base(configuration["YandexCloudServiceAccountKey"],
        configuration["YandexCloudFolderId"])
    {
    }
  }
}
