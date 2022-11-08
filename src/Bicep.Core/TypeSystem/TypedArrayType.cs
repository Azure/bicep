// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(ITypeReference itemReference, TypeSymbolValidationFlags validationFlags)
            : base(FormatTypeName(itemReference))
        {
            this.Item = itemReference;
            ValidationFlags = validationFlags;
        }

        public override ITypeReference Item { get; }

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        private static string FormatTypeName(ITypeReference itemReference) => $"{itemReference.Type.FormatNameForCompoundTypes()}[]";

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Item.Type;
            }
        }
    }
}
