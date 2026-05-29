// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;

namespace Bicep.Core.Registry
{
    public class OciArtifactEmulatedRegistry : ArtifactRegistry<OciArtifactEmulatedReference>
    {
        public override string Scheme => ArtifactReferenceSchemes.OciEmulated;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, OciArtifactEmulatedReference reference)
            => RegistryCapabilities.Default;

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference)
           => new(x => x.ModuleReferenceSchemeBrFsNotSupported());

        public override bool IsArtifactRestoreRequired(OciArtifactEmulatedReference reference) => false;

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, OciArtifactEmulatedReference reference)
            => Task.FromResult(reference.TryGetEntryPointFileHandle().IsSuccess(out var fileHandle, out _) && fileHandle.Exists());

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactEmulatedReference> references)
            => Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>>(
                new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>());

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<OciArtifactEmulatedReference> references)
            => Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>>(
                new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>());

        public override Task PublishModule(OciArtifactEmulatedReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Publishing is not supported for filesystem-based module aliases.");

        public override Task PublishExtension(OciArtifactEmulatedReference reference, ExtensionPackage package)
            => throw new NotSupportedException("Publishing is not supported for filesystem-based module aliases.");

        public override string? TryGetDocumentationUri(OciArtifactEmulatedReference reference) => null;

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, OciArtifactEmulatedReference reference)
            => Task.FromResult<string?>(null);
    }
}
