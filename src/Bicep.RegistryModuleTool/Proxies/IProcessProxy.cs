// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RegistryModuleTool.Proxies
{
    internal interface IProcessProxy
    {
        (int exitCode, string standardOutput, string standardError) Start(string executablePath, string arguments);
    }
}
