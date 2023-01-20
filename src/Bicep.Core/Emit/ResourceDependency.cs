// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit;

public record ResourceDependency(
    DeclaredSymbol Resource,
    SyntaxBase? IndexExpression,
    ResourceDependencyKind Kind);