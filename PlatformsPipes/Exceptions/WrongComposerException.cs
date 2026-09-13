namespace PlatformsPipes.Exceptions;

public class WrongComposerException : Exception
{
    public WrongComposerException(string composerName) : base($"Your system doesn't use {composerName} composer")
    {
    }
}