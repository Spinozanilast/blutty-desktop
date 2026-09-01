using Microsoft.Extensions.Logging;

namespace BluttyDaemon;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Daemon is starting on host '{HostName}'")]
    public static partial void LogDaemonStarting(
        this ILogger logger, string hostName);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Daemon started on host '{HostName}'")]
    public static partial void LogDaemonStarted(
        this ILogger logger, string hostName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Debug,
        Message = "Daemon stopping")]
    public static partial void LogDaemonStopping(
        this ILogger logger);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Debug,
        Message = "Daemon stopped")]
    public static partial void LogDaemonStopped(
        this ILogger logger);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Information,
        Message = "Discovery started on adapter '{AdapterAddress}' ({AdapterName})")]
    public static partial void LogDiscoveryStarted(
        this ILogger logger, string adapterAddress, string adapterName);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Information,
        Message = "Discovery stopped on adapter '{AdapterAddress}' ({AdapterName})")]
    public static partial void LogDiscoveryStopped(
        this ILogger logger, string adapterAddress, string adapterName);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Debug,
        Message = "Device found '{DeviceAddress}' ({DeviceName})")]
    public static partial void LogDeviceFound(
        this ILogger logger, string deviceAddress, string deviceName);

    [LoggerMessage(
        EventId = 7,
        Level = LogLevel.Information,
        Message = "Device connected '{DeviceAddress}' ({DeviceName})")]
    public static partial void LogDeviceConnected(
        this ILogger logger, string deviceAddress, string deviceName);

    [LoggerMessage(
        EventId = 8,
        Level = LogLevel.Information,
        Message = "Device disconnected '{DeviceAddress}' ({DeviceName})")]
    public static partial void LogDeviceDisconnected(
        this ILogger logger, string deviceAddress, string deviceName);

    [LoggerMessage(
        EventId = 9,
        Level = LogLevel.Error,
        Message = "No accessible Bluetooth adapters found")]
    public static partial void LogNoAdapterAccessible(
        this ILogger logger);

    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Error,
        Message = "Failed to notify host about connection state for device")]
    public static partial void LogDeviceNotifyFailed(
        this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 11,
        Level = LogLevel.Error,
        Message = "Failed to subscribe to device events")]
    public static partial void LogDeviceSubscribeFailed(
        this ILogger logger, Exception exception);
}
