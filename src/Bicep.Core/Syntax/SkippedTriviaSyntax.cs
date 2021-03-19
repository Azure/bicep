// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SkippedTriviaSyntax : SyntaxBase
    {
        public SkippedTriviaSyntax(TextSpan span, IEnumerable<SyntaxBase> elements, IEnumerable<IDiagnostic> diagnostics)
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
        public ImmutableArray<IDiagnostic> Diagnostics { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSkippedTriviaSyntax(this);

        public override TextSpan Span { get; }
    }
}