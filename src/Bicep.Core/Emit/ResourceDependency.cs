// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit;

public record ResourceDependency(
    DeclaredSymbol Resource,
    SyntaxBase? IndexExpression,
    // A reference is considered "weak" if it would not automatically create an implicit dependency in the ARM engine
    bool WeakReference = false);
