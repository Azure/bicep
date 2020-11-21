// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class ArgumentTypeMismatch
    {
        public ArgumentTypeMismatch(FunctionOverload source, int argumentIndex, TypeSymbol argumentType, TypeSymbol parameterType)
        {
            this.Source = source;
            this.ArgumentIndex = argumentIndex;
            this.ArgumentType = argumentType;
            this.ParameterType = parameterType;
        }

        public FunctionOverload Source { get; }

        public int ArgumentIndex { get; }

        public TypeSymbol ArgumentType { get; }

        public TypeSymbol ParameterType { get; }

        public void Deconstruct(out FunctionOverload source, out int argumentIndex,  out TypeSymbol argumentType, out TypeSymbol parameterType)
        {
            source = this.Source;
            argumentIndex = this.ArgumentIndex;
            argumentType = this.ArgumentType;
            parameterType = this.ParameterType;
        }
    }
}

