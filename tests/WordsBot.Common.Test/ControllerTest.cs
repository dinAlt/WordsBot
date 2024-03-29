using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WordsBot.Common.Controllers;
using WordsBot.Common.Models;
using WordsBot.Common.Views;

namespace WordsBot.Common.Test
{
  public abstract class ControllerTest
  {


    protected static readonly IDbContextFactory<WordsBotDbContext> _dbContextFactory;
    protected static readonly IViewFactory _viewFactory;
    protected static readonly ITelegramBotClient _botClient;

    static ControllerTest()
    {
      _dbContextFactory = new MockDbContextFactory();
      _viewFactory = new MockViewFactory();
      _botClient = new MockTelegramBotClient();
    }
  }
}