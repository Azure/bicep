// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ArrayType : TypeSymbol
    {
        public ArrayType(TypeSymbolValidationFlags validationFlags = default, long? minLength = null, long? maxLength = null)
            : this(LanguageConstants.ArrayType, LanguageConstants.Any, validationFlags, minLength, maxLength) {}

        protected ArrayType(string name, ITypeReference item, TypeSymbolValidationFlags validationFlags, long? minLength = null, long? maxLength = null)
            : base(name)
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
    }
}
