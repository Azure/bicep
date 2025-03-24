// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnosticLookup : IEnumerable<IDiagnostic>
    {
        IEnumerable<IDiagnostic> this[IPositionable positionable] { get; }

        bool Contains(IPositionable positionable);
    }
}
