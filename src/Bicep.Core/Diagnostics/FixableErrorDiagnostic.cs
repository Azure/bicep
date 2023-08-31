// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public class FixableErrorDiagnostic : ErrorDiagnostic, IFixable
    {
        private readonly CodeFix fix;
        private readonly IEnumerable<CodeFix>? additionalFixes;

        public FixableErrorDiagnostic(Parsing.TextSpan span, string code, string message, Uri? documentationUri, DiagnosticStyling styling, CodeFix fix)
            : base(span, code, message, documentationUri, styling)
        {
            this.fix = fix;
            this.additionalFixes = null;
        }

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
