// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Providers
{
    public record PublicRegistryModule(string Name, string? Description, string? DocumentationUri);
    public record PublicRegistryModuleVersion(string Version, string? Description, string? DocumentationUri);

    public interface IPublicRegistryModuleMetadataProvider
    {
        Task<IEnumerable<PublicRegistryModule>> GetModules();

        Task<IEnumerable<PublicRegistryModuleVersion>> GetVersions(string modulePath);
    }
}
