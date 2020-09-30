// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(ISymbolContext context, string name, SyntaxBase declaringSyntax, IdentifierSyntaxBase nameSyntax)
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
        public IdentifierSyntaxBase NameSyntax { get; }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        public TypeSymbol Type => this.Context.TypeManager.GetTypeInfo(DeclaringSyntax);

        public bool IsIdentifierValid => this.NameSyntax is IdentifierSyntax;
    }
}
