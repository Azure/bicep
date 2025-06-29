// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Semantics
{
    public class ExtensionConfigAssignmentSymbol : DeclaredSymbol
    {
        private sealed class ExtensionConfigAssignmentNameSource : ISymbolNameSource
        {
            public ExtensionConfigAssignmentNameSource(ExtensionConfigAssignmentSyntax syntax)
            {
                this.Span = syntax.SpecificationString.Span;
            }

            public TextSpan Span { get; }

            public bool IsValid => true;
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
