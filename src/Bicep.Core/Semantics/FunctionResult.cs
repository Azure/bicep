// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class FunctionResult
{
    public TypeSymbol Type { get; }
    public object? Value { get; }

    public FunctionResult(TypeSymbol type, object? value = null)
    {
        Type = type;
        Value = value;
    }

}
