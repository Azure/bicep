// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    public class DeferredTypeReference : ITypeReference
    {
        private readonly Func<TypeSymbol> typeGetterFunc;

        public DeferredTypeReference(Func<TypeSymbol> typeGetterFunc)
        {
            this.typeGetterFunc = typeGetterFunc;
        }

        public TypeSymbol Type => typeGetterFunc();
    }
}