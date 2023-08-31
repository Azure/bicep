// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class TypeAssignment
    {
        public TypeAssignment(ITypeReference reference)
            : this(reference, Enumerable.Empty<IDiagnostic>())
        {
        }

        public TypeAssignment(ITypeReference reference, IEnumerable<IDiagnostic> diagnostics)
        {
            Reference = reference;
            Diagnostics = diagnostics;
        }

        public ITypeReference Reference { get; }

        public IEnumerable<IDiagnostic> Diagnostics { get; }
    }
}
