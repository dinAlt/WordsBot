using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WordsBot.Common;
using WordsBot.Database.Sqlite;
using WordsBot.Translators.YandexTranslate;

namespace WordsBot
{
  class Program
  {
    static void Main()
    {
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

      var bot = new Bot(telegramAccessToken,
        new YandexTranslate(yandexCloudServiceAccountKey, yandexCloudFolderId),
        dbContextFactory);

      Console.CancelKeyPress += async (sender, eventArgs) =>
      {
        Console.WriteLine("Shutting down gracefully");
        eventArgs.Cancel = true;
        await bot.Stop();
      };

      Console.WriteLine("Startup");
      bot.Run().Wait();
    }
  }

  // EF cli needs DbContext descendant, to generate migrations.
  class ProgramDbContext : SqliteDbContext
  {
    public ProgramDbContext(string dbPath) : base(dbPath)
    {
    }
  }
}
