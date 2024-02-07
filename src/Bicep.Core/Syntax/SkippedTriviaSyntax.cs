// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements, IEnumerable<ErrorDiagnostic> diagnostics) : SyntaxBase
    {
        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements)
            : this(span, elements, ImmutableArray<ErrorDiagnostic>.Empty)
        {
        }

        public override bool IsSkipped => true;

        /// <summary>
        /// The elements that were skipped.
        /// </summary>
        public ImmutableArray<SyntaxBase> Elements { get; } = elements.ToImmutableArray();

        /// <summary>
        /// Diagnostics to raise.
        /// </summary>
        public ImmutableArray<ErrorDiagnostic> Diagnostics { get; } = diagnostics.ToImmutableArray();

        public string TriviaName => this.Elements.Any() ? LanguageConstants.ErrorName : LanguageConstants.MissingName;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSkippedTriviaSyntax(this);

        public override TextSpan Span { get; } = span;
    }
}
