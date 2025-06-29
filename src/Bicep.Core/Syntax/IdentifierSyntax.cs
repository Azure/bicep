// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a well-formed identifier.
    /// </summary>
    [DebuggerDisplay("IdentifierName = {" + nameof(IdentifierName) + "}")]
    public class IdentifierSyntax : SyntaxBase, ISymbolNameSource
    {
        public IdentifierSyntax(SyntaxBase child)
        {
            if (child is Token token)
            {
                AssertTokenType(token, nameof(child), TokenType.Identifier);
                Assert(string.IsNullOrEmpty(token.Text) == false, "Identifier must not be null or empty.");
            }
            else
            {
                AssertSyntaxType(child, nameof(child), typeof(SkippedTriviaSyntax));
            }

            this.Child = child;
        }

        public SyntaxBase Child { get; }

        public string IdentifierName
        {
            get
            {
                switch (this.Child)
                {
                    case Token identifier:
                        return identifier.Text;

                    case SkippedTriviaSyntax skipped:
                        return skipped.TriviaName;

                    default:
                        throw new NotImplementedException($"Unexpected child node type '{this.Child.GetType().Name}'.");
                }
            }
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span => this.Child.Span;

        public bool IsValid => this.Child is Token;
    }
}
