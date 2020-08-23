using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class SkippedTriviaSyntax : SyntaxBase
    {
        public SkippedTriviaSyntax(TextSpan span, IEnumerable<Token> tokens, IEnumerable<SyntaxBase> syntax, IEnumerable<Diagnostic> diagnostics)
        {
            this.Span = span;
            this.Tokens = tokens.ToImmutableArray();
            this.Syntax = syntax.ToImmutableArray();
            this.Diagnostics = diagnostics.ToImmutableArray();
        }

        public override bool IsSkipped => true;

        /// <summary>
        /// The tokens that were skipped.
        /// </summary>
        public ImmutableArray<Token> Tokens { get; }

        /// <summary>
        /// Syntax that was skipped.
        /// </summary>
        public ImmutableArray<SyntaxBase> Syntax { get; }

        /// <summary>
        /// Diagnostics to raise.
        /// </summary>
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitSkippedTriviaSyntax(this);

        public override TextSpan Span { get; }
    }
}