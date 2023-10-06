// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnosticLookup : IEnumerable<IDiagnostic>
    {
        IEnumerable<IDiagnostic> this[IPositionable positionable] { get; }

        bool Contains(IPositionable positionable);
    }
}
