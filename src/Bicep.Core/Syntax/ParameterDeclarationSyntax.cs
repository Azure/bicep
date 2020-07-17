using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclarationSyntax : StatementSyntax
    {
        public ParameterDeclarationSyntax(Token parameterKeyword, IdentifierSyntax name, TypeSyntax type, Token? defaultKeyword, SyntaxBase? value, Token newLine)
        {
            AssertTokenType(parameterKeyword, nameof(parameterKeyword), TokenType.ParameterKeyword);
            AssertTokenType(defaultKeyword, nameof(defaultKeyword), TokenType.DefaultKeyword);
            AssertTokenType(newLine, nameof(newLine), TokenType.NewLine);

            if ((defaultKeyword == null) != (value == null))
            {
                throw new ArgumentException($"Both {nameof(defaultKeyword)} and {nameof(value)} must be null or they both must be non-null.");
            }

            this.ParameterKeyword = parameterKeyword;
            this.Name = name;
            this.Type = type;
            this.DefaultKeyword = defaultKeyword;
            this.Value = value;
            this.NewLine = newLine;
        }

        public Token ParameterKeyword { get; }
        
        public IdentifierSyntax Name { get; }

        public TypeSyntax Type { get; }

        public Token? DefaultKeyword { get; }

        public SyntaxBase? Value { get; }

        public Token NewLine { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitParameterDeclarationSyntax(this);

        public override TextSpan Span
            => this.Value == null
                ? TextSpan.Between(this.ParameterKeyword, this.Type)
                : TextSpan.Between(this.ParameterKeyword, this.Value);
    }
}