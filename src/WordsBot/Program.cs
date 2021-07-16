using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WordsBot.Common;
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
        .ConfigureServices((histContext, services) =>
        {
          services.AddHostedService<Worker>();
        });
  }

  // EF cli needs DbContext descendant, to generate migrations.
  class ProgramDbContext : SqliteDbContext
  {
    public ProgramDbContext() : base()
    {

    }

    public ProgramDbContext(string dbPath) : base(dbPath)
    {
    }
  }
}
