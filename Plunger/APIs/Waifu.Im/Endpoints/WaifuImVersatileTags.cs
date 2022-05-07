using System.Runtime.Serialization;

namespace Plunger.APIs.Waifu.Im.Endpoints;

public enum WaifuImVersatileTags
{
    [EnumMember(Value = "uniform")] Uniform,
    [EnumMember(Value = "maid")] Maid,
    [EnumMember(Value = "waifu")] Waifu,
    [EnumMember(Value = "marin-kitagawa")] Marin_Kitagawa,
    [EnumMember(Value = "mori-calliope")] Mori_Calliope,
    [EnumMember(Value = "raiden-shogun")] Raiden_Shogun,
    [EnumMember(Value = "oppai")] Oppai,
    [EnumMember(Value = "selfies")] Selfies,
}