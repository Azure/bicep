// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;

namespace Bicep.Core.TypeSystem
{
    public class DeferredTypeReference : ITypeReference
    {
        private readonly Lazy<TypeSymbol> lazyType;

        public DeferredTypeReference(Func<TypeSymbol> typeGetterFunc)
        {
            lazyType = new(typeGetterFunc, LazyThreadSafetyMode.PublicationOnly);
        }

        public TypeSymbol Type => lazyType.Value;
    }
}
