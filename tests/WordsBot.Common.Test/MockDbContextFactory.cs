using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WordsBot.Common.Test
{
  internal class MockDbContextFactory : IDbContextFactory<MockDbContext>
  {
    public MockDbContext CreateDbContext()
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