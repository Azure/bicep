// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class SkippedTriviaSyntax : SyntaxBase
    {
        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements)
            : this(span, elements, ImmutableArray<Diagnostic>.Empty)
        {
        }

        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements, IEnumerable<Diagnostic> diagnostics)
        {
            this.Span = span;
            this.Elements = [.. elements];
            this.Diagnostics = [.. diagnostics];
        }

        public override bool IsSkipped => true;

        /// <summary>
        /// The elements that were skipped.
        /// </summary>
        public ImmutableArray<SyntaxBase> Elements { get; }

        /// <summary>
        /// Diagnostics to raise.
        /// </summary>
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public string TriviaName => this.Elements.Any() ? LanguageConstants.ErrorName : LanguageConstants.MissingName;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSkippedTriviaSyntax(this);

        public override TextSpan Span { get; }
    }
}
