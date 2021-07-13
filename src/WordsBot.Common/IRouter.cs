using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace WordsBot.Common
{
  public interface IRouter
  {
    public Task RouteUpdateAsync(Update update);
  }
}