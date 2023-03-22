// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Diagnostics
{
    public sealed class EmptyDiagnosticLookup : IDiagnosticLookup
    {
        public readonly static EmptyDiagnosticLookup Instance = new();

        private EmptyDiagnosticLookup() { }

        public IEnumerable<IDiagnostic> this[IPositionable positionable] => Enumerable.Empty<IDiagnostic>();

        public bool Contains(IPositionable positionable) => false;

        public IEnumerator<IDiagnostic> GetEnumerator() => Enumerable.Empty<IDiagnostic>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
