namespace WordsBot.Common
{
  public interface IGameDbContext
  {
    GameSession GetSession(long userId);
    void Update(GameSession sesson);
    void AddWord(long userId, string word);
    void RemoveWord(long userId, string word);
    string RandomWord(long userId);
    bool IsWordTraining(long userId, string word);
    void SaveChanges();
  }
}