using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class GiveUpGameView : View<GiveUpGameView.Data>
  {
    public GiveUpGameView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, string QueryId,
      IEnumerable<string> Translations);

    public async override Task Render(ITelegramBotClient renderer)
    {
      await renderer.AnswerCallbackQueryAsync(_data.QueryId);
      await renderer.SendTextMessageAsync(_data.ChatId, string.Join("\n", _data.Translations));
    }
  }
}