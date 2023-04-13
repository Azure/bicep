// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public record EmitLimitationInfo(
    IReadOnlyList<IDiagnostic> Diagnostics,
    ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData,
    ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> ResourceScopeData,
    ImmutableDictionary<ParameterAssignmentSymbol, JToken> ParameterAssignments);