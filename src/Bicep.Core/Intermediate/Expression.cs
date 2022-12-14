// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate;

public record IndexReplacementContext(
    ImmutableDictionary<LocalVariableSymbol, Expression> LocalReplacements,
    Expression Index);

public record ObjectProperty(
    Expression Key,
    Expression Value);

public abstract record Expression()
{
    public abstract void Accept(IExpressionVisitor visitor);
}