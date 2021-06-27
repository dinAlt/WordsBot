using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordsBot.Common
{
  public interface ITranslator
  {
    Task<List<string>> TranslateAsync(string word, string from, string to);
  }
}