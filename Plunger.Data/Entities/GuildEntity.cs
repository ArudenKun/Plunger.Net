using System.ComponentModel.DataAnnotations.Schema;

namespace Plunger.Data.Entities;

public class GuildEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong Id { get; set; }
    public string GuildName {get; set;} = string.Empty;
    public string Prefix { get; set; } = "!";
    public ulong WelcomeChannelId { get; set; }
    public ulong GoodbyeChannelId { get; set; }
    public ulong LoggingChannelId { get; set; }
    public List<string> Words { get; set; } = new List<string> { " " };
}