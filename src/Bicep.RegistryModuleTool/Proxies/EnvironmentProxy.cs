// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.RegistryModuleTool.Proxies
{
    public class EnvironmentProxy : IEnvironmentProxy
    {
        public string? GetEnvironmentVariable(string variableName) => Environment.GetEnvironmentVariable(variableName);

        public string GetHomeDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
}
