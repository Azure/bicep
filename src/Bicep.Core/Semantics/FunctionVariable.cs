// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Intermediate;

namespace Bicep.Core.Semantics;

public record FunctionVariable(
    string Name,
    Expression Value);
