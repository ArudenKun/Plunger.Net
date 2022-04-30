namespace Plunger.Database;

public class PlungerDatabaseConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string GuildCollection { get; set; } = null!;
    public string SuggestionCollection { get; set; } = null!;
    public string TestCollection { get; set; } = null!;
}
