using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common.Views
{
  public class RunGameView : View<RunGameView.Data>
  {
    public RunGameView(Data data) : base(data)
    {
    }

    public record Data(long ChatId, int WordsCount = default,
      CallbackQuery? Query = default);

    public async override Task Render(ITelegramBotClient renderer)
    {
      var (chatId, wordsCount, query) = _data;
      if (query is not null)
      {
        await renderer.AnswerCallbackQueryAsync(query.Id);
      }

      await renderer.SendTextMessageAsync(chatId, wordsCount > 0 ?
        "Введите количество" : "Игра началась!");
    }
  }
}