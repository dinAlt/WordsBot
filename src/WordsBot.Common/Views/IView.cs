using System.Threading.Tasks;
using Telegram.Bot;

namespace WordsBot.Common.Views
{
  public interface IView
  {
    Task Render(ITelegramBotClient botClient);
  }
}