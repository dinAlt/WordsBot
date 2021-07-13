using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class FinishGameView : View<FinishGameView.Data>
  {
    public FinishGameView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, int FailsCount, int AnsweredCount, int TotalCount,
      string RepeatCommand);

    public override Task Render(ITelegramBotClient renderer)
    {
      var (chatId, failsCount, answeredCount, totalCount, repeatCommand) = _data;
      return renderer.SendTextMessageAsync(chatId,
        $"Игра завершена!\nВсего слов: {totalCount}\n" +
        $"Правильных ответов: {answeredCount}\n" +
        $"Ошибок: {failsCount}",
        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
        {
          Text = "повторить",
          CallbackData = repeatCommand,
        })
      );
    }
  }
}