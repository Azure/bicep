// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RegistryModuleTool.Proxies
{
    public interface IProcessProxy
    {
        public (int exitCode, string standardOutput, string standardError) Start(string executablePath, string arguments);
    }
}
