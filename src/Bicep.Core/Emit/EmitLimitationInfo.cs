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
        public IReadOnlyList<Diagnostic> Diagnostics { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData { get; }

        public EmitLimitationInfo(IReadOnlyList<Diagnostic> diagnostics, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            Diagnostics = diagnostics;
            ModuleScopeData = moduleScopeData;
        }
    }
}
