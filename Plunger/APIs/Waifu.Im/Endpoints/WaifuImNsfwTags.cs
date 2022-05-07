using System.Runtime.Serialization;

namespace Plunger.APIs.Waifu.Im.Endpoints;

public enum WaifuImNsfwTags
{
    [EnumMember(Value = "ass")] Ass,
    [EnumMember(Value = "Hentai")] Hentai,
    [EnumMember(Value = "milf")] Milf,
    [EnumMember(Value = "oral")] Oral,
    [EnumMember(Value = "paizuri")] Paizuri,
    [EnumMember(Value = "ecchi")] Ecchi,
    [EnumMember(Value = "ero")] Ero,
}
