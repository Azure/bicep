// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public class FixableDiagnostic : Diagnostic, IFixable
    {
        private readonly CodeFix fix;
        private readonly IEnumerable<CodeFix>? additionalFixes;

        public FixableDiagnostic(Parsing.TextSpan span, DiagnosticLevel level, string code, string message, Uri? documentationUri, DiagnosticStyling styling, CodeFix fix)
            : base(span, level, code, message, documentationUri, styling)
        {
            this.fix = fix;
            this.additionalFixes = null;
        }

        public FixableDiagnostic(Parsing.TextSpan span, DiagnosticLevel level, string code, string message, Uri? documentationUri, DiagnosticStyling styling, CodeFix fix, params CodeFix[] additionalFixes)
            : this(span, level, code, message, documentationUri, styling, fix)
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
