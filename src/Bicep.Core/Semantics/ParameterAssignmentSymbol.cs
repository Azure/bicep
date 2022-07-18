// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ParameterAssignmentSymbol : BindableSymbol
    {

        private ParamsSymbolContext paramsSymbolContext;
        public ParameterAssignmentSymbol(string name, SyntaxBase assigningSyntax, IdentifierSyntax nameSyntax, ParamsSymbolContext paramsSymbolContext)
            : base(name, nameSyntax)
        {
            this.AssigningSyntax = assigningSyntax;
            this.paramsSymbolContext = paramsSymbolContext;
        }
        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase AssigningSyntax { get; }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        public TypeSymbol Type => paramsSymbolContext.ParamsTypeManager.GetTypeInfo(AssigningSyntax); // change this variable name

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