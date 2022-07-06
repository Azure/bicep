// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO;

namespace Bicep.VSLanguageServerClient.ProcessLauncher
{
    /// <summary>
    /// Represents the started process
    /// </summary>
    public interface IClientProcess
    {
        Process Process { get; }
        StreamReader StandardOutput { get; }
        StreamReader StandardError { get; }
        StreamWriter StandardInput { get; }
    }
}
