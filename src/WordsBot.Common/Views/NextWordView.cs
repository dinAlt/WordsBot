using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class NextWordView : View<NextWordView.Data>
  {
    public NextWordView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, int WordsCount, int CurrentWordNum,
      int FailsCount, string Word, string FailCommand);

    public override async Task Render(ITelegramBotClient renderer)
    {
      await renderer.SendTextMessageAsync(_data.ChatId,
        $"{_data.Word}\n{_data.CurrentWordNum}\\{_data.WordsCount}",
        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
        {
          Text = "не знаю",
          CallbackData = _data.FailCommand,
        })
      );
    }
  }
}