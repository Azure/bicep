// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public record ProviderBinary(
        SupportedArchitecture Architecture,
        BinaryData Data);

    public record ProviderPackage(
        BinaryData Types,
        bool LocalDeployEnabled,
        ImmutableArray<ProviderBinary> Binaries);

    public interface IModuleDispatcher : IArtifactReferenceFactory
    {
        RegistryCapabilities GetRegistryCapabilities(ArtifactType artifactType, ArtifactReference reference);

        ArtifactRestoreStatus GetArtifactRestoreStatus(ArtifactReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference);

        Task<bool> RestoreArtifacts(IEnumerable<ArtifactReference> references, bool forceRestore);

        Task<bool> CheckModuleExists(ArtifactReference reference);

        Task<bool> CheckProviderExists(ArtifactReference reference);

        Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri);

        Task PublishProvider(ArtifactReference reference, ProviderPackage provider);

        void PruneRestoreStatuses();

        // Retrieves the sources that have been restored along with the module into the cache (if available)
        ResultWithException<SourceArchive> TryGetModuleSources(ArtifactReference reference);

        Uri? TryGetProviderBinary(ArtifactReference reference);
    }
}
