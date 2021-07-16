using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using WordsBot.Common;
using WordsBot.Common.Views;
using WordsBot.Database.Sqlite;
using WordsBot.Translators.YandexTranslate;

namespace WordsBot
{
  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      Console.OutputEncoding = System.Text.Encoding.UTF8;
      var config = new ConfigurationBuilder()
         .SetBasePath(System.IO.Directory.GetCurrentDirectory())
         .AddJsonFile("config.json", true)
         .AddEnvironmentVariables("WORDSBOT_")
         .Build();

      var telegramAccessToken = config["TelegramAccessToken"];
      var sqliteDbPath = config["SqliteDatabasePath"];
      var yandexCloudServiceAccountKey = config["YandexCloudServiceAccountKey"];
      var yandexCloudFolderId = config["YandexCloudFolderId"];
      var dbContextFactory = new SqliteDbContextFactory(sqliteDbPath);

      using (var dbContext = new ProgramDbContext(sqliteDbPath))
      {
        dbContext!.Database.Migrate();
      }
      var telegramBotClient = new TelegramBotClient(telegramAccessToken);
      var router = new Router(
        dbContextFactory,
        telegramBotClient,
        new ViewFactory(),
        new YandexTranslate(yandexCloudServiceAccountKey, yandexCloudFolderId)
      );

      var bot = new Bot(telegramBotClient, router);
      await bot.Run(stoppingToken);
    }
  }
}
