// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ResourceDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, Token? existingKeyword, Token? nullableMarker, SyntaxBase assignment, ImmutableArray<Token> newlines, SyntaxBase value)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ResourceKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertKeyword(existingKeyword, nameof(existingKeyword), LanguageConstants.ExistingKeyword);
            AssertTokenType(nullableMarker, nameof(nullableMarker), TokenType.Question);
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
            AssertSyntaxType(value, nameof(value), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax), typeof(IfConditionSyntax), typeof(ForSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.ExistingKeyword = existingKeyword;
            this.NullableMarker = nullableMarker;
            this.Assignment = assignment;
            this.Newlines = newlines;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        public Token? ExistingKeyword { get; }

        /// <summary>
        /// The '?' token that follows the 'existing' keyword, indicating the resource may not exist.
        /// </summary>
        public Token? NullableMarker { get; }

        public SyntaxBase Assignment { get; }

        public ImmutableArray<Token> Newlines { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

        public StringSyntax? TypeString => Type as StringSyntax;

        public bool IsExistingResource() => ExistingKeyword is not null;

        /// <summary>
        /// Returns true if this is an 'existing?' resource declaration, meaning the resource may not exist
        /// and its type should be nullable.
        /// </summary>
        public bool IsNullableExistingResource() => ExistingKeyword is not null && NullableMarker is not null;

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
                _ => throw new NotImplementedException($"Unexpected type of resource value '{this.Value.GetType().Name}'.")
            };

        public ObjectSyntax GetBody() =>
            this.TryGetBody() ?? throw new InvalidOperationException($"A valid resource body is not available on this module due to errors. Use {nameof(TryGetBody)}() instead.");

        public bool HasCondition() => TryGetCondition() is not null;

        public SyntaxBase? TryGetCondition() => Value switch
        {
            IfConditionSyntax ifCondition => ifCondition.ConditionExpression,
            ForSyntax { Body: IfConditionSyntax ifCondition } => ifCondition.ConditionExpression,
            _ => null,
        };

        public bool IsCollection() => this.Value is ForSyntax;
    }
}
