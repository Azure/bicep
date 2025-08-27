// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public record VariableFunctionParameter(
    string NamePrefix,
    string Description,
    TypeSymbol Type,
    int MinimumCount,
    FunctionParameterFlags Flags)
{
    public string GetNamedSignature(int index) => $"{this.NamePrefix}{index} : {this.Type}";

    public string GenericSignature => $"... : {this.Type}";
}
