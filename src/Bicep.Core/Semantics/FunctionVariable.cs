// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class FunctionVariable
    {
        public FunctionVariable(string name, SyntaxBase value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }

        public SyntaxBase Value { get; }
    }
}

