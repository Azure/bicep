// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;

namespace Bicep.Core.TypeSystem.Providers;

public interface IProviderDescriptor
{
    public string NamespaceIdentifier { get; }

    string? Alias { get; }

    string Version { get; }

    public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference();

    public ResultWithDiagnostic<Uri> TryGetTypesDataUri();

    public ResultWithDiagnostic<Uri> TryGetParentModuleUri();
}

public record LegacyProviderDescriptor(string NamespaceIdentifier, string? Alias = null) : IProviderDescriptor
{
    public string Version => "1.0.0"; //the placeholder version for legacy providers
    public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference()
        => throw new NotImplementedException();

    public ResultWithDiagnostic<Uri> TryGetParentModuleUri()
        => throw new NotImplementedException();

    public ResultWithDiagnostic<Uri> TryGetTypesDataUri()
        => throw new NotImplementedException();
}

// public class ConfigManagedProviderDescriptor : IProviderDescriptor
// {
//     public ConfigManagedProviderDescriptor(string NamespaceIdentifier, ProviderConfigEntry configEntry, Uri ParentModuleUri, string? Alias = null)
//     {
//         this.NamespaceIdentifier = NamespaceIdentifier;
//         this.Alias = Alias ?? NamespaceIdentifier;
//         this.ParentModuleUri = ParentModuleUri;
//     }

//     public string NamespaceIdentifier { get; }

//     public string Version { get; }

//     public string Alias { get; }

//     public ArtifactReference ArtifactReference { get; }

//     public Uri ParentModuleUri { get; }

//     public Uri TypesDataUri { get; }

//     public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference()
//         => new(this.ArtifactReference);

//     public ResultWithDiagnostic<Uri> TryGetParentModuleUri()
//         => new(this.ParentModuleUri);

//     public ResultWithDiagnostic<Uri> TryGetTypesDataUri()
//         => new(this.TypesDataUri);
// }
