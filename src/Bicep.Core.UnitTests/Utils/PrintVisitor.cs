// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public class PrintVisitor : SyntaxVisitor
    {
        private readonly StringBuilder buffer;

        private readonly Predicate<IPositionable>? shouldIgnore = null;

        public PrintVisitor(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        public PrintVisitor(StringBuilder buffer, Predicate<IPositionable> shouldIgnore)
            : this(buffer)
        {
            this.shouldIgnore = shouldIgnore;
        }

        public override void VisitToken(Token token)
        {
            WriteTrivia(token.LeadingTrivia);

            if (shouldIgnore == null || !shouldIgnore(token))
            {
                buffer.Append(token.Text);
            }

            WriteTrivia(token.TrailingTrivia);
        }

        private void WriteTrivia(IEnumerable<SyntaxTrivia> triviaList)
        {
            foreach (var trivia in triviaList)
            {
                if (shouldIgnore == null || !shouldIgnore(trivia))
                {
                    buffer.Append(trivia.Text);
                }
            }
        }
    }
}
