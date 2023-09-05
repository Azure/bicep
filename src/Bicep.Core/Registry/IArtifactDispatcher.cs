// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
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

        bool TryGetLocalModuleEntryPointUri(ArtifactReference moduleReference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        Task<bool> RestoreModules(IEnumerable<ArtifactReference> moduleReferences, bool forceModulesRestore = false);

        Task<bool> CheckModuleExists(ArtifactReference moduleReference);

        Task PublishModule(ArtifactReference moduleReference, Stream compiled, string? documentationUri);

        void PruneRestoreStatuses();
    }
}
