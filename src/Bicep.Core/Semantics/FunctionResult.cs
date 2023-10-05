// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Intermediate;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public record FunctionResult(
    TypeSymbol Type,
    Expression? Value = null);
