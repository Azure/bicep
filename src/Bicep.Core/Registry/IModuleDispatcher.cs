// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher
    {
        ImmutableArray<string> AvailableSchemes { get; }

        ModuleReference? TryGetModuleReference(string reference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference);

        ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference moduleReference, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        Task<bool> RestoreModules(RootConfiguration configuration, IEnumerable<ModuleReference> moduleReferences);

        Task PublishModule(RootConfiguration configuration, ModuleReference moduleReference, Stream compiled);

        void PruneRestoreStatuses();
    }
}
