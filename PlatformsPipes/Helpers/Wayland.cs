namespace PlatformsPipes.Helpers;

public static class Wayland
{
    public static readonly string WaylandDisplayVarName = "WAYLAND_DISPLAY";

    extension(OperatingSystem)
    {
        public static bool UsesWayland() =>
            OperatingSystem.IsLinux() && WaylandDisplayVarName.TryGetEnvVarValue(out _);
    }
}
