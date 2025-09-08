// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class RuleDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    public RuleDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, SyntaxBase assignment, SyntaxBase value)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.RuleKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
        AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
        AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

        this.Keyword = keyword;
        this.Name = name;
        this.Type = type;
        this.Assignment = assignment;
        this.Value = value;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Type { get; }

    public SyntaxBase Assignment { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitRuleDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

    public StringSyntax? TypeString => Type as StringSyntax;
}