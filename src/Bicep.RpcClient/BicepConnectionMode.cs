// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RpcClient;

/// <summary>
/// Specifies how the Bicep RPC client communicates with the Bicep CLI process.
/// </summary>
public enum BicepConnectionMode
{
    /// <summary>
    /// Communicates via a named pipe. This is the default.
    /// </summary>
    NamedPipe,

    /// <summary>
    /// Communicates via the Bicep CLI's standard input/output streams.
    /// This can be useful in environments where named pipes are unavailable or restricted.
    /// </summary>
    Stdio,
}
