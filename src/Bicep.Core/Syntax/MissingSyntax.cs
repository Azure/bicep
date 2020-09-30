// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class MissingSyntax : SyntaxBase
    {
        public MissingSyntax(TextSpan span, Diagnostic diagnostic)
        {
            this.Span = span;
            this.Diagnostic = diagnostic;
        }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitMissingSyntax(this);

        public override TextSpan Span { get; }

        public Diagnostic Diagnostic { get; }
    }
}