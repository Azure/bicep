// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class FunctionWildcardOverload : FunctionOverload
    {
        public FunctionWildcardOverload(string name, TypeSymbol returnType, int minimumArgumentCount, int? maximumArgumentCount, IEnumerable<TypeSymbol> fixedArgumentTypes, TypeSymbol? variableArgumentType, Regex wildcardRegex, FunctionFlags flags = FunctionFlags.Default)
            : base(name, returnType, minimumArgumentCount, maximumArgumentCount, fixedArgumentTypes, variableArgumentType, flags)
        {
            WildcardRegex = wildcardRegex;
        }

        public Regex WildcardRegex { get; }
    }
}
