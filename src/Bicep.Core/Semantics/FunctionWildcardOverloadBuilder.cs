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
                this.Name,
                this.Description,
                this.WildcardRegex,
                this.ReturnTypeBuilder,
                this.ReturnType,
                this.FixedParameters.ToImmutable(),
                this.VariableParameter,
                this.Flags);
        }
    }
}