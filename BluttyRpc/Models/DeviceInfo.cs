namespace BluttyRpc;

/// <summary>
/// Lightweight, serializable device information passed over RPC.
/// The underlying BlueZ <c>DeviceProperties</c> cannot be serialized because its
/// getters throw for properties the adapter did not report.
/// </summary>
public sealed record DeviceInfo(string Address, string Name, string Alias, bool Paired, string Icon);