using Microsoft.EntityFrameworkCore;

namespace WordsBot.Common.Models
{
  abstract class WordsBotDbContext : DbContext
  {
    // public DbSet<Translation> Translations => Set<Translation>();
    // public DbSet<TrainingTranslation> TrainingTranslations => Set<TrainingTranslation>();
    // public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<UserData> Users => Set<UserData>();

    public abstract string RandomWord(long userId);
  }
}