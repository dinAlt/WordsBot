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
  public class TranslateControllerTest : ControllerTest
  {
    [Theory]
    [MemberData(nameof(GetMessageCase))]
    public async Task HandlesMessage(Message message, MockTranslater translator, TranslateWordView.Data want, bool wantTranslatorCall)
    {
      var viewFactory = new MockViewFactory();
      var controller = new TranslateController(_dbContextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), viewFactory, translator);

      await controller.HandleMessageAsync(message);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      Assert.Equal(wantTranslatorCall, translator.AccessCount > 0);
      var viewData = (TranslateWordView.Data)view.Data;
      Assert.Equal(want.Word, viewData.Word);
      Assert.Equal(want.IsWordTraining, viewData.IsWordTraining);
      Assert.Equal(want.Translations, viewData.Translations);
      Assert.Equal(want.CallbackData, viewData.CallbackData);
    }

    public static IEnumerable<object[]> GetMessageCase()
    {
      static ITranslator getTranslator() => new MockTranslater(
       new MockTranslation("tincture", "en", "ru", new string[] { "настойка" }));
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
          getTranslator(),
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
          getTranslator(),
          new TranslateWordView.Data(123, "tincture",
            new List<string>(new string[] { "настойка" }), true, removeCmd+"|tincture"),
          false,
        };

      using var dbContext = _dbContextFactory.CreateDbContext();
      dbContext.TrainingTranslations.Add(new Models.TrainingTranslation(123, "tincture"));
      dbContext.SaveChanges();
    }


    [Theory]
    [MemberData(nameof(GetCallbackCase))]
    public async Task HandlesCallback(CallbackQuery query, IEnumerable<string> parsedArgs, AddRemoveWordView.Data want)
    {
      var viewFactory = new MockViewFactory();
      var controller = new TranslateController(_dbContextFactory.CreateDbContext(),
        _botClient, new CommandBuilder('|'), viewFactory, new MockTranslater());

      await controller.HandleCallbackAsync(query, parsedArgs);

      Assert.Single(viewFactory.Views);

      var view = viewFactory.Views.First() as MockView ??
        throw new Exception($"view is not {nameof(MockView)}");

      Assert.True(view.Rendered);
      var viewData = (AddRemoveWordView.Data)view.Data;
      Assert.Equal(want.IsWordTraining, viewData.IsWordTraining);
      Assert.Equal(want.CallbackData, viewData.CallbackData);
      using var dbContext = _dbContextFactory.CreateDbContext();
      Assert.Equal(want.IsWordTraining, dbContext.TrainingTranslations.Any(
        t => t.UserId == want.ChatId && t.Word == parsedArgs.ElementAt(1)));
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