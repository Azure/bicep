// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Core.Semantics
{
    public class FunctionWildcardOverloadBuilder : FunctionOverloadBuilder
    {
        private Regex WildcardRegex { get; }

        public FunctionWildcardOverloadBuilder(string name, Regex wildcardRegex) : base(name)
        {
            this.WildcardRegex = wildcardRegex;
        }

        public override FunctionOverload BuildInternal()
        {
            return new FunctionWildcardOverload(
                Name,
                Description,
                WildcardRegex,
                ReturnTypeBuilder,
                ReturnType,
                FixedParameters.ToImmutable(),
                VariableParameter,
                Evaluator,
                Flags);
        }
    }
}
