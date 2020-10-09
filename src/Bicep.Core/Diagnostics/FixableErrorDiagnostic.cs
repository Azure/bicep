// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Linter;

namespace Bicep.Core.Diagnostics
{
    public class FixableErrorDiagnostic : ErrorDiagnostic, IFixable
    {
        public FixableErrorDiagnostic(Parser.TextSpan span, string code, string message, params Fix[] fixes)
            : base(span, code, message)
        {
            this.Fixes = fixes;
        }

        public IReadOnlyList<Fix> Fixes { get; }
    }
}
