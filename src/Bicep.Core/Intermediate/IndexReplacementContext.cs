// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate
{
    public record IndexReplacementContext(
        ImmutableDictionary<LocalVariableSymbol, Operation> LocalReplacements,
        Operation Index);
}
