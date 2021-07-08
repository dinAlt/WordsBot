using Microsoft.EntityFrameworkCore;
using WordsBot.Common;
using WordsBot.Common.Models;

namespace WordsBot.Database.Sqlite
{
  public class SqliteDbContextFactory : IDbContextFactory<WordsBotDbContext>
  {
    private readonly string _conString;

    public SqliteDbContextFactory(string conSring) => (_conString) = (conSring);

    public WordsBotDbContext CreateDbContext()
    {
      var res = new SqliteDbContext(_conString);
      res.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      return res;
    }
  }
}
