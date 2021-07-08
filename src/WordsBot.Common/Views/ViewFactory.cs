using System;

namespace WordsBot.Common.Views
{
  class ViewFactory : IViewFactory
  {
    public IView Create<T>(T data) => data switch
    {
      TranslateWordView.Data d => (IView)new TranslateWordView(d),
      AddRemoveWordView.Data d => (IView)new AddRemoveWordView(d),
      _ => throw new ArgumentException($"can't use param of type {data?.GetType()}", nameof(data))
    };
  }
}