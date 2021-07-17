using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WordsBot.Common;
using WordsBot.Common.Models;

namespace WordsBot
{
  public class Worker : BackgroundService
  {
    readonly ILogger<Worker> _logger;
    readonly IWordsBot _wordsBot;
    readonly IHostApplicationLifetime _applicationLifetime;
    readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime,
      IServiceProvider serviceProvider, IWordsBot wordsBot) =>
      (_logger, _applicationLifetime, _serviceProvider, _wordsBot) =
        (logger, applicationLifetime, serviceProvider, wordsBot);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      try
      {
        using (var scope = _serviceProvider.CreateScope())
        {
          ((ProgramDbContext)scope
            .ServiceProvider.GetRequiredService<WordsBotDbContext>()).Database.Migrate();
        }
        await _wordsBot.Run(stoppingToken);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "worker exec");
        Environment.ExitCode = 1;
        _applicationLifetime.StopApplication();
      }
    }
  }
}
