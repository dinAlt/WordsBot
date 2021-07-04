using Microsoft.EntityFrameworkCore;

namespace WordsBot.Common.Models
{
  public abstract class WordsBotDbContext : DbContext
  {
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<TrainingTranslation> TrainingTranslations => Set<TrainingTranslation>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<UserInfo> Users => Set<UserInfo>();

    public abstract string RandomWord(long userId);
  }
}