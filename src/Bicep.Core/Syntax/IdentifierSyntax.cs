// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a well-formed identifier.
    /// </summary>
    [DebuggerDisplay("IdentifierName = {" + nameof(IdentifierName) +"}")]
    public class IdentifierSyntax : SyntaxBase
    {
        private static readonly Regex SnippetPlaceholderPattern = new Regex(@"\${\d+:\w+}", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

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

                        ImmutableArray<SyntaxBase> elements = skipped.Elements;
                        if (elements.Any())
                        {
                            return GetIdentifierName(elements);
                        }
                        return LanguageConstants.MissingName;


                    default:
                        throw new NotImplementedException($"Unexpected child node type '{this.Child.GetType().Name}'.");
                }
            }
        }

        private string GetIdentifierName(ImmutableArray<SyntaxBase> elements)
        {
            // Snippet templates have placeholders for symbolic name. If elements contains tokens with types in following
            // order - [Dollar, LeftBrace, Integer, Colon, Identifier, RightBrace], we'll recognize the pattern and use the text
            // as identifier name
            // E.g:
            // resource ${1:dnsZone} 'Microsoft.Network/dnsZones@2018-05-01' = {
            //   name: ${ 2:'name'}
            //   location: 'global'
            // }
            // In the above example, the IdentifierName of ResourceDeclarationSyntax would be ${1:dnsZone}
            if (elements.Count() == 6)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SyntaxBase element in elements)
                {
                    if (element is Token token)
                    {
                        sb.Append(token.Text);
                    }
                }

                string text = sb.ToString();
                if (SnippetPlaceholderPattern.IsMatch(text))
                {
                    return text;
                }
            }

            return LanguageConstants.ErrorName;
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span => this.Child.Span;

        public bool IsValid => this.Child is Token;
    }
}
