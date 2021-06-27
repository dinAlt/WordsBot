using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WordsBot.Database.Sqlite;

namespace WordsBot.Common.Test
{
  class MockDbContext : SqliteDbContext
  {
    static readonly SqliteConnection connection;

    static MockDbContext()
    {
      connection = new SqliteConnection("Filename=:memory:");
      connection.Open();
    }

    public MockDbContext() : base()
    {

    }

    public MockDbContext(string dbPath) : base(dbPath)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      options.UseSqlite(connection);
    }
  }
}