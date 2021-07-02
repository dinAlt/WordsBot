using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace WordsBot.Common.Test
{
  public class BotTest
  {
    readonly IDbContextFactory factory;
    public BotTest()
    {
      factory = new MockDbContextFactory();
    }

    [Fact]
    public async Task ReplyNotTranslatedWhenNoTranslationFound()
    {
      var tgCli = new MockTelegramBotClient("some", "one", "help");
      var bot = new Bot(tgCli, new MockTranslater(), factory);

      tgCli.OnNoMessages += (sender, args) => _ = bot.Stop();
      await bot.Run();
      Assert.Equal("нет перевода", tgCli.SentMessages[0].Text);
      Assert.Equal(3, tgCli.SentMessages.Count);
    }

    [Fact]
    public async Task ReplyTranslatedTextWhenFound()
    {
      var tgCli = new MockTelegramBotClient("train");
      var translation = new MockTranslation(
        "train", "en", "ru",
         (new string[] { "поезд", "тренироваться" }
        ).ToList());
      var translater = new MockTranslater(translation);
      var bot = new Bot(tgCli, translater, factory);
      tgCli.OnNoMessages += (sender, args) => _ = bot.Stop();
      await bot.Run();
      Assert.Equal("поезд, тренироваться", tgCli.SentMessages[0].Text);
    }

    [Fact]
    public async Task TakesValueFromLocalStorageIfExists()
    {
      var tgCli = new MockTelegramBotClient("chain", "chain");
      var translation = new MockTranslation(
        "chain", "en", "ru", (new string[] { "цепь" }).ToList());
      var translater = new MockTranslater(translation);
      var bot = new Bot(tgCli, translater, factory);

      tgCli.OnNoMessages += (sender, args) => _ = bot.Stop();

      await bot.Run();

      Assert.Equal(1, translater.AccessCount);
      Assert.Equal(2, tgCli.SentMessages.Count);
    }

    [Fact]
    public void MessageRegexWorks()
    {
      var (match, word, num, of, fails) = Bot.ParseWordMessage("some\n(1\\10\\0)");
      Assert.True(match);
      Assert.Equal("some", word);
      Assert.Equal(1, num);
      Assert.Equal(10, of);
      Assert.Equal(0, fails);
    }
  }
}

