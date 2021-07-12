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
  public class TranslateController : Controller
  {
    public enum Command
    {
      Add,
      Remove
    }

    public TranslateController(WordsBotDbContext dbContext,
      ITelegramBotClient telegramBotClient, ICommandBuilder commandBuilder, IViewFactory? viewFactory = default, ITranslator? translator = default) : base
      (dbContext, telegramBotClient, commandBuilder, viewFactory)
    {
      _translator = translator ?? Translator ?? throw new Exception("translator not set");
    }

    public override Task HandleCallbackAsync(CallbackQuery query, IEnumerable<string> parsedArgs)
    {
      if (parsedArgs.Count() < 2)
      {
        throw new ArgumentException("To few callback arguments", nameof(parsedArgs));
      }

      var command = Enum.Parse<Command>(parsedArgs.First());
      var word = parsedArgs.ElementAt(1);
      var isTraining = false;
      switch (command)
      {
        case Command.Add:
          if (!_dbContext.TrainingTranslations.Any(
            t => t.UserId == query.From.Id && t.Word == word))
          {
            _dbContext.TrainingTranslations.Add(
              new TrainingTranslation(query.From.Id, word));
          }
          isTraining = true;
          break;
        case Command.Remove:
          var translation = _dbContext.TrainingTranslations.
            FirstOrDefault(t => t.UserId == query.From.Id && t.Word == word);
          if (translation != null)
          {
            _dbContext.TrainingTranslations.Remove(translation);
          }
          break;
        default:
          throw new NotImplementedException($"command not implemented: {command}");
      }

      _dbContext.SaveChanges();

      var view = _viewFactory.Create(new AddRemoveWordView.Data(
        query.Message.Chat.Id,
        query.Message.MessageId,
        query.Id,
        isTraining,
        _commandBuilder.Add(
          $"{(isTraining ? Command.Remove : Command.Add)}", word).Build()));

      return view.Render(_telegramBotClient);
    }

    public override async Task HandleMessageAsync(Message message)
    {
      var word = message.Text.Trim().ToLower();
      var translations = _dbContext.Translations.FirstOrDefault(
        t => t.Word == word && t.From == "en" && t.To == "ru")?.
          Values ?? new List<string>();
      bool isTraining = false;

      if (!translations.Any())
      {
        translations = (await _translator.TranslateAsync(word, "en", "ru")).ToList();
        var translation = new Translation(word, "en", "ru");
        translation.Values.AddRange(translations);
        _dbContext.Update(translation);
        _dbContext.SaveChanges();
      }
      else
      {
        isTraining = _dbContext.TrainingTranslations.Any(
          t => t.UserId == message.From.Id && t.Word == word);
      }

      var view = _viewFactory.Create(new TranslateWordView.Data(
        message.From.Id,
        word,
        translations,
        isTraining,
        _commandBuilder.Add(
          $"{(isTraining ? Command.Remove : Command.Add)}", word).Build()));

      await view.Render(_telegramBotClient);
    }

    readonly ITranslator _translator;
    static ITranslator? Translator { get; set; }
  }
}