// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem
{
    public class TypeAssignment(ITypeReference reference, IEnumerable<IDiagnostic> diagnostics)
    {
        public TypeAssignment(ITypeReference reference)
            : this(reference, Enumerable.Empty<IDiagnostic>())
        {
        }

        public ITypeReference Reference { get; } = reference;

        public IEnumerable<IDiagnostic> Diagnostics { get; } = diagnostics;
    }
}
