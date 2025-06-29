// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public sealed class EmptyDiagnosticLookup : IDiagnosticLookup
    {
        public readonly static EmptyDiagnosticLookup Instance = new();

        private EmptyDiagnosticLookup() { }

        public IEnumerable<IDiagnostic> this[IPositionable positionable] => [];

        public bool Contains(IPositionable positionable) => false;

        public IEnumerator<IDiagnostic> GetEnumerator() => Enumerable.Empty<IDiagnostic>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
