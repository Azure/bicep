// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem
{
    public class StringLiteralType : TypeSymbol
    {
        internal StringLiteralType(string value, TypeSymbolValidationFlags validationFlags)
            // The name of the type should be set to the escaped string value (including quotes).
            // This affects how the type is displayed to the user, and is also used to compare two string literals types for equality.
            : this(StringUtils.EscapeBicepString(value), value, validationFlags) {}

        public StringLiteralType(string typeName, string rawValue, TypeSymbolValidationFlags validationFlags)
            : base(typeName)
        {
            RawStringValue = rawValue;
            ValidationFlags = validationFlags;
        }

        public override TypeKind TypeKind => TypeKind.StringLiteral;

        public string RawStringValue { get; }

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public override bool Equals(object? other) => other is StringLiteralType otherStringLiteral &&
            RawStringValue == otherStringLiteral.RawStringValue &&
            ValidationFlags == otherStringLiteral.ValidationFlags;

        public override int GetHashCode() => HashCode.Combine(TypeKind, RawStringValue, ValidationFlags);
    }
}
