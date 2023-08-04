// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher : IModuleReferenceFactory
    {
        RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference);

        ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        bool TryGetLocalModuleEntryPointUri(ModuleReference moduleReference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        Task<bool> RestoreModules(IEnumerable<ModuleReference> moduleReferences, bool forceModulesRestore = false);

        Task<bool> CheckModuleExists(ModuleReference moduleReference);

        Task PublishModule(ModuleReference moduleReference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri);

        void PruneRestoreStatuses();

        bool TryGetModuleSources(ModuleReference moduleReference, [NotNullWhen(true)] out SourceArchive? sourceArchive);
    }
}
