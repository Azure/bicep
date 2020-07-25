using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class FunctionCallSyntax : SyntaxBase
    {
        public FunctionCallSyntax(IdentifierSyntax functionName, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
        {
            this.AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            this.AssertTokenType(closeParen, nameof(closeParen), TokenType.RightParen);

            this.FunctionName = functionName;
            this.OpenParen = openParen;
            this.Arguments = arguments.ToImmutableArray();
            this.CloseParen = closeParen;
        }

        public SyntaxBase FunctionName { get; }

        public Token OpenParen { get; }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public Token CloseParen { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(FunctionName, CloseParen);
    }
}