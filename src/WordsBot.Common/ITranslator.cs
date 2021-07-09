using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordsBot.Common
{
  public interface ITranslator
  {
    Task<IEnumerable<string>> TranslateAsync(string word, string from, string to);
  }
}