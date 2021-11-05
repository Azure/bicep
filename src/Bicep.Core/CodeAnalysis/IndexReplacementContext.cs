// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.CodeAnalysis
{
    // TODO: refactor this to remove dependency on syntax
    public record IndexReplacementContext(
        ImmutableDictionary<LocalVariableSymbol, Operation> LocalReplacements,
        Operation Index);
}
