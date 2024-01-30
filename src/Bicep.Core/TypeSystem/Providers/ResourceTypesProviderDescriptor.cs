// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Providers;

public record ProviderDescriptor
{
    public ProviderDescriptor(
        string namespaceIdentifier,
        string? version,
        bool isImplicitImport,
        string? alias = null,
        string? registryAddress = null,
        Uri? typesDataUri = null,
        Uri? parentModuleUri = null)
    {
        NamespaceIdentifier = namespaceIdentifier;
        Version = version ?? "TODO(asilverman): Get the version from the config file somehow";
        Alias = alias ?? namespaceIdentifier;
        ArtifactReference = registryAddress ?? (isImplicitImport ? namespaceIdentifier : $"{namespaceIdentifier}@{version}");
        TypesDataUri = typesDataUri;
        this.parentModuleUri = parentModuleUri;
    }

    private Uri? parentModuleUri;

    public string NamespaceIdentifier { get; }

    public string Alias { get; }

    public Uri? TypesDataUri { get; }

    public string Version { get; }

    public string ArtifactReference { get; }
}
