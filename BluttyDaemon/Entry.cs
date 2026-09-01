using System.IO.Pipes;
using BluttyRpc;

namespace BluttyDaemon;

public static class Entry
{
    public static async Task Main(string[] args)
    {
        var dbusListener = await BluezDBusListener.PrepareDefaultAsync();

        await using var _readyClientSteam = new NamedPipeClientStream(serverName: ".", pipeName: RpcConfig.RpcPipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await _readyClientSteam.ConnectAsync();

        var deviceEventsProcessor = new DeviceEventsProcessor(_readyClientSteam);
        var bluttyDaemon = new BluttyDaemon(dbusListener, deviceEventsProcessor);
        await bluttyDaemon.StartDaemonAsync();
    }
}