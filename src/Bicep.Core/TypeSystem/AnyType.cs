// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Semantics;

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
