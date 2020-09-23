// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class VariableDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public VariableDeclarationSyntax(Token variableKeyword, IdentifierSyntax name, Token assignment, SyntaxBase value)
        {
            AssertKeyword(variableKeyword, nameof(variableKeyword), LanguageConstants.VariableKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);

            this.VariableKeyword = variableKeyword;
            this.Name = name;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token VariableKeyword { get; }

        public IdentifierSyntax Name { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitVariableDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(VariableKeyword, Value);
    }
}
