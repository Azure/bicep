// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ComponentDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax, IArtifactReferenceSyntax
{
    public ComponentDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase path, SyntaxBase assignment, ImmutableArray<Token> newlines, SyntaxBase value)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.ComponentKeyword);
        AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
        AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
        AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
        AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
        AssertSyntaxType(value, nameof(value), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax), typeof(IfConditionSyntax), typeof(ForSyntax));

        this.Keyword = keyword;
        this.Name = name;
        this.Path = path;
        this.Assignment = assignment;
        this.Newlines = newlines;
        this.Value = value;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Path { get; }

    public SyntaxBase Assignment { get; }

    public ImmutableArray<Token> Newlines { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitComponentDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

    SyntaxBase IArtifactReferenceSyntax.SourceSyntax => Path;

    public ObjectSyntax? TryGetBody() =>
        this.Value switch
        {
            ObjectSyntax @object => @object,
            IfConditionSyntax ifCondition => ifCondition.Body as ObjectSyntax,
            ForSyntax @for => @for.Body switch
            {
                ObjectSyntax @object => @object,
                IfConditionSyntax ifCondition => ifCondition.Body as ObjectSyntax,
                SkippedTriviaSyntax => null,

                _ => throw new NotImplementedException($"Unexpected type of for-expression value '{this.Value.GetType().Name}'.")
            },
            SkippedTriviaSyntax => null,

            // blocked by assert in the constructor
            _ => throw new NotImplementedException($"Unexpected type of module value '{this.Value.GetType().Name}'.")
        };

    public ObjectSyntax GetBody() =>
        this.TryGetBody() ?? throw new InvalidOperationException($"A valid module body is not available on this module due to errors. Use {nameof(TryGetBody)}() instead.");

    public bool HasCondition() => TryGetCondition() is not null;

    public SyntaxBase? TryGetCondition() => Value switch
    {
        IfConditionSyntax ifCondition => ifCondition.ConditionExpression,
        ForSyntax { Body: IfConditionSyntax ifCondition } => ifCondition.ConditionExpression,
        _ => null,
    };


    public ArtifactType GetArtifactType() => ArtifactType.Module;
}