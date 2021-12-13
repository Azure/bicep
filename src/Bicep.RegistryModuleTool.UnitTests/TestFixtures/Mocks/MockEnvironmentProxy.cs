// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Proxies;
using System.Collections.Generic;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks
{
    internal class MockEnvironmentProxy : IEnvironmentProxy
    {
        private readonly IReadOnlyDictionary<string, string> environmentVariablesByName;

        public MockEnvironmentProxy(IReadOnlyDictionary<string, string>? environmentVariablesByName = null)
        {
            this.environmentVariablesByName = environmentVariablesByName ?? new Dictionary<string, string>();
        }

        public string? GetEnvironmentVariable(string variableName) =>
            this.environmentVariablesByName.TryGetValue(variableName, out var variableValue) ? variableValue : null;

        public string GetHomeDirectory() => "/home";
    }
}
