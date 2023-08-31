// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Semantics
{
    public class FunctionWildcardOverload : FunctionOverload
    {
        public FunctionWildcardOverload(
            string name,
            string genericDescription,
            string description,
            Regex wildcardRegex,
            ResultBuilderDelegate resultBuilder,
            TypeSymbol returnType,
            IEnumerable<FixedFunctionParameter> fixedArgumentTypes,
            VariableFunctionParameter? variableArgumentType,
            EvaluatorDelegate? evaluator,
            FunctionFlags flags = FunctionFlags.Default)
            : base(name, genericDescription, description, resultBuilder, returnType, fixedArgumentTypes, variableArgumentType, evaluator, flags)
        {
            WildcardRegex = wildcardRegex;
        }

        public Regex WildcardRegex { get; }
    }
}
