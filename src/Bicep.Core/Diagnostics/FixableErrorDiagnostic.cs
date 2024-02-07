// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;

namespace Bicep.Core.Diagnostics
{
    public class FixableErrorDiagnostic(Parsing.TextSpan span, string code, string message, Uri? documentationUri, DiagnosticStyling styling, CodeFix fix) : ErrorDiagnostic(span, code, message, documentationUri, styling), IFixable
    {
        private readonly CodeFix fix = fix;
        private readonly IEnumerable<CodeFix>? additionalFixes = null;

        public FixableErrorDiagnostic(Parsing.TextSpan span, string code, string message, Uri? documentationUri, DiagnosticStyling styling, CodeFix fix, params CodeFix[] additionalFixes)
            : this(span, code, message, documentationUri, styling, fix)
        {
            this.additionalFixes = additionalFixes;
        }

        public IEnumerable<CodeFix> Fixes
        {
            get
            {
                yield return this.fix;

                if (this.additionalFixes != null)
                {
                    foreach (var fix in this.additionalFixes)
                    {
                        yield return fix;
                    }
                }
            }
        }
    }
}
