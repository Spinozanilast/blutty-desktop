using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Bluezat.DBus;
using BluttyRpc;
using StreamJsonRpc;

namespace Blutty.Services;

public class DeviceConnectorService : IDeviceConnector
{
    private readonly Action<bool, DeviceProperties> _onDeviceEvent;
    private NamedPipeServerStream? _pipeServer;
    private JsonRpc? _rpc;
    private CancellationTokenSource? _cts;

    public DeviceConnectorService(Action<bool, DeviceProperties> onDeviceEvent)
    {
        _onDeviceEvent = onDeviceEvent;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                _pipeServer = new NamedPipeServerStream(
                    RpcConfig.RpcPipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                await _pipeServer.WaitForConnectionAsync(_cts.Token);

                _rpc = JsonRpc.Attach(_pipeServer, this);
                await _rpc.Completion;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
            }
            finally
            {
                _rpc?.Dispose();
                _pipeServer?.Dispose();
            }
        }
    }

    public void SendConnectedDeviceInfo(bool isConnected, DeviceProperties info)
    {
        Dispatcher.UIThread.Post(() => _onDeviceEvent.Invoke(isConnected, info));
    }

    public void ConnectToDevice(string address)
    {
    }

    public void DisconnectFromDevice(string address)
    {
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _rpc?.Dispose();
        _pipeServer?.Dispose();
    }
}