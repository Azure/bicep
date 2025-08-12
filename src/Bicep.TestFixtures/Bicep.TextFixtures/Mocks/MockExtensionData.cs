// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;

namespace Bicep.TextFixtures.Mocks
{
    public record MockExtensionData(
        string Name,
        string Version,
        string RepoVersion,
        BinaryData TypesTgzData)
    {
        public string RegistryScheme => "br";

        public string Registry => LanguageConstants.BicepPublicMcrRegistry;

        public string RepoPath => $"bicep/extensions/{Name}/{RepoVersion}";

        public string ExtensionRepoReference => $"{RegistryScheme}:{Registry}/{RepoPath}:{Version}";

        public IEnumerable<string> Tags { get; } = ["tag"];
    }
}
