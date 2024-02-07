// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Core.Semantics
{
    public class FunctionWildcardOverloadBuilder(string name, Regex wildcardRegex) : FunctionOverloadBuilder(name)
    {
        private Regex WildcardRegex { get; } = wildcardRegex;

        protected override FunctionOverload BuildInternal()
        {
            return new FunctionWildcardOverload(
                Name,
                GenericDescription,
                Description,
                WildcardRegex,
                ResultBuilder,
                ReturnType,
                FixedParameters.ToImmutable(),
                VariableParameter,
                Evaluator,
                Flags);
        }
    }
}
