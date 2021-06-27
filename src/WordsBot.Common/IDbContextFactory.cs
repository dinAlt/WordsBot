namespace WordsBot.Common
{
  public interface IDbContextFactory
  {
    IDbContext GetContext();
  }
}