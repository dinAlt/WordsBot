using Microsoft.EntityFrameworkCore;
using WordsBot.Common;

namespace WordsBot.Database.Sqlite
{
  public class SqliteDbContextFactory : IDbContextFactory
  {
    private readonly string _conString;

    public SqliteDbContextFactory(string conSring) => (_conString) = (conSring);


    public IDbContext GetContext()
    {
      var res = new SqliteDbContext(_conString);
      res.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      return res;
    }
  }
}
