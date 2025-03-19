// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class FunctionDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    public FunctionDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase lambda)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.FunctionKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));

        this.Keyword = keyword;
        this.Name = name;
        this.Lambda = lambda;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Lambda { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? Keyword, Lambda);
}
