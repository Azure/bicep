// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeAssignment
    {
        public DeclaredTypeAssignment(ITypeReference reference, DeclaredTypeFlags flags)
        {
            this.Reference = reference;
            this.Flags = flags;
        }

        public DeclaredTypeAssignment(ITypeReference reference)
            : this(reference, DeclaredTypeFlags.None)
        {
        }

        public ITypeReference Reference { get; }

        public DeclaredTypeFlags Flags { get; }
    }
}