// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.TypeSystem
{
    public class StringLiteralType : TypeSymbol
    {
        public StringLiteralType(string value)
            : base(StringUtils.EscapeBicepString(value))
        {
            // The name of the type should be set to the escaped string value (including quotes).
            // This affects how the type is displayed to the user, and is also used to compare two string literals types for equality.
            RawStringValue = value;
        }
        public StringLiteralType(string typeName, string rawValue):
            base (typeName)
        {
            RawStringValue = rawValue;
        }
        public override TypeKind TypeKind => TypeKind.StringLiteral;

        public string RawStringValue { get; }
    }
}
