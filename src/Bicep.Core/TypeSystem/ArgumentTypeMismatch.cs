// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class ArgumentTypeMismatch(FunctionOverload source, int argumentIndex, TypeSymbol argumentType, TypeSymbol parameterType)
    {
        public FunctionOverload Source { get; } = source;

        public int ArgumentIndex { get; } = argumentIndex;

        public TypeSymbol ArgumentType { get; } = argumentType;

        public TypeSymbol ParameterType { get; } = parameterType;

        public void Deconstruct(out FunctionOverload source, out int argumentIndex, out TypeSymbol argumentType, out TypeSymbol parameterType)
        {
            source = this.Source;
            argumentIndex = this.ArgumentIndex;
            argumentType = this.ArgumentType;
            parameterType = this.ParameterType;
        }
    }
}

