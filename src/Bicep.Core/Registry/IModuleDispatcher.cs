// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Registry
{
    public interface IModuleDispatcher
    {
        ImmutableArray<string> AvailableSchemes { get; }

        bool ValidateModuleReference(ModuleDeclarationSyntax module, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        bool IsModuleAvailable(ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? errorDetailBuilder);

        Uri? TryGetLocalModuleEntryPointUri(Uri parentModuleUri, ModuleDeclarationSyntax module, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        bool RestoreModules(IEnumerable<ModuleDeclarationSyntax> modules);
    }
}
