// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
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

        public abstract Task PublishExtension(T reference, ExtensionPackage package);

        public virtual Task OnRestoreArtifacts(bool forceRestore)
        {
            return Task.CompletedTask;
        }

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<T> references);

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<T> references);

        public abstract ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference);

        public abstract string? TryGetDocumentationUri(T reference);

        public abstract Task<string?> TryGetModuleDescription(ModuleSymbol module, T reference);

        public bool IsArtifactRestoreRequired(ArtifactReference reference) => this.IsArtifactRestoreRequired(ConvertReference(reference));

        public Task<bool> CheckArtifactExists(ArtifactType artifactType, ArtifactReference reference)
            => this.CheckArtifactExists(artifactType, ConvertReference(reference));

        public Task PublishModule(ArtifactReference artifactReference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => this.PublishModule(ConvertReference(artifactReference), compiled, bicepSources, documentationUri, description);

        public Task PublishExtension(ArtifactReference reference, ExtensionPackage package)
            => this.PublishExtension(ConvertReference(reference), package);

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references) =>
            this.RestoreArtifacts(references.Select(ConvertReference));

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references) =>
             this.InvalidateArtifactsCache(references.Select(ConvertReference));

        public string? GetDocumentationUri(ArtifactReference reference) => this.TryGetDocumentationUri(ConvertReference(reference));

        public async Task<string?> TryGetModuleDescription(ModuleSymbol module, ArtifactReference reference) =>
            await this.TryGetModuleDescription(module, ConvertReference(reference));

        public abstract RegistryCapabilities GetCapabilities(ArtifactType artifactType, T reference);

        private static T ConvertReference(ArtifactReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
