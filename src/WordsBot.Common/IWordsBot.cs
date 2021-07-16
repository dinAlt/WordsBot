using System.Threading;
using System.Threading.Tasks;

namespace WordsBot.Common
{
  public interface IWordsBot
  {
    Task Run(CancellationToken cancellationToken = default);
  }
}