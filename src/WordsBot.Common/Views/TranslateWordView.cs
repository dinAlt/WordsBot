using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class TranslateWordView : View<TranslateWordView.Data>
  {
    public TranslateWordView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, string Word,
      IEnumerable<string> Translations, bool IsWordTraining, string CallbackData);

    public override Task Render(ITelegramBotClient renderer)
    {
      return renderer.SendTextMessageAsync(
        _data.ChatId,
        string.Join("\n", _data.Translations),
        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
        {
          Text = _data.IsWordTraining ? "Удалить" : "Добавить",
          CallbackData = _data.CallbackData
        })
      );
    }
  }
}