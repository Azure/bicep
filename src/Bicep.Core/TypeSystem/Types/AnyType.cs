// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
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
