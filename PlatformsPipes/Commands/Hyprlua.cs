namespace PlatformsPipes.Commands;

public static class Hyprlua
{
    public record struct MoveToPxCommand(int x, int y, bool relative, string windowDefinition) : ICommand
    {
        public string GetString()
        {
            return $"hl.dsp.window.move({{x={x},y={y},relative={relative},window=\"{windowDefinition}\"}})";
        }
    }
}