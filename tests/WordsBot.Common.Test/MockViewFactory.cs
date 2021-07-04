using System.Collections.Generic;
using WordsBot.Common.Views;

namespace WordsBot.Common.Test
{
  class MockViewFactory : IViewFactory
  {
    public List<IView> Views { get; } = new();
#pragma warning disable CS8604
    public IView Create<T>(T data)
    {
      IView res = new MockView(data);
      Views.Add(res);
      return res;
    }
#pragma warning restore
  }
}