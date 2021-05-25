// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.UnitTests.Diagnostics
{
    public static class IDiagnosticExtensions
    {
        public static IEnumerable<IDiagnostic> ExcludingMissingTypes(this IEnumerable<IDiagnostic> diagnostics)
        {
            return diagnostics.Where(x => x.Code != "BCP081");
        }
    }
}
