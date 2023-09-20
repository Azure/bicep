// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceCode;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher : IModuleReferenceFactory
    {
        RegistryCapabilities GetRegistryCapabilities(ArtifactReference moduleReference);

        ArtifactRestoreStatus GetArtifactRestoreStatus(ArtifactReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        ResultWithDiagnostic<Uri> TryGetLocalModuleEntryPointUri(ArtifactReference moduleReference);

        Task<bool> RestoreModules(IEnumerable<ArtifactReference> moduleReferences, bool forceModulesRestore = false);

        Task<bool> CheckModuleExists(ArtifactReference moduleReference);

        Task PublishModule(ArtifactReference moduleReference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri);

        void PruneRestoreStatuses();

        SourceArchive? TryGetModuleSources(ArtifactReference moduleReference);
    }
}
