// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionWildcardOverload : FunctionOverload
    {
        public FunctionWildcardOverload(
            string name,
            string genericDescription,
            string description,
            Regex wildcardRegex,
            ReturnTypeBuilderDelegate returnTypeBuilder,
            TypeSymbol returnType,
            IEnumerable<FixedFunctionParameter> fixedArgumentTypes,
            VariableFunctionParameter? variableArgumentType,
            EvaluatorDelegate? expressionEmitterDelegate,
            FunctionFlags flags = FunctionFlags.Default)
            : base(name, genericDescription, description, returnTypeBuilder, returnType, fixedArgumentTypes, variableArgumentType, expressionEmitterDelegate, flags)
        {
            WildcardRegex = wildcardRegex;
        }

        public Regex WildcardRegex { get; }
    }
}
