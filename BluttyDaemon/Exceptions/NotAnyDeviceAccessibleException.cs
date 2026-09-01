namespace BluttyDaemon.Exceptions;

public class NotAnyDeviceAccessibleException: Exception
{
    public override string Message => "Not any device accessible for now, check for adapters for connection";
}