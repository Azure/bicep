// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

namespace Bicep.Core.TypeSystem
{
    public class ArrayType : TypeSymbol
    {
        public ArrayType(TypeSymbolValidationFlags validationFlags = default, long? minLength = null, long? maxLength = null)
            : this(null, LanguageConstants.Any, validationFlags, minLength, maxLength) {}

        protected ArrayType(string? name, ITypeReference item, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(name ?? FormatTypeName(item, minLength, maxLength))
        {
            Item = item;
            ValidationFlags = validationFlags;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public virtual ITypeReference Item { get; }

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public long? MinLength { get; }

        public long? MaxLength { get; }

        private static string FormatTypeName(ITypeReference itemReference, long? minLength, long? maxLength)
        {
            var nameBuilder = new StringBuilder();

            if (ReferenceEquals(itemReference, LanguageConstants.Any))
            {
                nameBuilder.Append(LanguageConstants.ArrayType);
            }
            else
            {
                nameBuilder.Append(itemReference.Type.FormatNameForCompoundTypes()).Append("[]");
            }

            ApplyRefinementsToName(nameBuilder, minLength, maxLength);

            return nameBuilder.ToString();
        }

        protected static string ApplyRefinementsToName(string baseName, long? minLength, long? maxLength)
        {
            var nameBuilder = new StringBuilder(baseName);
            ApplyRefinementsToName(nameBuilder, minLength, maxLength);
            return nameBuilder.ToString();
        }

        private static void ApplyRefinementsToName(StringBuilder nameBuilder, long? minLength, long? maxLength)
        {
            if (minLength.HasValue || maxLength.HasValue)
            {
                nameBuilder.Append(" {");

                if (minLength.HasValue)
                {
                    nameBuilder.Append('@').Append(LanguageConstants.ParameterMinLengthPropertyName).Append('(').Append(minLength.Value).Append(')');
                }

                if (maxLength.HasValue)
                {
                    if (minLength.HasValue)
                    {
                        nameBuilder.Append(", ");
                    }

                    nameBuilder.Append('@').Append(LanguageConstants.ParameterMaxLengthPropertyName).Append('(').Append(maxLength.Value).Append(')');
                }

                nameBuilder.Append('}');
            }
        }
    }
}
