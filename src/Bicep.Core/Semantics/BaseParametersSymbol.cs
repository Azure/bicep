// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Text;

namespace Bicep.Core.Semantics
{
    public class BaseParametersSymbol : DeclaredSymbol
    {
        public BaseParametersSymbol(ISymbolContext context, ImmutableArray<ParameterAssignmentSymbol> parentAssignments) : base(context, LanguageConstants.BaseIdentifier, new ImplicitBaseIdentifierSyntax(parentAssignments.First().DeclaringParameterAssignment), parentAssignments.First().DeclaringParameterAssignment.Name)
        {
            this.ParentAssignments = parentAssignments;
            this.syntheticSyntax = (ImplicitBaseIdentifierSyntax) this.DeclaringSyntax;
        }

        public ImmutableArray<ParameterAssignmentSymbol> ParentAssignments { get; }

        private readonly ImplicitBaseIdentifierSyntax syntheticSyntax;

    public override SymbolKind Kind => SymbolKind.BaseParameters;

        public override IEnumerable<Symbol> Descendants => ParentAssignments;

        public override void Accept(SymbolVisitor visitor)
        {
        }

        public ImplicitBaseIdentifierSyntax SyntheticDeclaringSyntax => syntheticSyntax;
    }

    public sealed class ImplicitBaseIdentifierSyntax : SyntaxBase
    {
        private readonly ParameterAssignmentSyntax anchor;

        public ImplicitBaseIdentifierSyntax(ParameterAssignmentSyntax anchor)
        {
            this.anchor = anchor;
        }

    public override TextSpan Span => anchor.Span;

        public override void Accept(ISyntaxVisitor visitor)
        {
        }
    }
}
