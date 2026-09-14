using PlatformsPipes.Exceptions;
using PlatformsPipes.Helpers;
using System.Net.Sockets;
using System.Text;

namespace PlatformsPipes.Linux;

/// <summary>
/// Hyprland exposes 2 UNIX Sockets, for controlling / getting info about Hyprland via code / bash utilities.
/// Watch more at <a href="https://wiki.hypr.land/IPC/">Hyprland docs IPC page</a>
/// </summary>
public class HyprlandSocketClient
{
    public const string ComposerName = "Hyprland";
    public const string XdgRuntimeDirVarName = "XDG_RUNTIME_DIR";
    public static readonly string HyprlandInstanceSignatureVarName = "HYPRLAND_INSTANCE_SIGNATURE";

    public string HyprlandInstanceSignature { get; private set; } = string.Empty;
    public string XdgRuntimeDirectory { get; private set; } = string.Empty;

    private readonly UnixDomainSocketEndPoint _commandsSocketEndpoint;

    public HyprlandSocketClient()
    {
        if (!CheckEnvironmentVarsExists(out _))
        {
            throw new WrongComposerException(ComposerName);
        }

        var commandsSocketPath = Path.Combine(XdgRuntimeDirectory, "hypr", HyprlandInstanceSignature, ".socket.sock");

        if (!File.Exists(commandsSocketPath))
        {
            throw new FileNotFoundException("Hyprland socker descriptor was not found.", commandsSocketPath);
        }

        _commandsSocketEndpoint =
            new UnixDomainSocketEndPoint(commandsSocketPath);
    }

    public void DispatchCommand(string luaCommand)
    {
        SendCommand("dispatch", luaCommand);
    }

    public void SendCommand(string command, string arg)
    {
        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        socket.Connect(_commandsSocketEndpoint);
        socket.Send(Encoding.UTF8.GetBytes(command + " " + arg));
        Console.WriteLine($"{command} {arg} sent");

        var buffer = new byte[512];
        var bytesAnswer = socket.Receive(buffer);
        var response = Encoding.UTF8.GetString(buffer, 0, bytesAnswer);
        Console.WriteLine($"Response: {response}");
    }

    private bool CheckEnvironmentVarsExists(out string? missingVarName)
    {
        missingVarName = null;
        if (XdgRuntimeDirVarName.TryGetEnvVarValue(out var xdgRuntimeDir))
        {
            XdgRuntimeDirectory = xdgRuntimeDir;
        }
        else
        {
            missingVarName = XdgRuntimeDirVarName;
            return false;
        }

        if (HyprlandInstanceSignatureVarName.TryGetEnvVarValue(out var his))
        {
            HyprlandInstanceSignature = his;
        }
        else
        {
            missingVarName = HyprlandInstanceSignatureVarName;
            return false;
        }

        return true;
    }
}