// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Registry
{
    public static class ModuleDispatcherExtensions
    {
        public static IEnumerable<ModuleReference> GetValidModuleReferences(this IModuleDispatcher moduleDispatcher, IEnumerable<ModuleDeclarationSyntax> modules, RootConfiguration configuration) =>
            modules
                .Select(module => moduleDispatcher.TryGetModuleReference(module, configuration, out _))
                .WhereNotNull();
    }
}
