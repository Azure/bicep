// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class ExtensionConfigAssignmentSymbol : DeclaredSymbol
    {
        private class ExtensionConfigAssignmentNameSource : ISymbolNameSource
        {
            public ExtensionConfigAssignmentNameSource(ExtensionConfigAssignmentSyntax syntax)
            {
                // TODO(kylealbert): is this right?
                this.Span = syntax.SpecificationString.Span;
            }

            public TextSpan Span { get; }

            public bool IsValid { get; } = true;
        }

        public ExtensionConfigAssignmentSymbol(ISymbolContext context, string name, ExtensionConfigAssignmentSyntax declaringSyntax)
            : base(context, name, declaringSyntax, new ExtensionConfigAssignmentNameSource(declaringSyntax))
        {
        }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public ExtensionConfigAssignmentSyntax DeclaringExtensionConfigAssignment => (ExtensionConfigAssignmentSyntax)this.DeclaringSyntax;

        public override SymbolKind Kind => SymbolKind.ExtensionConfigAssignment;

        public override IEnumerable<Symbol> Descendants
        {
            get { yield return this.Type; }
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitExtensionConfigAssignmentSymbol(this);
    }
}
