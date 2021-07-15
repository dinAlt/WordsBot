using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class MainMenuView : View<MainMenuView.Data>
  {
    public struct MenuItem
    {
      public string Text;
      public string Callback;
    }

    public MainMenuView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, IEnumerable<MenuItem> Menu);

    public override async Task Render(ITelegramBotClient renderer)
    {
      await renderer.SendTextMessageAsync(
        _data.ChatId,
        "Выберите действие:",
        replyMarkup: new InlineKeyboardMarkup(_data.Menu.Select(t =>
          new InlineKeyboardButton[]
            {
              new InlineKeyboardButton
              {
                Text = t.Text,
                CallbackData = t.Callback
              }
            }
          )
        )
      );
    }
  }
}