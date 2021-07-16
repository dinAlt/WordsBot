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

    public record Data(long ChatId, int FailsCount, int SuccessCount, int TotalCount, int GiveUpsCount, string RepeatCommand);

    public override async Task Render(ITelegramBotClient renderer)
    {
      var (chatId, failsCount, answeredCount, totalCount, giveUpsCount, repeatCommand) = _data;
      await renderer.SendTextMessageAsync(chatId,
        $"Игра завершена!\nВсего слов: {totalCount}\n" +
        $"Правильных ответов: {answeredCount}\n" +
        $"Без ответа: {giveUpsCount}\n" +
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