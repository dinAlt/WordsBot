namespace WordsBot.Common
{
  interface ICommandBuilder
  {
    ICommandBuilder Add(params string[] args);
    string Build();
  }
}