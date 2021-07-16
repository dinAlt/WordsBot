using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace WordsBot.Common
{
  public class Bot
  {
    public Bot(ITelegramBotClient botClient, IRouter router)
    {
      _telegramBotClient = botClient;
      _router = router;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
      var me = await _telegramBotClient.GetMeAsync(cancellationToken: cancellationToken);
      if (me == null)
      {
        throw new Exception("Not me at all");
      }

      updateReceiver = new QueuedUpdateReceiver(_telegramBotClient);
      updateReceiver.StartReceiving(cancellationToken: cancellationToken);

      await foreach (var update in updateReceiver.YieldUpdatesAsync())
      {
        await _router.RouteUpdateAsync(update);
      }
    }

    public async Task Stop()
    {
      updateReceiver?.StopReceiving();
      await _telegramBotClient.CloseAsync();
    }

    readonly ITelegramBotClient _telegramBotClient;
    readonly IRouter _router;

    QueuedUpdateReceiver? updateReceiver;
  }
}
