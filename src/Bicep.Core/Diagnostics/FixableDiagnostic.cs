// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.CodeAction;

namespace Bicep.Core.Diagnostics
{
    public class FixableDiagnostic : Diagnostic, IFixable
    {
        private readonly CodeFix fix;
        private readonly IEnumerable<CodeFix>? additionalFixes;

        public FixableDiagnostic(Parser.TextSpan span, DiagnosticLevel level, string code, string message, CodeFix fix)
            : base(span, level, code, message)
        {
            this.fix = fix;
            this.additionalFixes = null;
        }

        public FixableDiagnostic(Parser.TextSpan span, DiagnosticLevel level, string code, string message, CodeFix fix, params CodeFix[] additionalFixes)
            : this(span, level, code, message, fix)
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
