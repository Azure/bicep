// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Proxies;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks
{
    public class MockEnvironmentProxy : IEnvironmentProxy
    {
        private readonly IReadOnlyDictionary<string, string> environmentVariablesByName;

        public MockEnvironmentProxy(IReadOnlyDictionary<string, string>? environmentVariablesByName = null)
        {
            this.environmentVariablesByName = environmentVariablesByName ?? new Dictionary<string, string>();
        }

        public static MockEnvironmentProxy Default { get; } = new MockEnvironmentProxy(new Dictionary<string, string>
        {
            ["PATH"] = "/bin",
        });

        public string? GetEnvironmentVariable(string variableName) =>
            this.environmentVariablesByName.TryGetValue(variableName, out var variableValue) ? variableValue : null;

        public string GetHomeDirectory() => "/home";
    }
}
