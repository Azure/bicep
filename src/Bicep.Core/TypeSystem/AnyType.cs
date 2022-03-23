// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class AnyType : TypeSymbol
    {
        public AnyType()
            : base("any")
        {
        }

        public override TypeKind TypeKind => TypeKind.Any;
    }
}
