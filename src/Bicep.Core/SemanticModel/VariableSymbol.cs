﻿using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class VariableSymbol : DeclaredSymbol
    {
        public VariableSymbol(ITypeContext context, string name, SyntaxBase declaringSyntax, SyntaxBase value) 
            : base(context, name, declaringSyntax)
        {
            this.Value = value;
        }

        public VariableDeclarationSyntax DeclaringVariable => (VariableDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase Value { get; }

        public TypeSymbol Type => this.Context.GetTypeInfo(this.DeclaringVariable.Value);

        public override void Accept(SymbolVisitor visitor) => visitor.VisitVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Variable;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override SyntaxBase? NameSyntax => (this.DeclaringSyntax as VariableDeclarationSyntax)?.Name;
    }
}
