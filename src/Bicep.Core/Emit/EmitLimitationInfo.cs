// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class EmitLimitationInfo
    {
        public IReadOnlyList<IDiagnostic> Diagnostics { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData { get; }

        public ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> ResourceScopeData { get; }

        public EmitLimitationInfo(IReadOnlyList<IDiagnostic> diagnostics, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData, ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> resourceScopeData)
        {
            Diagnostics = diagnostics;
            ModuleScopeData = moduleScopeData;
            ResourceScopeData = resourceScopeData;
        }
    }
}
