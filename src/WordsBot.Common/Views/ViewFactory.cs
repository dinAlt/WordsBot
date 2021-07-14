using System;

namespace WordsBot.Common.Views
{
  public class ViewFactory : IViewFactory
  {
    public IView Create<T>(T data) => data switch
    {
      TranslateWordView.Data d => new TranslateWordView(d),
      AddRemoveWordView.Data d => new AddRemoveWordView(d),
      ErrorView.Data d => new ErrorView(d),
      FailGameView.Data d => new FailGameView(d),
      FinishGameView.Data d => new FinishGameView(d),
      GiveUpGameView.Data d => new GiveUpGameView(d),
      NextWordView.Data d => new NextWordView(d),
      PauseResumeGameView.Data d => new PauseResumeGameView(d),
      RunGameView.Data d => new RunGameView(d),
      MainMenuView.Data d => new MainMenuView(d),
      _ => throw new ArgumentException(
        $"can't use param of type {data?.GetType()}", nameof(data))
    };
  }
}