using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WordsBot.Common.Models;
using WordsBot.Common.Views;

namespace WordsBot.Common.Controllers
{
  public class GameController : Controller
  {
    public enum Command
    {
      Run,
      Fail,
      Pause,
      Resume
    }

    public GameController(WordsBotDbContext dbContext,
      ITelegramBotClient telegramBotClient, ICommandBuilder commandBuilder, long userId, IViewFactory? viewFactory = default) :
        base(dbContext, telegramBotClient, commandBuilder, viewFactory)
    {
      _gameSession = _dbContext.GameSessions.
        FirstOrDefault(t => t.UserId == userId) ?? new GameSession { UserId = userId };
    }

    public GameSession.GameState State
    {
      get
      {
        return _gameSession.State;
      }
    }

    public async override Task HandleCallbackAsync(CallbackQuery query,
      IEnumerable<string> parsedArgs)
    {
      if (parsedArgs is null || !parsedArgs.Any())
      {
        throw new ArgumentException("Collection is empty.", nameof(parsedArgs));
      }

      await (Enum.Parse<Command>(parsedArgs.First()) switch
      {
        Command.Run => HandleRunAsync(query, parsedArgs.Skip(1)),
        Command.Fail => HandleFailAsync(query, parsedArgs.Skip(1)),
        Command.Pause => HandlePauseAsync(query),
        Command.Resume => HandleResumeAsync(query),
        _ => throw new NotImplementedException()
      });

      _dbContext.Update(_gameSession);
      _dbContext.SaveChanges();
    }

    public async override Task HandleMessageAsync(Message message)
    {
      await (State switch
      {
        GameSession.GameState.Undefined => throw new NotImplementedException(),
        GameSession.GameState.Running => HandleAnswerMessageAsync(message),
        GameSession.GameState.Ended => throw new NotImplementedException(),
        GameSession.GameState.Paused => throw new NotImplementedException(),
        GameSession.GameState.WaitingCount => HandleWordCountMessageAsync(message),
        _ => throw new NotImplementedException()
      });

      _dbContext.Update(_gameSession);
      _dbContext.SaveChanges();
    }

    readonly GameSession _gameSession;

    private Task HandleAnswerMessageAsync(Message message)
    {
      throw new NotImplementedException();
    }

    private async Task HandleWordCountMessageAsync(Message message)
    {
      _ = int.TryParse(message.Text.Trim(), out int count);
      if (count < 1)
      {
        await _viewFactory.Create(new ErrorView.Data(message.From.Id,
          "количество должно быть положительным целым числом")).
          Render(_telegramBotClient);
        return;
      }

      _gameSession.Reset();
      _gameSession.From = "en";
      _gameSession.To = "ru";
      _gameSession.TotalWordsCount = count;
      _gameSession.State = GameSession.GameState.Running;

      await _viewFactory.Create(new RunGameView.Data(message.From.Id, count)).
        Render(_telegramBotClient);
      await SendNextWordAsync(message.From.Id);
    }

    private async Task HandleRunAsync(CallbackQuery query, IEnumerable<string> parsedArgs)
    {
      int count = 0;

      if (parsedArgs.Any() && !int.TryParse(parsedArgs.First(), out count))
      {
        throw new Exception("count argument is not number");
      }

      if (count == 0)
      {
        count = _gameSession.TotalWordsCount;
      }

      _gameSession.Reset();
      _gameSession.From = "en";
      _gameSession.To = "ru";

      if (count == 0)
      {
        _gameSession.State = GameSession.GameState.WaitingCount;
      }

      await _viewFactory.Create(new RunGameView.Data(query.From.Id, count, query)).
        Render(_telegramBotClient);

      if (count > 0)
      {
        _gameSession.TotalWordsCount = count;
        _gameSession.State = GameSession.GameState.Running;
        await SendNextWordAsync(query.From.Id);
      }
    }

    private Task SendNextWordAsync(ChatId to)
    {
      _gameSession.CurrentWord = _dbContext.RandomWord(to.Identifier);
      _gameSession.CurrentWordNumber++;

      return _viewFactory.Create(
        new NextWordView.Data(
          to.Identifier,
          _gameSession.TotalWordsCount,
          _gameSession.CurrentWordNumber,
          _gameSession.FailsCount,
          _gameSession.CurrentWord,
          _commandBuilder.Add(Command.Fail.ToString(), _gameSession.CurrentWord).Build())).
            Render(_telegramBotClient);
    }

    private async Task HandleFailAsync(CallbackQuery query, IEnumerable<string> args)
    {
      if (!args.Any())
      {
        throw new Exception("Fail command must provide an argument");
      }

      string word = args.First();

      IEnumerable<string> translations = _dbContext.Translations.FirstOrDefault(
        t => t.Word == word)?.Values ??
        throw new Exception($"Unexpectedly no translation for word: {word}");

      await _viewFactory.Create(
        new GiveUpView.Data(query.From.Id, query.Id, translations)).
        Render(_telegramBotClient);
      _gameSession.FailsCount++;
      await SendNextWordAsync(query.From.Id);
    }


    private async Task HandlePauseAsync(CallbackQuery query)
    {
      var state = _gameSession.State;
      var isRunning = (state == GameSession.GameState.Running) ||
          (state == GameSession.GameState.Paused);

      await _viewFactory.Create(
        new PauseResumeGameView.Data(
          query.From.Id,
          query.Id,
          isRunning,
          false)).
        Render(_telegramBotClient);

      _gameSession.State = GameSession.GameState.Paused;
    }

    private async Task HandleResumeAsync(CallbackQuery query)
    {
      var state = _gameSession.State;
      var isRunning = (state == GameSession.GameState.Running) ||
          (state == GameSession.GameState.Paused);

      await _viewFactory.Create(
        new PauseResumeGameView.Data(
          query.From.Id,
          query.Id,
          isRunning,
          true)).
        Render(_telegramBotClient);

      await _viewFactory.Create(
        new NextWordView.Data(
          query.From.Id,
          _gameSession.TotalWordsCount,
          _gameSession.CurrentWordNumber,
          _gameSession.FailsCount,
          _gameSession.CurrentWord,
          _commandBuilder.Add(Command.Fail.ToString(), _gameSession.CurrentWord).Build())).
            Render(_telegramBotClient);

      _gameSession.State = GameSession.GameState.Running;
    }
  }
}