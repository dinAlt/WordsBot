using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WordsBot.Common.Controllers;
using WordsBot.Common.Views;
using Xunit;

namespace WordsBot.Common.Test
{
  public class TranslateControllerTest : ControllerTest
  {
    public record MessageCase(Message Message, object Want);
    public record CallbackCase(CallbackQuery Input, object Want);

    [Theory]
    [MemberData(nameof(GetMessageCase))]
    public async Task HandlesMessage(Message message, object want)
    {
      var viewFactory = new MockViewFactory();
      var controller = new TranslateController(_dbCotextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), viewFactory);

      await controller.HandleMessageAsync(message);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new System.Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      Assert.Equal(want, view.Data);
    }

    public static IEnumerable<object[]> GetMessageCase()
    {
      yield return new object[] {
          new Message {
            MessageId = 1,
            Text = "tincture",
            From = new User{
              Id = 123,
            },
            ReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton{
              Text = "добавить",
              CallbackData = "add|tincture"
            })
          },
          new TranslateWordView.Data(123, "tincture",
            new List<string>(new string[] { "настойка" }), false, "add|tincture")
        };
    }


    [Theory]
    [MemberData(nameof(HandlesCallback))]
    public async Task HandlesCallback(CallbackQuery query, IEnumerable<string> parsedArgs, object want)
    {
      var viewFactory = new MockViewFactory();
      var controller = new TranslateController(_dbCotextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), viewFactory);

      await controller.HandleCallbackAsync(query, parsedArgs);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new System.Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      Assert.Equal(want, view.Data);
    }

    public static IEnumerable<object[]> GetCallbackCase()
    {
      yield return new object[] {
          new CallbackQuery {
          },
          new string[] {"add", "tincture"},
          new AddRemoveWordView.Data(123, 1, "1234", false, "add|tincture")
        };
    }


  }
}