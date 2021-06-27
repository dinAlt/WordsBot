using System;
using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace WordsBot.Translators.YandexTranslateTest
{
  public class YandexTranslateTest
  {
    static YandexTranslateTest()
    {
      DotNetEnv.Env.Load(Path.Join(Environment.CurrentDirectory, ".env"));
    }

    [Fact]
    public async Task ShouldTranslateTheWord()
    {
      Console.WriteLine(Environment.CurrentDirectory);
      var translator = GetTranslator();
      var translations = await translator.TranslateAsync("can", "en", "ru");
      Assert.NotEmpty(translations);
    }

    private static YandexTranslate.YandexTranslate GetTranslator()
    {
      var serviceAccountKey = Environment.GetEnvironmentVariable("WORDSBOT_YANDEX_TRANSLATE_TEST_KEY");
      var foulder = Environment.GetEnvironmentVariable("WORDSBOT_YANDEX_TRANSLATE_TEST_FOULDER");
      if (string.IsNullOrEmpty(serviceAccountKey) || string.IsNullOrEmpty(foulder))
      {
        throw new Exception("Not all env variables are set");
      }
      return new YandexTranslate.YandexTranslate(serviceAccountKey, foulder);
    }
  }
}
