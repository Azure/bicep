// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Registry
{
    public static class ModuleDispatcherExtensions
    {
        public static IEnumerable<ModuleReference> GetValidModuleReferences(this IModuleDispatcher moduleDispatcher, IEnumerable<(ISourceFile parentModule, ModuleDeclarationSyntax declarationSyntax)> modules) =>
            modules
                .Select(t => moduleDispatcher.TryGetModuleReference(t.declarationSyntax, t.parentModule.FileUri, out var moduleRef, out _) ? moduleRef : null)
                .WhereNotNull();
    }
}
