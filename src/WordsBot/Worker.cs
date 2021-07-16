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
    private readonly IWordsBot _wordsBot;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime, IWordsBot wordsBot) =>
      (_logger, _applicationLifetime, _wordsBot) = (logger, applicationLifetime, wordsBot);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      try
      {
        await _wordsBot.Run(stoppingToken);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "worker exec");
        _applicationLifetime.StopApplication();
      }
    }
  }
}
