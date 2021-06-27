using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordsBot.Common.Test
{
  public record MockTranslation(string Word, string From, string To, List<string> Values);

  public class MockTranslater : ITranslator
  {
    readonly MockTranslation[] stored;
    public int AccessCount { get; private set; }

    public MockTranslater(params MockTranslation[] translations)
    {
      stored = translations;
    }

    public Task<List<string>> TranslateAsync(string word, string from, string to)
    {
      AccessCount++;
      return Task.FromResult(
        stored.Where(
          t => t.Word == word && t.From == from && t.To == to).
          FirstOrDefault()?.Values ?? new List<string>());
    }
  }
}
