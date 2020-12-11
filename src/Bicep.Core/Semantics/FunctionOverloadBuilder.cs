// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public sealed class FunctionOverloadBuilder
    {
        private readonly string name;

        private string? description;

        private TypeSymbol returnType;

        private readonly ImmutableArray<TypeSymbol>.Builder fixedParameterTypes;

        private int minimumArgumentCount;

        private int? maximumArgumentCount;

        private TypeSymbol? variableParameterType;

        private FunctionOverload.ReturnTypeBuilderDelegate returnTypeBuilder;

        private FunctionFlags flags;

        public FunctionOverloadBuilder(string name)
        {
            this.name = name;
            this.returnType = LanguageConstants.Any;
            this.fixedParameterTypes = ImmutableArray.CreateBuilder<TypeSymbol>();
            this.returnTypeBuilder = args => LanguageConstants.Any;
            this.variableParameterType = null;
        }

        public FunctionOverload Build() =>
            new FunctionOverload(
                this.name,
                this.returnTypeBuilder,
                this.returnType,
                this.minimumArgumentCount,
                this.maximumArgumentCount,
                this.fixedParameterTypes.ToImmutable(),
                this.variableParameterType,
                this.flags);

        public FunctionOverloadBuilder WithDescription(string description)
        {
            this.description = description;

            return this;
        }

        public FunctionOverloadBuilder WithReturnType(TypeSymbol returnType)
        {
            this.returnType = returnType;
            this.returnTypeBuilder = args => returnType;

            return this;
        }

        public FunctionOverloadBuilder WithDynamicReturnType(FunctionOverload.ReturnTypeBuilderDelegate returnTypeBuilder)
        {
            this.returnType = returnTypeBuilder(Enumerable.Empty<FunctionArgumentSyntax>());
            this.returnTypeBuilder = returnTypeBuilder;

            return this;
        }

        public FunctionOverloadBuilder WithFixedParameters(params TypeSymbol[] parameterTypes)
        {
            this.fixedParameterTypes.Clear();
            foreach (TypeSymbol parameterType in parameterTypes)
            {
                this.fixedParameterTypes.Add(parameterType);
            }

            this.minimumArgumentCount = parameterTypes.Length;
            this.maximumArgumentCount = parameterTypes.Length;

            return this;
        }

        public FunctionOverloadBuilder WithVariableParameters(int minimumArgumentCount, TypeSymbol parameterType)
        {
            this.fixedParameterTypes.Clear();
            for (int i = 0; i < minimumArgumentCount; i++)
            {
                this.fixedParameterTypes.Add(parameterType);
            }

            this.minimumArgumentCount = minimumArgumentCount;
            this.maximumArgumentCount = null;
            this.variableParameterType = parameterType;

            return this;
        }

        public FunctionOverloadBuilder WithFlags(FunctionFlags flags)
        {
            this.flags = flags;

            return this;
        }
    }
}
