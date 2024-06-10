// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Providers;

public record RegistryModule(
    string Name, // e.g. "avm/app/dapr-containerapp"
    string? Description,
    string? DocumentationUri);

public record RegistryModuleVersion(
    string Version,
    string? Description,
    string? DocumentationUri);

public interface IPublicRegistryModuleMetadataProvider
{
    Task<IEnumerable<RegistryModule>> GetModules();

    Task<IEnumerable<RegistryModuleVersion>> GetVersions(string modulePath);
}
