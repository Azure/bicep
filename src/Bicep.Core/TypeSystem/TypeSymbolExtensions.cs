// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public static class TypeSymbolExtensions
    {
        public static TypeSymbol UnwrapArrayType(this TypeSymbol type) =>
            type switch
            {
                ArrayType arrayType => arrayType.Item.Type,
                _ => type
            };

        public static bool IsSecureType(this TypeSymbol type) => type.Type.FindPathsToSecureTypeComponents(new HashSet<TypeSymbol>(), visitAccessedMembers: true, path: string.Empty).Any();

        public static IEnumerable<string> FindPathsToSecureTypeComponents(this TypeSymbol type, HashSet<TypeSymbol> currentlyProcessing, bool visitAccessedMembers, string path)
        {
            // types can be recursive. cut out early if we've already seen this type
            if (!currentlyProcessing.Add(type))
            {
                yield break;
            }

            if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return path;
            }

            if (type is UnionType union)
            {
                foreach (var variantPath in union.Members.SelectMany(m => m.Type.FindPathsToSecureTypeComponents(currentlyProcessing, visitAccessedMembers, path)))
                {
                    yield return variantPath;
                }
            }

            // if the expression being visited is dereferencing a specific property or index of this type, we shouldn't warn if the type under inspection
            // *contains* properties or indices that are flagged as secure. We will have already warned if those have been accessed in the expression, and
            // if they haven't, then the value dereferenced isn't sensitive
            //
            //    param p {
            //      prop: {
            //        @secure()
            //        nestedSecret: string
            //        nestedInnocuousProperty: string
            //      }
            //    }
            //
            //    output objectContainingSecrets object = p                     // <-- should be flagged
            //    output propertyContainingSecrets object = p.prop              // <-- should be flagged
            //    output nestedSecret string = p.prop.nestedSecret              // <-- should be flagged
            //    output siblingOfSecret string = p.prop.nestedInnocuousData    // <-- should NOT be flagged
            if (visitAccessedMembers)
            {
                switch (type)
                {
                    case ObjectType obj:
                        if (obj.AdditionalProperties?.TypeReference.Type is TypeSymbol addlPropsType)
                        {
                            foreach (var dictMemberPath in addlPropsType.FindPathsToSecureTypeComponents(currentlyProcessing, visitAccessedMembers, $"{path}.*"))
                            {
                                yield return dictMemberPath;
                            }
                        }

                        foreach (var propertyPath in obj.Properties.SelectMany(p => p.Value.TypeReference.Type.FindPathsToSecureTypeComponents(currentlyProcessing, visitAccessedMembers, $"{path}.{p.Key}")))
                        {
                            yield return propertyPath;
                        }
                        break;
                    case TupleType tuple:
                        foreach (var pathFromIndex in tuple.Items.SelectMany((ITypeReference typeAtIndex, int index) => typeAtIndex.Type.FindPathsToSecureTypeComponents(currentlyProcessing, visitAccessedMembers, $"{path}[{index}]")))
                        {
                            yield return pathFromIndex;
                        }
                        break;
                    case ArrayType array:
                        foreach (var pathFromElement in array.Item.Type.FindPathsToSecureTypeComponents(currentlyProcessing, visitAccessedMembers, $"{path}[*]"))
                        {
                            yield return pathFromElement;
                        }
                        break;
                }
            }

            currentlyProcessing.Remove(type);
        }

        public static bool IsIntegerOrIntegerLiteral(this TypeSymbol type) =>
             type is IntegerType or IntegerLiteralType;

        public static bool ExtensionNameEquals(this NamespaceType namespaceType, string extensionName)
            => StringComparer.Ordinal.Equals(namespaceType.ExtensionName, extensionName);

        public static bool AliasNameEquals(this NamespaceType namespaceType, string aliasName)
            => LanguageConstants.IdentifierComparer.Equals(namespaceType.Name, aliasName);
    }
}
