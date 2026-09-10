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
    public class OciArtifactMockedRegistry : ArtifactRegistry<OciArtifactMockedReference>
    {
        public override string Scheme => ArtifactReferenceSchemes.OciMocked;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, OciArtifactMockedReference reference)
            => RegistryCapabilities.Default;

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference)
           => new(x => x.ModuleReferenceSchemeBrFsNotSupported());

        public override bool IsArtifactRestoreRequired(OciArtifactMockedReference reference) => false;

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, OciArtifactMockedReference reference)
            => Task.FromResult(reference.TryGetEntryPointFileHandle().IsSuccess(out var fileHandle, out _) && fileHandle.Exists());

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactMockedReference> references)
            => Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>>(
                new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>());

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<OciArtifactMockedReference> references)
            => Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>>(
                new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>());

        public override Task PublishModule(OciArtifactMockedReference reference, BinaryData compiled, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Publishing is not supported for mocked module aliases.");

        public override Task PublishExtension(OciArtifactMockedReference reference, ExtensionPackage package)
            => throw new NotSupportedException("Publishing is not supported for mocked module aliases.");

        public override string? TryGetDocumentationUri(OciArtifactMockedReference reference) => null;

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, OciArtifactMockedReference reference)
            => Task.FromResult<string?>(null);
    }
}
