using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WordsBot.Common.Models;
using WordsBot.Common.Views;

namespace WordsBot.Common.Controllers
{
  class GameController : Controller
  {
    readonly GameSession _gameSession;

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
      throw new NotImplementedException();
    }

    public override async Task HandleMessageAsync(Message message)
    {
      switch (State)
      {
        case GameSession.GameState.Running:
          await HandleAnswerMessage(message);
          break;
        case GameSession.GameState.WaitingCount:
          await HandleWordCountMessage(message);
          return;
        default:
          throw new Exception($"unexpected game state: {State}");
      }
    }

    private enum CallbackCommand
    {
      Run,
      Fail,
    }

    private Task HandleAnswerMessage(Message message)
    {
      throw new NotImplementedException();
    }

    private async Task HandleWordCountMessage(Message message)
    {
      _ = int.TryParse(message.Text.Trim(), out int count);
      if (count < 1)
      {
        await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "количество должно быть положительным числом");
        return;
      }

      await StartGame(message.Chat.Id, count);
    }

    private async Task HandleStart(CallbackQuery query, string[] parsedArgs)
    {
      int count = 0;

      if (parsedArgs.Length > 0 && !int.TryParse(parsedArgs[0], out count))
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
        await ReplyCallback(query);
        return;
      }

      await ReplyCallback(query);
      await StartGame(query.ChatInstance, count);
      _dbContext.SaveChanges();
    }

    private async Task StartGame(ChatId forChat, int count)
    {
      await _telegramBotClient.SendTextMessageAsync(forChat, "Игра началась");
      _gameSession.TotalWordsCount = count;
      _gameSession.State = GameSession.GameState.Running;
      await SendNextWord(forChat);
    }

    private async Task SendNextWord(ChatId to)
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
                CallbackCommand.Fail.ToString(),
                _gameSession.CurrentWord
              ).Build(),
            }
          )
        );
    }

    private async Task HandlePause(CallbackQuery query)
    {
      await ReplyNotImplemented(query.From.Id);
    }

    private async Task HandleResume(CallbackQuery query)
    {
      await ReplyNotImplemented(query.From.Id);
    }

    private Task ReplyNotImplemented(long to)
    {
      return _telegramBotClient.SendTextMessageAsync(
        to, "Комманда не релизована");
    }

    private Task ReplyCallback(CallbackQuery query, string text = null)
    {
      return _telegramBotClient.AnswerCallbackQueryAsync(query.Id, text);
    }
  }
}