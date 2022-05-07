using System.Runtime.Serialization;

namespace Plunger.APIs.Nekobot.Endpoints;

public enum NekobotTypes
{
    [EnumMember(Value = "hass")] Hass,
    [EnumMember(Value = "hmidriff")] Hmidriff,
    [EnumMember(Value = "pgif")] Pgif,
    [EnumMember(Value = "hentai")] Hentai,
    [EnumMember(Value = "hneko")] Hneko,
    [EnumMember(Value = "hkitsune")] Hkitsune,
    [EnumMember(Value = "kemonomimi")] Kemonomimi,
    [EnumMember(Value = "hthigh")] Hthigh,
    [EnumMember(Value = "pussy")] Pussy,
    [EnumMember(Value = "gonewild")] Gonewild,
}