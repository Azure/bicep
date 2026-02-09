// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class FunctionDeclarationSyntax : NamedDeclarationSyntax
{
    public FunctionDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase lambda)
        : base(keyword, name, leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.FunctionKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));

        this.Lambda = lambda;
    }

    public SyntaxBase Lambda { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? Keyword, Lambda);
}
