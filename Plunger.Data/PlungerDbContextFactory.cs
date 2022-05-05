using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Plunger.Data;

public class PlungerDbContextFactory : IDesignTimeDbContextFactory<PlungerDbContext>
{
    public PlungerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PlungerDbContext>()
            .UseSqlite("Data Source=..\\Plunger\\Data.db")
            .Options;
        return new PlungerDbContext(optionsBuilder);
    }
}