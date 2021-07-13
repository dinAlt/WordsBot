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
  public abstract class Controller
  {
    public Controller(WordsBotDbContext dbContext, ITelegramBotClient telegramBotClient,
      ICommandBuilder commandBuilder, IViewFactory viewFactory)
    {
      _dbContext = dbContext;
      _telegramBotClient = telegramBotClient;
      _commandBuilder = commandBuilder;
      _viewFactory = viewFactory;
    }

    public abstract Task HandleMessageAsync(Message message);
    public abstract Task HandleCallbackAsync(CallbackQuery query, IEnumerable<string> parsedArgs);

    protected readonly WordsBotDbContext _dbContext;
    protected readonly ITelegramBotClient _telegramBotClient;
    protected readonly ICommandBuilder _commandBuilder;
    protected readonly IViewFactory _viewFactory;

    protected UserInfo GetUser(long userId) =>
      _dbContext.Users.Where(e => e.UserInfoId == userId)
        .FirstOrDefault() ?? new UserInfo { UserInfoId = userId };
  }
}