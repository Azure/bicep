// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class VariableSymbol : DeclaredSymbol
    {
        public VariableSymbol(ISymbolContext context, string name, VariableDeclarationSyntax declaringSyntax, SyntaxBase value) 
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Value = value;
        }

        public VariableDeclarationSyntax DeclaringVariable => (VariableDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase Value { get; }

        public override TypeSymbol Type => GetVariableType(new TypeManagerContext());

        public TypeSymbol GetVariableType(TypeManagerContext context)
        {
            return this.Context.TypeManager.GetTypeInfo(this.DeclaringVariable.Value, context);
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Variable;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}

