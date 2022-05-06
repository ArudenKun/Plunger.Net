using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Plunger.APIs.WaifuPics.Entities;

public class WaifuImages
{
    [JsonPropertyName("files")]
    public string[]? Files { get; set; }
}
