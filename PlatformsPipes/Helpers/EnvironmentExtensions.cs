namespace PlatformsPipes.Helpers;

public static class EnvironmentExtensions
{
    public static bool TryGetEnvVarValue(this string varName, out string varValue)
    {
        varValue = Environment.GetEnvironmentVariable(varName);

        if (varValue is not null) return true;

        varValue = null;
        return false;
    }
}