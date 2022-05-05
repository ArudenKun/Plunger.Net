namespace Plunger.Data.Entities;

public class LockdownEntity
{
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
    public string? GuildName { get; set; }
    public ulong ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public ulong MessageId { get; set; }
    public DateTimeOffset Duration { get; set; }
    public string Reason { get; set; } = "None";
}
