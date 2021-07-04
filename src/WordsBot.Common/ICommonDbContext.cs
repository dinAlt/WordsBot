using System;
using System.Collections.Generic;

namespace WordsBot.Common
{
  public interface ICommonDbContext : IDisposable
  {
    List<string> GetTranslation(string word, string from, string to);
    void AddTranslation(string word, string from, string to, IEnumerable<string> variants);
    bool IsTrainingWord(long userId, string word);
    string RandomWordFor(long userId);
    void AddTrainingWord(long userId, string word);
    void RemoveTrainingWord(long userId, string word);
    void SaveChanges();
  }
}