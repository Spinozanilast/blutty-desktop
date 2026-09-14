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


public static class Hyprland
{
    extension(OperatingSystem)
    {
        public static bool UsesHyprlandComposer() =>
            OperatingSystem.IsLinux() &&
            "XDG_CURRENT_DESKTOP".TryGetEnvVarValue(out _);
    }
    
    public static string CreateWinDefinition(HyprlandWinDefiner definer, string regexOrSelector)
    {
        var normalizedDefiner = definer.ToString().ToLower();
        return $"{normalizedDefiner}:{regexOrSelector}";
    }
}