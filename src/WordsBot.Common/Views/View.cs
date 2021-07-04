using System.Threading.Tasks;
using Telegram.Bot;

namespace WordsBot.Common.Views
{
  public abstract class View<T> : IView
  {
#pragma warning disable IDE0052
    protected readonly T _data;
#pragma warning restore

    public View(T data) => _data = data;

    public abstract Task Render(ITelegramBotClient botClient);
  }
}