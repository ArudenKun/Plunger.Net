using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Plunger.Data;

public class PlungerDbContextFactory : IDesignTimeDbContextFactory<PlungerDbContext>
{
    public PlungerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder()
            .UseSqlite("Data Source=..\\Plunger\\Data.db");
        return new PlungerDbContext(optionsBuilder.Options);
    }
}