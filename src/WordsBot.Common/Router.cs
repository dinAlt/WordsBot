using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WordsBot.Common.Controllers;
using WordsBot.Common.Views;

namespace WordsBot.Common
{
  public class Router : IRouter
  {
    enum CallbackRoute
    {
      Translator,
      Game,
      Menu,
    }

    public Router(
      IDbContextFactory<Models.WordsBotDbContext> dbContextFactory,
      ITelegramBotClient telegramBotClient, IViewFactory viewFactory,
      ITranslator translator) =>
      (_dbContextFactory, _telegramBotClient, _viewFactory, _translator) =
        (dbContextFactory, telegramBotClient, viewFactory, translator);

    public Task RouteUpdateAsync(Update update) => update.Type switch
    {
      UpdateType.Message => RouteMessageAsync(update.Message),
      UpdateType.CallbackQuery => RouteCallbackQueryAsync(update.CallbackQuery),
      _ => Task.CompletedTask,
    };

    readonly IDbContextFactory<Models.WordsBotDbContext> _dbContextFactory;
    readonly ITelegramBotClient _telegramBotClient;
    readonly IViewFactory _viewFactory;
    readonly ITranslator _translator;

    async Task RouteCallbackQueryAsync(CallbackQuery query)
    {
      IEnumerable<string> args = query.Data.Split("|");
      if (!args.Any())
      {
        throw new Exception("Command do not contains any argumets");
      }
      var route = Enum.Parse<CallbackRoute>(args.First());
      var commandBuilder = new CommandBuilder('|', new string[] { route.ToString() });

      using var dbContext = _dbContextFactory.CreateDbContext();

      await (route switch
      {
        CallbackRoute.Translator => new TranslateController(
          dbContext, _telegramBotClient, commandBuilder, _viewFactory, _translator).
          HandleCallbackAsync(query, args.Skip(1)),
        CallbackRoute.Game => new GameController(
          dbContext, _telegramBotClient, commandBuilder, query.From.Id, _viewFactory).
          HandleCallbackAsync(query, args.Skip(1)),
        _ => throw new NotImplementedException()
      });
    }

    async Task RouteMessageAsync(Message message)
    {
      using var dbContext = _dbContextFactory.CreateDbContext();
      var text = message.Text.Trim().ToLower();
      if (text == "/start" || text == "/menu")
      {
        await new MenuController(dbContext, _telegramBotClient,
          new CommandBuilder('|', new string[] { CallbackRoute.Game.ToString() }), _viewFactory).
            HandleMessageAsync(message);
        return;
      }

      var gameController = new GameController(dbContext, _telegramBotClient,
        new CommandBuilder('|', new string[] { CallbackRoute.Game.ToString() }),
        message.From.Id, _viewFactory);

      if (gameController.State == Models.GameSession.GameState.Running ||
        gameController.State == Models.GameSession.GameState.WaitingCount)
      {
        await gameController.HandleMessageAsync(message);
        return;
      }

      await new TranslateController(dbContext, _telegramBotClient,
        new CommandBuilder('|', new string[] { CallbackRoute.Translator.ToString() }), _viewFactory, _translator).HandleMessageAsync(message);
    }
  }
}