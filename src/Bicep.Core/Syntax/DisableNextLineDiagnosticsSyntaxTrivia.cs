// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DisableNextLineDiagnosticsSyntaxTrivia : DisableDiagnosticsSyntaxTrivia
    {
        public DisableNextLineDiagnosticsSyntaxTrivia(Token pound, Token keyword, IEnumerable<SyntaxBase> diagnosticCodes, TextSpan span, string text)
            : base(pound, keyword, diagnosticCodes, SyntaxTriviaType.DisableNextLineDirective, span, text)
        {
        }
    }
}
