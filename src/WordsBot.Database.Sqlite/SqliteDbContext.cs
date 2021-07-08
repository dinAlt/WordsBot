using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WordsBot.Common.Models;

namespace WordsBot.Database.Sqlite
{
  public class SqliteDbContext : WordsBotDbContext
  {
    public SqliteDbContext(string dbPath)
    {
      _conString = dbPath;
    }

    public SqliteDbContext() : this("Filename=:memory:")
    {
    }

    public override string RandomWord(long userId) => TrainingTranslations.
      FromSqlInterpolated(
        $"select * from TrainingTranslations where UserId = {userId} order by RANDOM() limit 1").
      FirstOrDefault()?.Word ?? string.Empty;

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
