// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ParameterAssignmentSymbol : BindableSymbol
    {

        public ParameterAssignmentSymbol(string name, SyntaxBase declaringSyntax, IdentifierSyntax nameSyntax)
            : base(name, nameSyntax)
        {
            this.DeclaringSyntax = declaringSyntax;
        }
        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        public TypeSymbol Type => throw new NotImplementedException(); // TODO

        public override SymbolKind Kind => SymbolKind.AssignedParameter;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override void Accept(SymbolVisitor visitor)
        {
           visitor.VisitParamAssignmentSymbol(this);
        }
    }
}