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
    }

    public GameController(WordsBotDbContext dbContext, ITelegramBotClient telegramBotClient, ICommandBuilder commandBuilder, IViewFactory viewFactory) : base(dbContext, telegramBotClient, commandBuilder, viewFactory)
    {
    }

    public GameSession.GameState State
    {
      get
      {
        return _gameSession.State;
      }
    }

    public override Task HandleCallbackAsync(CallbackQuery query, IEnumerable<string> parsedArgs)
    {
      if (parsedArgs is null || !parsedArgs.Any())
      {
        throw new ArgumentException("Collection is empty.", nameof(parsedArgs));
      }

      return Enum.Parse<Command>(parsedArgs.First()) switch
      {
        Command.Run => HandleRunAsync(query, parsedArgs.Skip(1)),
        Command.Fail => HandleFailAsync(query, parsedArgs.Skip(1)),
        _ => throw new NotImplementedException()
      };
    }

    public override Task HandleMessageAsync(Message message) => State switch
    {
      GameSession.GameState.Undefined => throw new NotImplementedException(),
      GameSession.GameState.Running => HandleAnswerMessageAsync(message),
      GameSession.GameState.Ended => throw new NotImplementedException(),
      GameSession.GameState.Paused => throw new NotImplementedException(),
      GameSession.GameState.WaitingCount => HandleWordCountMessageAsync(message),
      _ => throw new NotImplementedException()
    };

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
        await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "количество должно быть положительным числом");
        return;
      }

      await StartGameAsync(message.Chat.Id, count);
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
        _dbContext.Update(_gameSession);
        _dbContext.SaveChanges();

        await _telegramBotClient.SendTextMessageAsync(query.From.Id,
          "Введите количество слов");
        await ReplyCallbackAsync(query);
        return;
      }

      await ReplyCallbackAsync(query);
      await StartGameAsync(query.ChatInstance, count);
      _dbContext.SaveChanges();
    }

    private async Task StartGameAsync(ChatId forChat, int count)
    {
      await _telegramBotClient.SendTextMessageAsync(forChat, "Игра началась");
      _gameSession.TotalWordsCount = count;
      _gameSession.State = GameSession.GameState.Running;
      await SendNextWordAsync(forChat);
    }

    private async Task SendNextWordAsync(ChatId to)
    {
      _gameSession.CurrentWord = _dbContext.RandomWord(to.Identifier);
      _gameSession.CurrentWordNumber++;
      _dbContext.Update(_gameSession);

      await _telegramBotClient.SendTextMessageAsync(to,
        $"{_gameSession.CurrentWord}\n" +
        $"({_gameSession.CurrentWordNumber}\\{_gameSession.TotalWordsCount}" +
        $"\\{_gameSession.FailsCount})",
        replyMarkup: new InlineKeyboardMarkup
          (
            new InlineKeyboardButton
            {
              Text = "не знаю",
              CallbackData = _commandBuilder.Add(
                Command.Fail.ToString(),
                _gameSession.CurrentWord
              ).Build(),
            }
          )
        );
    }

    private async Task HandleFailAsync(CallbackQuery query, IEnumerable<string> args)
    {
      await ReplyNotImplementedAsync(query.From.Id);
    }


    private async Task HandlePauseAsync(CallbackQuery query)
    {
      await ReplyNotImplementedAsync(query.From.Id);
    }

    private async Task HandleResumeAsync(CallbackQuery query)
    {
      await ReplyNotImplementedAsync(query.From.Id);
    }

    private Task ReplyNotImplementedAsync(long to)
    {
      return _telegramBotClient.SendTextMessageAsync(
        to, "Комманда не релизована");
    }

    private Task ReplyCallbackAsync(CallbackQuery query, string text = null)
    {
      return _telegramBotClient.AnswerCallbackQueryAsync(query.Id, text);
    }
  }
}