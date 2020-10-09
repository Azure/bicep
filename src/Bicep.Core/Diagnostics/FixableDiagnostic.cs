// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Linter;

namespace Bicep.Core.Diagnostics
{
    public class FixableDiagnostic : Diagnostic, IFixable
    {
        public FixableDiagnostic(Parser.TextSpan span, DiagnosticLevel level, string code, string message, params Fix[] fixes)
            : base(span, level, code, message)
        {
            this.Fixes = fixes;
        }

        public IReadOnlyList<Fix> Fixes { get; }
    }
}
