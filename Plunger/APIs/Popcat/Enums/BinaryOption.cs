using System.Runtime.Serialization;

namespace Plunger.APIs.Popcat.Enums;

public enum BinaryOption
{
    [EnumMember(Value = "encode")] Encode,
    [EnumMember(Value = "decode")] Decode
}
