using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class PauseResumeGameView : View<PauseResumeGameView.Data>
  {
    public PauseResumeGameView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, string QueryId, bool GameRunning, bool Resuming);

    public async override Task Render(ITelegramBotClient renderer)
    {
      await renderer.AnswerCallbackQueryAsync(_data.QueryId);
      if (!_data.GameRunning)
      {
        await renderer.SendTextMessageAsync(_data.ChatId, "Игра не запущена");
        return;
      }

      await renderer.SendTextMessageAsync(_data.ChatId,
        _data.Resuming ? "Игра возобновлена" : "Игра приостановлена");
    }
  }
}