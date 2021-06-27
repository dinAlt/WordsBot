using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WordsBot.Database.Sqlite.Models
{
  [Index(nameof(Word), nameof(From), nameof(To), IsUnique = true)]
  public class Translation
  {
    public Translation(string word, string from, string to)
    {
      Word = word;
      From = from;
      To = to;
    }

    public int TranslationId { get; set; }
    public string Word { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public List<string> Values { get; } = new List<string>();
  }

  public class TrainingTranslation
  {
    public TrainingTranslation(long userId, string word)
    {
      UserId = userId;
      Word = word;
    }

    public long UserId { get; set; }
    public string Word { get; set; }
  }
}