using System;
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
  public class GameControllerTest : ControllerTest
  {
    [Theory]
    [MemberData(nameof(GetMessageCase))]
    public async Task HandlesMessage(Message message, MockTranslater translator, TranslateWordView.Data want, bool wantTranslatorCall)
    {
      var viewFactory = new MockViewFactory();
      var controller = new GameController(_dbContextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), 1, viewFactory);

      await controller.HandleMessageAsync(message);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      var viewData = view.Data;
    }

    public static IEnumerable<object[]> GetMessageCase()
    {
      var addCmd = $"{TranslateController.Command.Add}";
      var removeCmd = $"{TranslateController.Command.Remove}";

      yield return new object[] {
          new Message {
            MessageId = 1,
            Text = "tincture",
            From = new User{
              Id = 123,
            },
          },
          new TranslateWordView.Data(123, "tincture",
            new List<string>(new string[] { "настойка" }), false, addCmd+"|tincture"),
          true,
        };

      yield return new object[] {
          new Message {
            MessageId = 2,
            Text = "tincture",
            From = new User{
              Id = 123,
            },
          },
          new TranslateWordView.Data(123, "tincture",
            new List<string>(new string[] { "настойка" }), true, removeCmd+"|tincture"),
          false,
        };
    }


    [Theory]
    [MemberData(nameof(GetCallbackCase))]
    public async Task HandlesCallback(CallbackQuery query, IEnumerable<string> parsedArgs, AddRemoveWordView.Data want)
    {
      var viewFactory = new MockViewFactory();
      var controller = new GameController(_dbContextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), 1, viewFactory);

      await controller.HandleCallbackAsync(query, parsedArgs);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      var viewData = view.Data;
    }

    public static IEnumerable<object[]> GetCallbackCase()
    {
      var addCmd = $"{TranslateController.Command.Add}";
      var removeCmd = $"{TranslateController.Command.Remove}";
      yield return new object[] {
          new CallbackQuery {
            Message = new Message {
              Chat = new Chat{Id = 123},
              MessageId = 1,
            },
            From = new User { Id = 123},
            Id = "1234",
            Data = addCmd+"|plane"
          },
          new string[] {addCmd, "plane"},
          new AddRemoveWordView.Data(123, 1, "1234", true,
            removeCmd + "|plane")
        };
      yield return new object[] {
          new CallbackQuery {
            Message = new Message {
              Chat = new Chat{Id = 123},
              MessageId = 1,
            },
            From = new User { Id = 123},
            Id = "1234",
            Data = removeCmd+"|plane"
          },
          new string[] {removeCmd, "plane"},
          new AddRemoveWordView.Data(123, 1, "1234", false,
            addCmd+"|plane")
        };
    }
  }
}