using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plunger.Models;

public class Enum
{
    public enum SuggestionTypes
    {
        Command,
        Event,
        System,
        Other,
    }

    public enum DurationType
    {
        Seconds,
        Minutes,
        Hours,
        Days,
    }
}
