using PlatformsPipes.Commands;
using PlatformsPipes.Linux;

namespace Blutty.Services;

public class HyprlandCommandService
{
    private HyprlandSocketClient? _socketClient;

    private HyprlandSocketClient SocketClient => _socketClient ??= new HyprlandSocketClient();

    public void Dispatch(ICommand command)
    {
        SocketClient.DispatchCommand(command.GetString());
    }
}
