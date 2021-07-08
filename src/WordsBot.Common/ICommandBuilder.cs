namespace WordsBot.Common
{
  public interface ICommandBuilder
  {
    ICommandBuilder Add(params string[] args);
    string Build();
  }
}