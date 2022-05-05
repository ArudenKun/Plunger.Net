namespace Plunger.Data.Entities;

public class SuggestionEntity
{
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
    public string GuildName {get; set;} = "";
    public ulong MessageId { get; set; }
    public ulong MemberId { get; set; }
    public string? Type { get; set; }
    public string? Suggestion { get; set; }
    public string? Status { get; set; }
}