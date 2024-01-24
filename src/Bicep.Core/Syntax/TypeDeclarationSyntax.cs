// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TypeDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    public TypeDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase assignment, SyntaxBase value)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.TypeKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
        AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

        Keyword = keyword;
        Name = name;
        Assignment = assignment;
        Value = value;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Assignment { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(LeadingNodes.FirstOrDefault() ?? Keyword, Value);
}
