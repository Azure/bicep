// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(ITypeManager typeManager, string name, SyntaxBase declaringSyntax, IdentifierSyntax nameSyntax)
            : base(name)
        {
            this.TypeManager = typeManager;
            this.DeclaringSyntax = declaringSyntax;
            this.NameSyntax = nameSyntax;
        }

        public ITypeManager TypeManager { get; }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        /// <summary>
        /// Gets the syntax node of the identifier.
        /// </summary>
        public IdentifierSyntax NameSyntax { get; }

        protected TypeSymbol? GetPrimitiveTypeByName(string typeName)
        {
            var type = this.TypeManager.GetTypeByName(typeName);
            if (type?.TypeKind == TypeKind.Primitive)
            {
                return type;
            }

            return null;
        }
    }
}
