// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class DeferredTypeReference(Func<TypeSymbol> typeGetterFunc) : ITypeReference
    {
        private readonly Lazy<TypeSymbol> lazyType = new(typeGetterFunc, LazyThreadSafetyMode.PublicationOnly);

        public TypeSymbol Type => lazyType.Value;
    }
}
