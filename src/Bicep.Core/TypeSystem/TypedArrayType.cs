// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(null, itemReference, validationFlags, minLength, maxLength) {}

        public TypedArrayType(string name, ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(ApplyRefinementsToName(name, minLength, maxLength), itemReference, validationFlags, minLength, maxLength) {}

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Item.Type;
            }
        }
    }
}
