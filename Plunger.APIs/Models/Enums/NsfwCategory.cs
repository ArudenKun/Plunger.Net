﻿using System.Runtime.Serialization;

namespace Plunger.APIs.Models.Enums;

public enum NsfwCategory
{
    [EnumMember(Value = "waifu")] Waifu,
    [EnumMember(Value = "neko")] Neko,
    [EnumMember(Value = "trap")] Trap,
    [EnumMember(Value = "blowjob")] Blowjob
}