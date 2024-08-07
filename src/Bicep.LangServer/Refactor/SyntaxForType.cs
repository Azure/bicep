// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.TypeSystem;
using Bicep.Core;
using Bicep.Core.Parsing;
using static Bicep.LanguageServer.Refactor.SyntaxForType;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Refactor
{
    public static class SyntaxForType
    {
        private const string UnknownTypeName = "unknown";

        public enum Strictness
        {
            Strict, // Create syntax representing the exact type, e.g. `{ p1: 123, p2: 'abc' | 'def' }`
            Medium, // Widen literal types, e.g. => `{ p1: int, p2: 'abc' | 'def' }`
            Loose,  // Widen everything to basic types only, e.g. => `object`
        }

        //asdfg should this be creating syntax nodes?  Probably...
        //asdfg recursive types
        public static string GetSyntaxStringForType(TypeSymbol? type, Strictness strictness) //asdfg test
        {
            //asdfg test
            return type switch
            {
                // Literal types - keep as is only when strict
                StringLiteralType
                   or IntegerLiteralType
                   or BooleanLiteralType
                   when strictness == Strictness.Strict => type.Name,

                // ... otherwise widen to simple type
                StringLiteralType => LanguageConstants.String.Name,
                IntegerLiteralType => LanguageConstants.Int.Name,
                BooleanLiteralType => LanguageConstants.Bool.Name,

                TupleType when strictness == Strictness.Loose => LanguageConstants.Array.Name,
                TupleType tupleType => $"[{string.Join(", ", tupleType.Items.Select(tt => GetSyntaxStringForType(tt.Type, strictness)))}]",

                TypedArrayType when strictness == Strictness.Loose => LanguageConstants.Array.Name,
                TypedArrayType typedArrayType =>
                    $"{
                        (typedArrayType.Item.Type is UnionType
                            ? $"({GetSyntaxStringForType(typedArrayType.Item.Type, strictness)})"
                            : GetSyntaxStringForType(typedArrayType.Item.Type, strictness))
                        }[]",                    

                UnionType unionType when strictness == Strictness.Loose =>
                    // Widen to the item type (which are supposed to all be literal types of the same type)
                    GetSyntaxStringForType(unionType.Members.FirstOrDefault()?.Type, strictness),
                UnionType => type.Name,

                BooleanType => LanguageConstants.Bool.Name,
                IntegerType => LanguageConstants.Int.Name,
                StringType => LanguageConstants.String.Name,
                NullType => LanguageConstants.Null.Name,


                ObjectType objectType when (strictness == Strictness.Loose || objectType.Properties.Count == 0) => LanguageConstants.Object.Name,
                ObjectType objectType => $"{{ {
                    string.Join(", ", objectType.Properties.Select(p => GetFormattedTypeProperty(p.Value, strictness)))
                } }}",

                AnyType => LanguageConstants.Any.Name, //asdfg??? test

                _ => UnknownTypeName,
            };
        }

        // asdfg??
        //type.AdditionalPropertiesFlags
        // type.AdditionalPropertiesType
        // type.UnwrapArrayType

        private static string GetFormattedTypeProperty(TypeProperty property, Strictness strictness)
        {
            return
                $"{StringUtils.EscapeBicepPropertyName(property.Name)}: {GetSyntaxStringForType(property.TypeReference.Type, strictness)}";
        }
    }
}
