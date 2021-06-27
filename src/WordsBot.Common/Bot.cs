using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordsBot.Common
{
  public class Bot
  {
    readonly ITranslator _translator;
    readonly IDbContextFactory _dbFactory;
    readonly ITelegramBotClient _telegramBotClient;
    static readonly Regex _wordMessageRegex;

#nullable enable
    QueuedUpdateReceiver? updateReceiver;
#nullable disable

    public Bot(string accessToken, ITranslator translater, IDbContextFactory dbFactory) : this(new TelegramBotClient(accessToken), translater, dbFactory) { }

    public Bot(ITelegramBotClient botClient, ITranslator translator, IDbContextFactory dbFactory)
    {
      _telegramBotClient = botClient;
      _translator = translator;
      _dbFactory = dbFactory;
    }

    static Bot()
    {
      _wordMessageRegex = new Regex(@"(?<word>.*)\n\((?<num>\d+)\\(?<of>\d+)\)", RegexOptions.Compiled | RegexOptions.Multiline);
    }

    public async Task Run()
    {
      var me = await _telegramBotClient.GetMeAsync();
      if (me == null)
      {
        throw new Exception("not me at all");
      }

      updateReceiver = new QueuedUpdateReceiver(_telegramBotClient);
      updateReceiver.StartReceiving();

      await foreach (var update in updateReceiver.YieldUpdatesAsync())
      {
        switch (update.Type)
        {
          case UpdateType.Message:
            Message m = update.Message;
            if (m.Text.StartsWith('/') && m.Text.Length > 1)
            {
              _ = HandleCommnadAsync(m);
              continue;
            }
            Message replyTo = m.ReplyToMessage;
            if (replyTo != null)
            {
              _ = HandleMessageReplyAsync(m);
              continue;
            }
            _ = HandleTranslateAsync(m);
            break;
          case UpdateType.CallbackQuery:
            _ = HandleCallbackQueryAsync(update.CallbackQuery);
            break;
        }
      }
    }

    private async Task HandleMessageReplyAsync(Message m)
    {
      var (match, word, num, of) = ParseWordMessage(m.ReplyToMessage.Text);
      if (!match) return;
      await HandleWordReplyAsync(m, word, num, of);
    }

    private async Task HandleWordReplyAsync(Message reply, string word, int num, int of)
    {
      var dbContext = _dbFactory.GetContext();
      var translations = dbContext.GetTranslation(word, "en", "ru");
      if (translations.Count == 0)
      {
        await _telegramBotClient.SendTextMessageAsync(reply.From.Id, "нет перевода для");
        return;
      }

      if (!translations.Any(t => t == reply.Text.ToLower().Trim()))
      {
        await _telegramBotClient.SendTextMessageAsync(reply.From.Id, "Не верно",
          replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
          {
            Text = "показать перевод",
            CallbackData = $"{CallbackCommand.Translate}|{word}",
          }));
        return;
      }

      if (num < of)
      {
        await SendNextWordAsync(reply.From.Id, ++num, of, dbContext);
        return;
      }

      await _telegramBotClient.SendTextMessageAsync(reply.From.Id, "Игра завершена!");
    }

    private async Task SendNextWordAsync(long userId, int num, int of, IDbContext dbContext)
    {
      await _telegramBotClient.SendTextMessageAsync(userId,
        $"<b>{dbContext.RandomWordFor(userId)}</b>\n({num}\\{of})",
        parseMode: ParseMode.Html);
    }

    public static (bool match, string word, int num, int of) ParseWordMessage(string text)
    {
      var match = _wordMessageRegex.Match(text);
      if (!match.Success)
      {
        return (false, "", -1, -1);
      }

      var word = match.Groups["word"].Value;
      var num = int.Parse(match.Groups["num"].Value);
      var of = int.Parse(match.Groups["of"].Value);

      return (true, word, num, of);
    }

    private async Task HandleCommnadAsync(Message m)
    {
      var txtCmd = m.Text.TrimStart('/').Split(' ', 2,
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .First().ToLower();
      var cmd = Enum.Parse<MessageCommand>(
        txtCmd[..1].ToUpper() + txtCmd[1..]);
      var dbContext = _dbFactory.GetContext();
      switch (cmd)
      {
        case MessageCommand.Run:
          await HandleRunCmdAsync(m, dbContext);
          break;
        default:
          await _telegramBotClient.SendTextMessageAsync(m.From.Id,
          "комманда не поддерживается");
          break;
      }
    }

    private async Task HandleRunCmdAsync(Message m, IDbContext dbContext)
    {
      if (!int.TryParse(m.Text.Remove(0, m.Text.IndexOf(' ') + 1).Trim(), out int count))
      {
        await _telegramBotClient.SendTextMessageAsync(m.From.Id,
         "Неверно указано количество.");
        return;
      }

      await SendNextWordAsync(m.From.Id, 1, count, dbContext);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery q)
    {
      using var dbContext = _dbFactory.GetContext();
      var cmd = Enum.Parse<CallbackCommand>(q.Data.Split('|', 2,
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First());
      switch (cmd)
      {
        case CallbackCommand.Add:
          await HandleAddCmdAsync(q, dbContext);
          break;
        case CallbackCommand.Remove:
          await HandleRemoveCmdAsync(q, dbContext);
          break;
        case CallbackCommand.Translate:
          await HandleTranslateCmdAsync(q, dbContext);
          break;
        default:
          throw new NotImplementedException($"CallbackCommand: {cmd}");
      }
    }

    private async Task HandleTranslateCmdAsync(CallbackQuery q, IDbContext dbContext)
    {
      var word = q.Data.Remove(0, q.Data.IndexOf('|') + 1);
      var translations = dbContext.GetTranslation(word, "en", "ru");
      await _telegramBotClient.SendTextMessageAsync(q.From.Id,
        string.Join("\n", translations), ParseMode.Html);
    }

    private async Task HandleAddCmdAsync(CallbackQuery q, IDbContext dbContext)
    {
      var word = q.Data.Remove(0, q.Data.IndexOf('|') + 1);
      if (!dbContext.IsTrainingWord(q.From.Id, word))
      {
        dbContext.AddTrainingWord(q.From.Id, word);
        dbContext.SaveChanges();
      }
      await _telegramBotClient.AnswerCallbackQueryAsync(q.Id, $"Добавлено: {word}");
    }

    private async Task HandleRemoveCmdAsync(CallbackQuery q, IDbContext dbContext)
    {
      var word = q.Data.Remove(0, q.Data.IndexOf('|') + 1);
      dbContext.RemoveTrainingWord(q.From.Id, word);
      dbContext.SaveChanges();
      await _telegramBotClient.AnswerCallbackQueryAsync(q.Id, $"Удалено: {word}");
    }

    private async Task HandleTranslateAsync(Message m)
    {
      using var dbContext = _dbFactory.GetContext();

      var mText = m.Text.ToLower().Trim();
      var translations = dbContext.GetTranslation(mText, "en", "ru");

      if (translations.Count == 0)
      {
        translations = await TranslateAsync(mText, "en", "ru", dbContext);
        if (translations.Count == 0)
        {
          await _telegramBotClient.SendTextMessageAsync(m.Chat, "нет перевода");
          return;
        }
      }

      var btn = dbContext.IsTrainingWord(m.From.Id, mText) ?
          new InlineKeyboardButton
          {
            Text = "Удалить",
            CallbackData = $"{CallbackCommand.Remove}|{mText}"
          } :
          new InlineKeyboardButton
          {
            Text = "Добавить",
            CallbackData = $"{CallbackCommand.Add}|{mText}"
          };
      var kb = new InlineKeyboardMarkup(btn);

      await _telegramBotClient.SendTextMessageAsync(m.Chat,
        string.Join("\n", translations),
        parseMode: ParseMode.Html, replyMarkup: kb);
    }

    private async Task<List<string>> TranslateAsync(string word, string from, string to, IDbContext dbContext)
    {
      var variants = await _translator.TranslateAsync(word, from, to);
      if (variants.Count > 0)
      {
        dbContext.AddTranslation(word, from, to, variants);
        dbContext.SaveChanges();
      }

      return variants;
    }

    public async Task Stop()
    {
      updateReceiver?.StopReceiving();
      await _telegramBotClient.CloseAsync();
    }
  }

  public enum CallbackCommand
  {
    Add,
    Remove,
    Translate,
  }

  public enum MessageCommand
  {
    Run
  }
}
