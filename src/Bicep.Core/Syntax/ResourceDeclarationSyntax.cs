// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public ResourceDeclarationSyntax(Token resourceKeyword, IdentifierSyntax name, SyntaxBase type, Token assignment, SyntaxBase body)
        {
            AssertKeyword(resourceKeyword, nameof(resourceKeyword), LanguageConstants.ResourceKeyword);
            AssertTokenType(resourceKeyword, nameof(resourceKeyword), TokenType.Identifier);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);

            this.ResourceKeyword = resourceKeyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Body = body;
        }

        public Token ResourceKeyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        public Token Assignment { get; }

        public SyntaxBase Body { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(ResourceKeyword, Body);

        public StringSyntax? TryGetType() => Type as StringSyntax;
    }
}
