// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class FunctionDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    public FunctionDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase assignment, SyntaxBase lambda)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.FunctionKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
        AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

        this.Keyword = keyword;
        this.Name = name;
        this.Assignment = assignment;
        this.Lambda = lambda;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Assignment { get; }

    public SyntaxBase Lambda { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? Keyword, Lambda);
}