using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class ErrorView : View<ErrorView.Data>
  {
    public ErrorView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, string Text);

    public override Task Render(ITelegramBotClient renderer)
    {
      return renderer.SendTextMessageAsync(_data.ChatId, _data.Text);
    }
  }
}