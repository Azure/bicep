// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(ISymbolContext context, string name, SyntaxBase declaringSyntax, IdentifierSyntax nameSyntax)
            : base(name)
        {
            this.Context = context;
            this.DeclaringSyntax = declaringSyntax;
            this.NameSyntax = nameSyntax;
        }

        public ISymbolContext Context { get; }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        /// <summary>
        /// Gets the syntax node of the identifier.
        /// </summary>
        public IdentifierSyntax NameSyntax { get; }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        public TypeSymbol Type => this.Context.TypeManager.GetTypeInfo(DeclaringSyntax);
    }
}
