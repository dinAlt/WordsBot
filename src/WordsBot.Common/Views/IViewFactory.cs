namespace WordsBot.Common.Views
{
  public interface IViewFactory
  {
    IView Create<T>(T data);
  }
}