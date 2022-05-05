namespace Plunger.Modules.Admin.ChatFilters;

public class ChatFilterCache
{
    private static Dictionary<ulong, List<string>>? s_Filter;
    private static Dictionary<ulong, ulong>? s_FilterLogs;

    public static Dictionary<ulong, List<string>> Filter
    {
        get
        {
            if (s_Filter == null)
            {
                s_Filter = new();
            }
            return s_Filter;
        }
        set => s_Filter = value;
    }

    public static Dictionary<ulong, ulong> FilterLogs
    {
        get
        {
            if (s_FilterLogs == null)
            {
                s_FilterLogs = new();
            }
            return s_FilterLogs;
        }
        set => s_FilterLogs = value;
    }
}