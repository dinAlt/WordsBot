using System;
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

    public async Task Run()
    {
      var me = await _telegramBotClient.GetMeAsync();
      if (me == null)
      {
        throw new Exception("Not me at all");
      }

      updateReceiver = new QueuedUpdateReceiver(_telegramBotClient);
      updateReceiver.StartReceiving();

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
