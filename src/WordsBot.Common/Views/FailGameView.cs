using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class FailGameView : View<FailGameView.Data>
  {
    public FailGameView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, string GiveUpCommand);

    public override async Task Render(ITelegramBotClient renderer)
    {
      await renderer.SendTextMessageAsync(_data.ChatId,
        "не верно",
        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
        {
          Text = "не знаю",
          CallbackData = _data.GiveUpCommand,
        }));
    }
  }
}