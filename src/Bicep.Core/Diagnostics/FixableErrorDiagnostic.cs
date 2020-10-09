// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.CodeAction;

namespace Bicep.Core.Diagnostics
{
    public class FixableErrorDiagnostic : ErrorDiagnostic, IFixable
    {
        private readonly CodeFix fix;
        private readonly IEnumerable<CodeFix>? additionalFixes;

        public FixableErrorDiagnostic(Parser.TextSpan span, string code, string message, CodeFix fix)
            : base(span, code, message)
        {
            this.fix = fix;
            this.additionalFixes = null;
        }

        public FixableErrorDiagnostic(Parser.TextSpan span, string code, string message, CodeFix fix, params CodeFix[] additionalFixes)
            : this(span, code, message, fix)
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
