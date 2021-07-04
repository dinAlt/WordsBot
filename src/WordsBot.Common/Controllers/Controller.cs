using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WordsBot.Common.Models;

namespace WordsBot.Common.Controllers
{
  abstract class Controller
  {
    protected readonly WordsBotDbContext _dbContext;
    protected readonly ITelegramBotClient _telegramBotClient;
    protected readonly ICommandBuilder _commandBuilder;

    public Controller(WordsBotDbContext dbContext, ITelegramBotClient telegramBotClient,
     ICommandBuilder commandBuilder) =>
      (_dbContext, _telegramBotClient, _commandBuilder) =
      (dbContext, telegramBotClient, commandBuilder);

    public abstract Task HandleMessageAsync(Message message);
    public abstract Task HandleCallbackAsync(CallbackQuery query, string[] parsedArgs);

    protected UserData GetUser(long userId) =>
      _dbContext.Users.Where(e => e.UserId == userId)
        .FirstOrDefault() ?? new UserData { UserId = userId };
  }
}