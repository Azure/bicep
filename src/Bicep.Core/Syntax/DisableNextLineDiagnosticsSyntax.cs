// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineDiagnosticsSyntax : DisableDiagnosticsSyntax
    {
        public DisableNextLineDiagnosticsSyntax(Token pound, Token keyword, IEnumerable<SyntaxBase> diagnosticCodes)
            :base(pound, keyword, diagnosticCodes, LanguageConstants.DisableNextLineDiagnosticsKeyword)
        {
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitDisableNextLineSyntax(this);
    }
}
