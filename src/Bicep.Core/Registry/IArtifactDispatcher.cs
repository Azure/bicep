// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceLink;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public record ExtensionBinary(
        SupportedArchitecture Architecture,
        BinaryData Data);

    public record ExtensionPackage(
        BinaryData Types,
        bool LocalDeployEnabled,
        ImmutableArray<ExtensionBinary> Binaries);

    public interface IModuleDispatcher : IArtifactReferenceFactory
    {
        RegistryCapabilities GetRegistryCapabilities(ArtifactType artifactType, ArtifactReference reference);

        ArtifactRestoreStatus GetArtifactRestoreStatus(ArtifactReference reference, out DiagnosticBuilder.DiagnosticBuilderDelegate? errorDetailBuilder);

        ResultWithDiagnosticBuilder<IFileHandle> TryGetLocalArtifactEntryPointFileHandle(ArtifactReference reference);

        Task<bool> RestoreArtifacts(IEnumerable<ArtifactReference> references, bool forceRestore);

        Task<bool> CheckModuleExists(ArtifactReference reference);

        Task<bool> CheckExtensionExists(ArtifactReference reference);

        Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri);

        Task PublishExtension(ArtifactReference reference, ExtensionPackage package);

        void PruneRestoreStatuses();
    }
}
