using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WordsBot.Common.Test
{
  internal class MockDbContextFactory : IDbContextFactory
  {
    public IDbContext GetContext()
    {
      var res = new MockDbContext("");
      if (res.Database.GetMigrations().Any())
      {
        res.Database.Migrate();
      }
      return res;
    }
  }
}