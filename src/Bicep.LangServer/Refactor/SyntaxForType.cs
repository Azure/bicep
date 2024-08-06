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
        public static string GetSyntaxForType(TypeSymbol? type, Strictness strictness) //asdfg test
        {
            //asdfg test
            return type switch
            {
                // Strict literal types
                StringLiteralType
                   or IntegerLiteralType
                   or BooleanLiteralType
                   or TupleType
                   or UnionType
                   when strictness == Strictness.Strict => type.Name,//asdfg ?? is type.Name good enough?

                // Loose/medium literal types
                StringLiteralType => LanguageConstants.String.Name,
                IntegerLiteralType => LanguageConstants.Int.Name,
                BooleanLiteralType => LanguageConstants.Bool.Name,

                TypedArrayType when strictness != Strictness.Loose => type.Name,
                TypedArrayType => LanguageConstants.Array.Name,

                UnionType when strictness == Strictness.Medium => type.Name, //asdfg ?? is type.Name good enough?
                UnionType when strictness == Strictness.Loose => LanguageConstants.String.Name, // asdfg handle other union types

                // Non-literal types
                BooleanType => LanguageConstants.Bool.Name,
                IntegerType => LanguageConstants.Int.Name,
                StringType => LanguageConstants.String.Name,
                TupleType => LanguageConstants.Array.Name,
                NullType => LanguageConstants.Null.Name,


                ObjectType => GetObjectTypeString((ObjectType)type, strictness),

                //asdfg
                //// If it's a custom object like "{ i: int, o: { i2: int } }", keep it that way.
                //// Otherwise, e.g. for resource types (for now) or external types like "VirtualMachineExtensionProperties"
                ////   that aren't recognized in Bicep code, change to Object
                //ObjectType when !type.Name.StartsWith('{') => LanguageConstants.Object.Name,
                //ObjectType => GetObjectTypeString(type),

                AnyType => LanguageConstants.Any.Name, //asdfg???

                _ => UnknownTypeName, //asdfg
            };
        }

        private static string GetObjectTypeString(ObjectType type, Strictness strictness)
        {
            if (strictness == Strictness.Loose)
            {
                return LanguageConstants.Object.Name;
            }

            // asdfg??
            //type.AdditionalPropertiesFlags
            // type.AdditionalPropertiesType
            // type.UnwrapArrayType

            //asdfg what if key needs escaping?
            var members =
                string.Join(", ",
                    type.Properties.Select(p => $"{p.Key}: {GetSyntaxForType(p.Value.TypeReference.Type, strictness)}"));
            return $"{{ {members} }}";
        }
    }
}
