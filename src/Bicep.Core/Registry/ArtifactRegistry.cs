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
    public abstract class ArtifactRegistry<T> : IArtifactRegistry where T : ExternalArtifactReference
    {
        public abstract string Scheme { get; }

        public bool CanHandle(ExternalArtifactReference reference) => reference is T;

        public RegistryCapabilities GetCapabilities(ArtifactType artifactType, ExternalArtifactReference reference)
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

        public abstract string? TryGetDocumentationUri(T reference);

        public abstract Task<string?> TryGetModuleDescription(ModuleSymbol module, T reference);

        public bool IsArtifactRestoreRequired(ExternalArtifactReference reference) => this.IsArtifactRestoreRequired(ConvertReference(reference));

        public Task<bool> CheckArtifactExists(ArtifactType artifactType, ExternalArtifactReference reference)
            => this.CheckArtifactExists(artifactType, ConvertReference(reference));

        public Task PublishModule(ExternalArtifactReference artifactReference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => this.PublishModule(ConvertReference(artifactReference), compiled, bicepSources, documentationUri, description);

        public Task PublishExtension(ExternalArtifactReference reference, ExtensionPackage package)
            => this.PublishExtension(ConvertReference(reference), package);

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<ExternalArtifactReference> references) =>
            this.RestoreArtifacts(references.Select(ConvertReference));

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ExternalArtifactReference> references) =>
             this.InvalidateArtifactsCache(references.Select(ConvertReference));

        public string? GetDocumentationUri(ExternalArtifactReference reference) => this.TryGetDocumentationUri(ConvertReference(reference));

        public async Task<string?> TryGetModuleDescription(ModuleSymbol module, ExternalArtifactReference reference) =>
            await this.TryGetModuleDescription(module, ConvertReference(reference));

        public abstract RegistryCapabilities GetCapabilities(ArtifactType artifactType, T reference);

        private static T ConvertReference(ExternalArtifactReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
