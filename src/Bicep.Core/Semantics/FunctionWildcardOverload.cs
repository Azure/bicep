// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionWildcardOverload(
        string name,
        string genericDescription,
        string description,
        Regex wildcardRegex,
        ResultBuilderDelegate resultBuilder,
        TypeSymbol returnType,
        IEnumerable<FixedFunctionParameter> fixedArgumentTypes,
        VariableFunctionParameter? variableArgumentType,
        EvaluatorDelegate? evaluator,
        FunctionFlags flags = FunctionFlags.Default) : FunctionOverload(name, genericDescription, description, resultBuilder, returnType, fixedArgumentTypes, variableArgumentType, evaluator, flags)
    {
        public Regex WildcardRegex { get; } = wildcardRegex;
    }
}
