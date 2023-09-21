// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem
{
    public class ArrayType : TypeSymbol
    {
        internal ArrayType(TypeSymbolValidationFlags validationFlags = default, long? minLength = null, long? maxLength = null)
            : this(LanguageConstants.ArrayType, LanguageConstants.Any, validationFlags, minLength, maxLength) { }

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

        public override bool Equals(object? other) => other is ArrayType otherArray &&
            ValidationFlags == otherArray.ValidationFlags &&
            MinLength == otherArray.MinLength &&
            MaxLength == otherArray.MaxLength &&
            Name == otherArray.Name &&
            Item.Equals(otherArray.Item);

        public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinLength, MaxLength, Name, Item);
    }
}
