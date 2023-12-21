// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceCode;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher : IArtifactReferenceFactory
    {
        RegistryCapabilities GetRegistryCapabilities(ArtifactReference reference);

        ArtifactRestoreStatus GetArtifactRestoreStatus(ArtifactReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference);

        Task<bool> RestoreModules(IEnumerable<ArtifactReference> references, bool forceRestore = false);

        Task<bool> CheckModuleExists(ArtifactReference reference);

        Task<bool> CheckProviderExists(ArtifactReference reference);

        Task PublishModule(ArtifactReference reference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri);

        Task PublishProvider(ArtifactReference reference, Stream compiledArmTemplate);

        void PruneRestoreStatuses();

        // Retrieves the sources that have been restored along with the module into the cache (if available)
        SourceArchiveResult TryGetModuleSources(ArtifactReference reference);
    }
}
