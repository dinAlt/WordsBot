using System.Threading.Tasks;
using Telegram.Bot;
using WordsBot.Common.Views;

namespace WordsBot.Common.Test
{
  class MockView : View<object>
  {
    public bool Rendered { get; private set; }
    public object Data { get => _data; }

    public MockView(object data) : base(data)
    {
    }

    public override Task Render(ITelegramBotClient botClient)
    {
      Rendered = true;
      return Task.CompletedTask;
    }
  }
}