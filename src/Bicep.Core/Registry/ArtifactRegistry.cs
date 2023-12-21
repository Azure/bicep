// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceCode;

namespace Bicep.Core.Registry
{
    public abstract class ArtifactRegistry<T> : IArtifactRegistry where T : ArtifactReference
    {
        public abstract string Scheme { get; }

        public RegistryCapabilities GetCapabilities(ArtifactReference reference) => this.GetCapabilities(ConvertReference(reference));

        public abstract bool IsArtifactRestoreRequired(T reference);

        public abstract Task<bool> CheckArtifactExists(T reference);

        public abstract Task PublishModule(T reference, Stream compiled, Stream? bicepSources, string? documentationUri, string? description);

        public abstract Task PublishProvider(T reference, Stream typesTgz);

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<T> references);

        public abstract Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<T> references);

        public abstract ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(T reference);

        public abstract ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? aliasName, string reference);

        public abstract string? TryGetDocumentationUri(T reference);

        public abstract Task<string?> TryGetDescription(T reference);

        public abstract SourceArchiveResult TryGetSource(T reference); //asdfg try?

        public bool IsArtifactRestoreRequired(ArtifactReference reference) => this.IsArtifactRestoreRequired(ConvertReference(reference));

        public Task<bool> CheckArtifactExists(ArtifactReference reference) => this.CheckArtifactExists(ConvertReference(reference));

        public Task PublishModule(ArtifactReference artifactReference, Stream compiled, Stream? bicepSources, string? documentationUri, string? description)
            => this.PublishModule(ConvertReference(artifactReference), compiled, bicepSources, documentationUri, description);

        public Task PublishProvider(ArtifactReference reference, Stream typesTgz)
            => this.PublishProvider(ConvertReference(reference), typesTgz);

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references) =>
            this.RestoreArtifacts(references.Select(ConvertReference));

        public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references) =>
             this.InvalidateArtifactsCache(references.Select(ConvertReference));

        public ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference) =>
            this.TryGetLocalArtifactEntryPointUri(ConvertReference(reference));

        public string? GetDocumentationUri(ArtifactReference reference) => this.TryGetDocumentationUri(ConvertReference(reference));

        public async Task<string?> TryGetDescription(ArtifactReference reference) => await this.TryGetDescription(ConvertReference(reference));

        public SourceArchiveResult TryGetSource(ArtifactReference reference) => this.TryGetSource(ConvertReference(reference));

        public abstract RegistryCapabilities GetCapabilities(T reference);

        private static T ConvertReference(ArtifactReference reference) => reference switch
        {
            T typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}
