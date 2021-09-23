// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        ModuleReference? TryGetModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        ModuleReference? TryGetModuleReference(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference);

        ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        Task<bool> RestoreModules(IEnumerable<ModuleReference> moduleReferences);

        Task PublishModule(ModuleReference moduleReference, Stream compiled);

        void PruneRestoreStatuses();
    }
}
