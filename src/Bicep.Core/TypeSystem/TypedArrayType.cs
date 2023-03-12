// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(FormatTypeName(itemReference), itemReference, validationFlags, minLength, maxLength) {}

        public TypedArrayType(string name, ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(name, itemReference, validationFlags, minLength, maxLength) {}

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Item.Type;
            }
        }

        private static string FormatTypeName(ITypeReference itemReference) => itemReference.Type switch
        {
            TypeSymbol typeSymbol when ReferenceEquals(typeSymbol, LanguageConstants.Any) => LanguageConstants.ArrayType,
            TypeSymbol otherwise => $"{itemReference.Type.FormatNameForCompoundTypes()}[]",
        };
    }
}
