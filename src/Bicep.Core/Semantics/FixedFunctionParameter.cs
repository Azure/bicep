// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics
{
    public record FixedFunctionParameter(
        string Name,
        string Description,
        TypeSymbol Type,
        bool Required,
        FunctionArgumentTypeCalculator? Calculator)
    {
        public string Signature => this.Required
            ? $"{this.Name}: {this.Type}"
            : $"[{this.Name}: {this.Type}]";
    }
}
