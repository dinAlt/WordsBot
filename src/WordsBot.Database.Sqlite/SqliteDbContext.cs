using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WordsBot.Common;
using WordsBot.Database.Sqlite.Models;

namespace WordsBot.Database.Sqlite
{
  public class SqliteDbContext : DbContext, IDbContext
  {
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<TrainingTranslation> TrainingTranslations => Set<TrainingTranslation>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();

    public SqliteDbContext(string dbPath)
    {
      _conString = dbPath;
    }

    public SqliteDbContext() : this("Filename=:memory:")
    {
    }

    public void AddTrainingWord(long userId, string word)
    {
      Add(new TrainingTranslation(userId, word));
    }

    public void AddTranslation(string word, string from, string to, IEnumerable<string> variants)
    {
      var newTranslation = new Translation(word, from, to);
      newTranslation.Values.AddRange(variants);
      Add(newTranslation);
      SaveChanges();
    }

    public List<string> GetTranslation(string word, string from, string to)
    {
      return Translations.SingleOrDefault(t => t.Word == word && t.From == from && t.To == to)?
        .Values ?? new List<string>();
    }

    public bool IsTrainingWord(long userId, string word)
    {
      return TrainingTranslations.Any(t => t.UserId == userId && t.Word == word);
    }

    public void RemoveTrainingWord(long userId, string word)
    {
      var translation = TrainingTranslations.SingleOrDefault(t => t.UserId == userId && t.Word == word);
      if (translation != null)
      {
        Remove(translation);
      }
    }

    public string RandomWordFor(long userId)
    {
      return TrainingTranslations.
        FromSqlInterpolated(
          $"select * from TrainingTranslations where UserId = {userId} order by RANDOM() limit 1").
        FirstOrDefault()?.Word ?? string.Empty;
    }

    public void UpdateGameSession(GameSession session)
    {
      Update(session);
    }

    public GameSession GetGamesSession(long userId)
    {
      return GameSessions.Where(e => e.UserId == userId).FirstOrDefault() ?? new();
    }


    void IDbContext.SaveChanges()
    {
      base.SaveChanges();
    }

    private readonly string _conString;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder
        .Entity<Translation>()
        .Property(e => e.Values)
        .HasConversion(
          v => string.Join('|', v),
          v => v.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
          new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList())
          );

      modelBuilder
        .Entity<TrainingTranslation>()
        .HasKey(e => new { e.UserId, e.Word });

      modelBuilder
        .Entity<GameSession>()
        .HasIndex(e => e.UserId)
        .IsUnique(true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Data source={_conString}");
    }
  }
}
