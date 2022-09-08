// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher
    {
        ImmutableArray<string> AvailableSchemes(Uri parentModuleUri);

        bool TryGetModuleReference(string reference, Uri parentModuleUri, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        bool TryGetModuleReference(ModuleDeclarationSyntax module, Uri parentModuleUri, [NotNullWhen(true)] out ModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        RegistryCapabilities GetRegistryCapabilities(ModuleReference moduleReference);

        ModuleRestoreStatus GetModuleRestoreStatus(ModuleReference moduleReference, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        bool TryGetLocalModuleEntryPointUri(ModuleReference moduleReference, [NotNullWhen(true)] out Uri? localUri, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        Task<bool> RestoreModules(IEnumerable<ModuleReference> moduleReferences, bool forceModulesRestore = false);

        Task PublishModule(ModuleReference moduleReference, Stream compiled);

        void PruneRestoreStatuses();
    }
}
