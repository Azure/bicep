// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
            ResultBuilderDelegate resultBuilder,
            TypeSymbol returnType,
            IEnumerable<FixedFunctionParameter> fixedArgumentTypes,
            VariableFunctionParameter? variableArgumentType,
            EvaluatorDelegate? evaluator,
            ExpressionConverterDelegate? expressionConverter,
            FunctionFlags flags = FunctionFlags.Default)
            : base(name, genericDescription, description, resultBuilder, returnType, fixedArgumentTypes, variableArgumentType, evaluator, expressionConverter, flags)
        {
            WildcardRegex = wildcardRegex;
        }

        public Regex WildcardRegex { get; }
    }
}
