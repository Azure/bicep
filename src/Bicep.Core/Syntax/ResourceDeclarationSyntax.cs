// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public ResourceDeclarationSyntax(Token keyword, IdentifierSyntaxBase name, SyntaxBase type, SyntaxBase assignment, SyntaxBase body)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ResourceKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax), typeof(MalformedIdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Body = body;
        }

        public Token Keyword { get; }

        public IdentifierSyntaxBase Name { get; }

        public SyntaxBase Type { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Body { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(Keyword, Body);

        public StringSyntax? TryGetType() => Type as StringSyntax;
    }
}
