using System.IO.Pipes;
using BluttyRpc;
using Microsoft.Extensions.Logging;
using Tmds.DBus.Protocol;

namespace BluttyDaemon;

internal partial class Entry
{
    public static async Task Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder
            .SetMinimumLevel(LogLevel.Debug)
            .AddConsole());
        var logger = loggerFactory.CreateLogger<BluttyDaemon>();

        var dbusConnection = DBusConnection.System;
        await dbusConnection.ConnectAsync();

        var dbusListener =
            await BluezDBusListener.PrepareDefaultAsync(dbusConnection,
                loggerFactory.CreateLogger<BluezDBusListener>());

        await using var _readyClientSteam = new NamedPipeClientStream(serverName: ".", pipeName: RpcConfig.RpcPipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await _readyClientSteam.ConnectAsync();

        var deviceEventsProcessor =
            new DeviceEventsProcessor(_readyClientSteam, loggerFactory.CreateLogger<DeviceEventsProcessor>());
        try
        {
            var bluttyDaemon = new BluttyDaemon(dbusListener, deviceEventsProcessor, logger);
            await bluttyDaemon.StartDaemonAsync();
        }
        finally
        {
            deviceEventsProcessor.Dispose();
        }
    }
}