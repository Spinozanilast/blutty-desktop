namespace PlatformsPipes.Helpers;

public enum HyprlandWinDefiner
{
    Class,
    InitialClass,
    Title,
    InitialTitle,
    Tag,
    Pid,
    StableId,
    Address
}

public static class HyprlandWinDefinitionProducer
{
    public static string CreateWinDefinition(HyprlandWinDefiner definer, string regexOrSelector)
    {
        var normalizedDefiner = definer.ToString().ToLower();
        return $"{normalizedDefiner}:{regexOrSelector}";
    }
}