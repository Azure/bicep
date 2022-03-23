// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class InternalVariableSymbol : Symbol
    {
        public InternalVariableSymbol(string name, SyntaxBase value)
            : base(name)
        {
            this.Value = value;
        }

        public SyntaxBase Value { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitInternalVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Variable;

    }
}

