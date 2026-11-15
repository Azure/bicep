// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class GroupDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    public GroupDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase assignment, ImmutableArray<Token> newlines, SyntaxBase value)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.GroupKeyword);
        AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
        AssertSyntaxType(value, nameof(value), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax), typeof(IfConditionSyntax), typeof(ForSyntax));

        Keyword = keyword;
        Name = name;
        Assignment = assignment;
        Newlines = newlines;
        Value = value;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Assignment { get; }

    public ImmutableArray<Token> Newlines { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitGroupDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(LeadingNodes.FirstOrDefault() ?? Keyword, Value);

    public ObjectSyntax? TryGetBody() => Value switch
    {
        ObjectSyntax body => body,
        IfConditionSyntax { Body: ObjectSyntax body } => body,
        ForSyntax { Body: ObjectSyntax body } => body,
        ForSyntax { Body: IfConditionSyntax { Body: ObjectSyntax body } } => body,
        _ => null,
    };
}
