// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class ITypeReferenceExtensions
    {
        public static string GetPrimitiveTypeName(this ITypeReference typeReference) => typeReference.Type switch
        {
            // Widening concrete types is just a workaround. The ultimate goal would be
            // to update BRM in the future to correctly show custom defined types in README.
            NullType => LanguageConstants.NullKeyword,
            IntegerType or IntegerLiteralType => LanguageConstants.Int.Name,
            BooleanType or BooleanLiteralType => LanguageConstants.TypeNameBool,

            StringType or StringLiteralType when IsSecure(typeReference) => "securestring",
            StringType or StringLiteralType => LanguageConstants.TypeNameString,

            ObjectType when IsSecure(typeReference) => "secureObject",
            ObjectType or DiscriminatedObjectType => LanguageConstants.ObjectType,

            ArrayType => LanguageConstants.ArrayType,
            UnionType union => string.Join(" | ", union.Members.Select(x => x.GetPrimitiveTypeName()).Distinct()),

            TypeSymbol otherwise => throw new InvalidOperationException($"Unable to determine primitive type of {otherwise.Name}"),
        };

        private static bool IsSecure(ITypeReference typeReference) => typeReference.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
    }
}
