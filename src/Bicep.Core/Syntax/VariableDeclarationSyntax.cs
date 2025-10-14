// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class VariableDeclarationSyntax : NamedDeclarationSyntax
    {
        public VariableDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase? type, SyntaxBase assignment, SyntaxBase value)
            : base(keyword, name, leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.VariableKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Type = type;
            this.Assignment = assignment;
            this.Value = value;
        }

        public SyntaxBase? Type { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public SyntaxBase? TryGetBody() => UnwrapBody(Value);

        private static SyntaxBase? UnwrapBody(SyntaxBase body) => body switch
        {
            SkippedTriviaSyntax => null,
            ForSyntax @for => @for.Body,
            ParenthesizedExpressionSyntax parenthesized => UnwrapBody(parenthesized.Expression),
            var otherwise => otherwise,
        };

        public SyntaxBase GetBody() => TryGetBody() ??
            throw new InvalidOperationException($"A valid body is not available on this variable due to errors. Use {nameof(TryGetBody)}() instead.");

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitVariableDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? Keyword, Value);
    }
}
