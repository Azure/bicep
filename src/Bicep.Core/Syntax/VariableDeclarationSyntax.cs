// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class VariableDeclarationSyntax : SyntaxBase, INamedDeclarationSyntax
    {
        public VariableDeclarationSyntax(Token keyword, IdentifierSyntax name, SyntaxBase assignment, SyntaxBase value)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.VariableKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Name = name;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitVariableDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(Keyword, Value);
    }
}
