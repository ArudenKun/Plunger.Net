using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Plunger.Data.Entities;

namespace Plunger.Data;

public class PlungerDbContext : DbContext
{
    public DbSet<GuildEntity>? Guilds { get; set; }
    public DbSet<SuggestionEntity>? Suggestions { get; set; }
    public DbSet<LockdownEntity>? Lockdowns { get; set; }

    public PlungerDbContext(DbContextOptions<PlungerDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildEntity>()
            .Property(e => e.Words)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)default!),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)default!)!);
    }
}
