// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ResourceRefType : TypeSymbol
    {
        public ResourceRefType()
            : base("resource")
        {
        }

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}