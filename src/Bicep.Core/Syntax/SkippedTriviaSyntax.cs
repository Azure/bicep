// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Syntax
{
    public class SkippedTriviaSyntax : SyntaxBase
    {
        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements)
            : this(span, elements, ImmutableArray<ErrorDiagnostic>.Empty)
        {
        }

        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements, IEnumerable<ErrorDiagnostic> diagnostics)
        {
            this.Span = span;
            this.Elements = elements.ToImmutableArray();
            this.Diagnostics = diagnostics.ToImmutableArray();
        }

        public override bool IsSkipped => true;

        /// <summary>
        /// The elements that were skipped.
        /// </summary>
        public ImmutableArray<SyntaxBase> Elements { get; }

        /// <summary>
        /// Diagnostics to raise.
        /// </summary>
        public ImmutableArray<ErrorDiagnostic> Diagnostics { get; }

        public string TriviaName => this.Elements.Any() ? LanguageConstants.ErrorName : LanguageConstants.MissingName;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSkippedTriviaSyntax(this);

        public override TextSpan Span { get; }
    }
}
