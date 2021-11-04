// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineSyntax : DisableDiagnosticsSyntax
    {
        public DisableNextLineSyntax(Token keyword, IEnumerable<SyntaxBase> diagnosticCodes)
            :base(keyword, diagnosticCodes, TokenType.DisableNextLine)
        {
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitDisableNextLineSyntax(this);
    }
}
