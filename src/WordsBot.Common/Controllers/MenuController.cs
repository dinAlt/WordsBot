using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WordsBot.Common.Models;
using WordsBot.Common.Views;

namespace WordsBot.Common.Controllers
{
  public class MenuController : Controller
  {
    public MenuController(WordsBotDbContext dbContext, ITelegramBotClient telegramBotClient,
      ICommandBuilder commandBuilder, IViewFactory viewFactory) : base(dbContext, telegramBotClient, commandBuilder, viewFactory) { }


    public override Task HandleMessageAsync(Message message)
    {
      var command = message.Text.Trim().ToLower();

      if (command != "/start" && command != "/menu")
      {
        throw new Exception($"Commnad not supported: {command}");
      }

      var gameCommandBuilder = new CommandBuilder('|', "Game");
      var runCmd = GameController.Command.Run.ToString();

      return _viewFactory.Create(
        new MainMenuView.Data(message.From.Id, new MainMenuView.MenuItem[] {
          new MainMenuView.MenuItem{
            Text = "Начать игру",
            Callback = gameCommandBuilder.Clear().Add(runCmd).Build()
          },
          new MainMenuView.MenuItem{
            Text = "Начать игру (20 слов)",
            Callback = gameCommandBuilder.Clear().Add(runCmd, "20").Build()
          },
          new MainMenuView.MenuItem{
            Text = "Начать игру (30 слов)",
            Callback = gameCommandBuilder.Clear().Add(runCmd, "30").Build()
          },
       })).Render(_telegramBotClient);
    }

    public override Task HandleCallbackAsync(CallbackQuery query, IEnumerable<string> parsedArgs) =>
      throw new NotImplementedException();
  }
}