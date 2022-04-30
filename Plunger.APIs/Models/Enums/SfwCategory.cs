using System.Runtime.Serialization;

namespace Plunger.APIs.Models.Enums;

public enum SfwCategory
{
    [EnumMember(Value = "waifu")] Waifu,
    [EnumMember(Value = "neko")] Neko,
    [EnumMember(Value = "shinobu")] Shinobu,
    [EnumMember(Value = "megumin")] Megumin,
    [EnumMember(Value = "bully")] Bully,
    [EnumMember(Value = "cuddle")] Cuddle,
    [EnumMember(Value = "cry")] Cry,
    [EnumMember(Value = "hug")] Hug,
    [EnumMember(Value = "awoo")] Awoo,
    [EnumMember(Value = "kiss")] Kiss,
    [EnumMember(Value = "lick")] Lick,
    [EnumMember(Value = "pat")] Pat,
    [EnumMember(Value = "smug")] Smug,
    [EnumMember(Value = "bonk")] Bonk,
    [EnumMember(Value = "blush")] Blush,
    [EnumMember(Value = "smile")] Smile,
    [EnumMember(Value = "highfive")] Highfive,
    [EnumMember(Value = "nom")] Nom,
    [EnumMember(Value = "bite")] Bite,
    [EnumMember(Value = "slap")] Slap,
    [EnumMember(Value = "happy")] Happy,
    [EnumMember(Value = "kick")] Kick,
    [EnumMember(Value = "wink")] Wink,
    [EnumMember(Value = "poke")] Poke,
    [EnumMember(Value = "dance")] Dance,
    [EnumMember(Value = "cringe")] Cringe
}