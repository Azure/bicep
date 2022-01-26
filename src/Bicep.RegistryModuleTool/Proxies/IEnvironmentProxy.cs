// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RegistryModuleTool.Proxies
{
    public interface IEnvironmentProxy
    {
        public string? GetEnvironmentVariable(string variableName);

        public string GetHomeDirectory();
    }
}
