// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics {
    public abstract class BindableSymbol : Symbol
    {
        protected BindableSymbol(string name, IdentifierSyntax nameSyntax) : base(name)
        {
            this.NameSyntax = nameSyntax;
        }

        /// <summary>
        /// Gets the syntax node of the identifier.
        /// </summary>
        public IdentifierSyntax NameSyntax { get; }
    }

}