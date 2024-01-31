// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Providers;

public record ProviderDescriptor(
    string NamespaceIdentifier,
     string Version,
     Uri ParentModuleUri,
     string? alias = null,
     Uri? TypesDataUri = null,
     string? artifactReference = null)
{
    public string Alias => alias ?? NamespaceIdentifier;
    public string ArtifactReference => artifactReference ?? $"{NamespaceIdentifier}:{Version}";
};
