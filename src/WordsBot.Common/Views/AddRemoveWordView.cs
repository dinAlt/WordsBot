using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class AddRemoveWordView : View<AddRemoveWordView.Data>
  {
    public AddRemoveWordView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, int MessageId, string CallbackQueryId,
      bool IsWordTraining, string CallbackData);

    public override async Task Render(ITelegramBotClient renderer)
    {
      await renderer.AnswerCallbackQueryAsync(_data.CallbackQueryId);
      await renderer.EditMessageReplyMarkupAsync(_data.ChatId, _data.MessageId, new InlineKeyboardMarkup(new InlineKeyboardButton
      {
        Text = _data.IsWordTraining ? "Удалить" : "Добавить",
        CallbackData = _data.CallbackData
      }));
    }
  }
}