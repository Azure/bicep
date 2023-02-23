// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Text;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : this(FormatTypeName(itemReference, minLength, maxLength), itemReference, validationFlags, minLength, maxLength) { }

        public TypedArrayType(string name, ITypeReference itemReference, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(name, itemReference, validationFlags, minLength, maxLength) {}

        private static string FormatTypeName(ITypeReference itemReference, long? minLength, long? maxLength)
        {
            var nameBuilder = new StringBuilder(itemReference.Type.FormatNameForCompoundTypes()).Append("[]");

            if (minLength.HasValue || maxLength.HasValue)
            {
                nameBuilder.Append(" {");

                if (minLength.HasValue)
                {
                    nameBuilder.Append("@minLength(").Append(minLength.Value).Append(')');
                }

                if (maxLength.HasValue)
                {
                    if (minLength.HasValue)
                    {
                        nameBuilder.Append(", ");
                    }

                    nameBuilder.Append("@maxLength(").Append(maxLength.Value).Append(')');
                }

                nameBuilder.Append('}');
            }

            return nameBuilder.ToString();
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Item.Type;
            }
        }
    }
}
