namespace WordsBot.Common
{
  public interface ICommandBuilder
  {
    ICommandBuilder Add(params string[] args);
    ICommandBuilder Clear();
    string Build();
  }
}