using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace WordsBot.Common
{
  public class Bot : IWordsBot
  {
    public Bot(ILogger<Bot> logger, ITelegramBotClient botClient, IRouter router) =>
     (_logger, _telegramBotClient, _router) = (logger, botClient, router);

    public async Task Run(CancellationToken cancellationToken = default)
    {
      var me = await _telegramBotClient.GetMeAsync(cancellationToken: cancellationToken);
      if (me == null)
      {
        throw new Exception("Not me at all");
      }

      updateReceiver = new QueuedUpdateReceiver(_telegramBotClient);
      updateReceiver.StartReceiving(cancellationToken: cancellationToken);

      _logger.LogInformation("Bot listen for updates");
      await foreach (var update in updateReceiver.YieldUpdatesAsync())
      {
        _logger.LogInformation($"Got update with Type={update.Type} and Id={update.Id}");
        try
        {
          await _router.RouteUpdateAsync(update);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "RouteUpdate failed");
          throw;
        }
      }
    }

    public async Task Stop()
    {
      updateReceiver?.StopReceiving();
      await _telegramBotClient.CloseAsync();
    }

    readonly ITelegramBotClient _telegramBotClient;
    readonly IRouter _router;
    readonly ILogger<Bot> _logger;

    QueuedUpdateReceiver? updateReceiver;
  }
}
