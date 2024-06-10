// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public abstract class ArtifactRegistry<T> : IArtifactRegistry where T : ArtifactReference
    {
        public abstract string Scheme { get; }

        public RegistryCapabilities GetCapabilities(ArtifactType artifactType, ArtifactReference reference)
            => this.GetCapabilities(artifactType, ConvertReference(reference));

        public abstract bool IsArtifactRestoreRequired(T reference);

        public abstract Task<bool> CheckArtifactExists(ArtifactType artifactType, T reference);

        public abstract Task PublishModule(T reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description);

        public abstract Task PublishProvider(T reference, ProviderPackage provider);

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<T> references);

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<T> references);

        public abstract ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(T reference);

        public abstract ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? aliasName, string reference);

        public abstract string? TryGetDocumentationUri(T reference);

        public abstract Task<string?> TryGetModuleDescription(ModuleSymbol module, T reference);

        public abstract ResultWithException<SourceArchive> TryGetSource(T reference);

        public bool IsArtifactRestoreRequired(ArtifactReference reference) => this.IsArtifactRestoreRequired(ConvertReference(reference));

        public Task<bool> CheckArtifactExists(ArtifactType artifactType, ArtifactReference reference)
            => this.CheckArtifactExists(artifactType, ConvertReference(reference));

        public Task PublishModule(ArtifactReference artifactReference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => this.PublishModule(ConvertReference(artifactReference), compiled, bicepSources, documentationUri, description);

        public Task PublishProvider(ArtifactReference reference, ProviderPackage provider)
            => this.PublishProvider(ConvertReference(reference), provider);

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references) =>
            this.RestoreArtifacts(references.Select(ConvertReference));

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references) =>
             this.InvalidateArtifactsCache(references.Select(ConvertReference));

        public ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference) =>
            this.TryGetLocalArtifactEntryPointUri(ConvertReference(reference));

        public string? GetDocumentationUri(ArtifactReference reference) => this.TryGetDocumentationUri(ConvertReference(reference));

        public async Task<string?> TryGetModuleDescription(ModuleSymbol module, ArtifactReference reference) =>
            await this.TryGetModuleDescription(module, ConvertReference(reference));

        public ResultWithException<SourceArchive> TryGetSource(ArtifactReference reference) => this.TryGetSource(ConvertReference(reference));

        public abstract Uri? TryGetProviderBinary(T reference);

        public Uri? TryGetProviderBinary(ArtifactReference reference) => this.TryGetProviderBinary(ConvertReference(reference));

        public abstract RegistryCapabilities GetCapabilities(ArtifactType artifactType, T reference);

        private static T ConvertReference(ArtifactReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
