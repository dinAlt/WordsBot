using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace WordsBot.Common.Controllers
{
  public interface IController
  {
    Task HandleMessageAsync(Message message);
    Task HandleCallbackAsync(CallbackQuery query, IEnumerable<string> parsedArgs);
    void SetCommandBuilder(ICommandBuilder builder);
  }
}