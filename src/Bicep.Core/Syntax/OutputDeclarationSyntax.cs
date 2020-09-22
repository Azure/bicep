// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class OutputDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public OutputDeclarationSyntax(Token outputKeyword, IdentifierSyntax name, TypeSyntax type, Token assignment, SyntaxBase value)
        {
            AssertKeyword(outputKeyword, nameof(outputKeyword), LanguageConstants.OutputKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);

            this.OutputKeyword = outputKeyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token OutputKeyword { get; }

        public IdentifierSyntax Name { get; }

        public TypeSyntax Type { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitOutputDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(OutputKeyword, Value);
    }
}
