// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.LanguageServer.Refactor;

public static class TypeStringifier
{
    private const string UnknownTypeName = "object? /* unknown */";
    private const string AnyTypeName = "object? /* any */";
    private const string RecursiveTypeName = "object /* recursive */";
    private const string ErrorTypeName = "object? /* error */";
    private const string NullTypeName = "object? /* null */";

    public enum Strictness
    {
        /// <summary>
        /// Create syntax representing the exact type, e.g. `{ p1: 123, p2: 'abc' | 'def' }`
        /// </summary>
        Strict,

        /// <summary>
        /// Widen literal types when not part of a union, e.g. => `{ p1: int, p2: 'abc' | 'def' }`, empty arrays/objects, tuples, etc, hopefully more in line with user needs
        /// </summary>
        Medium,

        /// <summary>
        /// Widen everything to basic types only, e.g. => `object`
        /// </summary>
        Loose,
    }

    // Note: This is "best effort" code for now. Ideally we should handle this exactly, but Bicep doesn't support expressing all the types it actually supports
    // Note: Returns type on a single line
    public static string Stringify(TypeSymbol? type, TypeProperty? typeProperty, Strictness strictness, bool removeTopLevelNullability = false)
    {
        return StringifyCore(type, typeProperty, strictness, [], removeTopLevelNullability);
    }

    // This works off of the syntax tree of the declared resource rather than the types due to type system limitations
    public static string? TryGetResourceDerivedTypeName(SemanticModel semanticModel, ObjectPropertySyntax propertySyntax)
    {
        SyntaxBase? current = propertySyntax;
        string propertyAccessDotNotation = ""; // Includes leading periods

        while (current is not null)
        {
            if (current is ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                // Found the resource itself
                var resourceTypeName = (resourceDeclarationSyntax.Type as StringSyntax)?.TryGetLiteralValue();
                return $"resourceInput<'{resourceTypeName}'>{propertyAccessDotNotation}";
            }
            else if (current is ObjectPropertySyntax objectPropertySyntax)
            {
                var declaredType = semanticModel.GetDeclaredType(current);

                var isInterestingObjectType =
                    declaredType is ObjectType objectType
                    && objectType.Name != LanguageConstants.ObjectType
                    && GetWriteableProperties(objectType).Length > 0;

                if (isInterestingObjectType || declaredType is ArrayType)
                {
                    var propertyName = (objectPropertySyntax.Key as IdentifierSyntax)?.IdentifierName;
                    if (propertyName is null)
                    {
                        return null;
                    }

                    propertyAccessDotNotation = $".{propertyName}{propertyAccessDotNotation}";
                }
                else
                {
                    // Not an interesting resource-derived type (for example a primitive type, any, or simple object) that user can declare without needing a resource-derived type
                    return null;
                }
            }
            else if (current is ArrayItemSyntax)
            {
                propertyAccessDotNotation = $"[*]{propertyAccessDotNotation}";
            }

            current = semanticModel.Binder.GetParent(current);
        }

        return null;
    }

    private static string StringifyCore(TypeSymbol? type, TypeProperty? typeProperty, Strictness strictness, TypeSymbol[] visitedTypes, bool removeTopLevelNullability = false)
    {
        if (type == null)
        {
            return UnknownTypeName;
        }

        if (visitedTypes.Contains(type))
        {
            return RecursiveTypeName;
        }

        TypeSymbol[] previousVisitedTypes = visitedTypes;
        visitedTypes = [.. previousVisitedTypes, type];

        type = WidenType(type, strictness);

        // If from an object property that is implicitly allowed to be null (like for many resource properties)
        if (!removeTopLevelNullability && typeProperty?.Flags.HasFlag(TypePropertyFlags.AllowImplicitNull) == true)
        {
            // Won't recursive forever because now typeProperty = null
            // Note though that because this is by nature recursive with the same type, we must pass in previousVisitedTypes
            return StringifyCore(TypeHelper.MakeNullable(type), null, strictness, previousVisitedTypes);
        }

        // Show nullable types (always represented as a union type containing "null" as a member")
        //   as "type?" rather than "type|null"
        if (TypeHelper.TryRemoveNullability(type) is TypeSymbol nonNullableType)
        {
            if (removeTopLevelNullability)
            {
                return StringifyCore(nonNullableType, null, strictness, visitedTypes);
            }
            else
            {
                return Nullableify(nonNullableType, strictness, visitedTypes);
            }
        }

        switch (type)
        {
            // Literal types - keep as is if strict
            case StringLiteralType
               or IntegerLiteralType
               or BooleanLiteralType
               when strictness == Strictness.Strict:
                return type.Name;
            // ... otherwise widen to simple type
            case StringLiteralType:
                return LanguageConstants.String.Name;
            case IntegerLiteralType:
                return LanguageConstants.Int.Name;
            case BooleanLiteralType:
                return LanguageConstants.Bool.Name;

            // Tuple types
            case TupleType tupleType:
                if (strictness == Strictness.Loose)
                {
                    return LanguageConstants.Array.Name;
                }
                else if (strictness == Strictness.Medium)
                {
                    var widenedTypes = tupleType.Items.Select(t => WidenType(t.Type, strictness)).ToArray();
                    var firstItemType = widenedTypes.FirstOrDefault()?.Type;
                    if (firstItemType == null)
                    {
                        // Empty tuple - use "array" to allow items
                        return LanguageConstants.Array.Name;
                    }
                    else if (widenedTypes.All(t => t.Type.Name == firstItemType.Name))
                    {
                        // Bicep infers a tuple type from literals such as "[1, 2]", turn these
                        // into the more likely intended int[] if all the members are of the same type
                        return Arrayify(widenedTypes[0], strictness, visitedTypes);
                    }
                }

                return $"[{string.Join(", ", tupleType.Items.Select(tt => StringifyCore(tt.Type, null, strictness, visitedTypes)))}]";

            // Typed arrays (e.g. int[])
            case TypedArrayType when strictness == Strictness.Loose:
                return LanguageConstants.Array.Name;
            case TypedArrayType typedArrayType:
                return Arrayify(typedArrayType.Item.Type, strictness, visitedTypes);

            // Plain old "array"
            case ArrayType:
                return LanguageConstants.Array.Name;

            // Nullable types are union types with one of the members being the null type
            case UnionType unionType:
                if (unionType.Members.Any(m => !IsLiteralType(m.Type)))
                {
                    // This handles the "open enum" type scenario (e.g. "type t = 'abc' | 'def' | string"),
                    //   which is supported by swagger but not by Bicep syntax.
                    // In this case, we will generate the widened type with a comment indicating the exact actual type,
                    //   e.g. "string /* 'abc' | 'def' | string */"
                    var widenedUnionType = WidenType(unionType, Strictness.Loose);
                    string widenedTypeString;
                    if (widenedUnionType != unionType)
                    {
                        if (TypeHelper.IsNullable(unionType))
                        {
                            widenedUnionType = TypeHelper.MakeNullable(widenedUnionType);
                        }
                        widenedTypeString = StringifyCore(widenedUnionType, null, Strictness.Loose, visitedTypes);
                    }
                    else
                    {
                        // Invalid or unsupported (CONSIDER: handle tagged union types)
                        widenedTypeString = TypeHelper.IsNullable(unionType) ? "object?" : "object";
                    }
                    return $"{widenedTypeString} /* {type.Name} */";
                }
                return type.Name;

            case BooleanType:
                return LanguageConstants.Bool.Name;
            case IntegerType:
                return LanguageConstants.Int.Name;
            case StringType:
                return LanguageConstants.String.Name;
            case NullType:
                return NullTypeName;

            case ObjectType objectType:
                if (strictness == Strictness.Loose)
                {
                    return LanguageConstants.Object.Name;
                }

                var writeableProperties = GetWriteableProperties(objectType);

                // strict: {} with additional properties allowed should be "object" not "{}"
                // medium: Bicep infers {} with no allowable members from the literal "{}", the user more likely wants to allow members
                if (writeableProperties.Length == 0 &&
                    (strictness == Strictness.Medium || !IsEmptyObjectLiteral(objectType)))
                {
                    return "object";
                }

                return $"{{ {string.Join(", ", writeableProperties
                        .Select(p => GetFormattedTypeProperty(p, strictness, visitedTypes)))} }}";

            case AnyType:
                return AnyTypeName;
            case ErrorType:
                return ErrorTypeName;

            // Anything else we don't know about (e.g. a type from a resource's swagger)
            default:
                return $"object? /* {type.Name} */";
        }
    }

    private static TypeSymbol WidenType(TypeSymbol type, Strictness strictness)
    {
        if (strictness == Strictness.Strict)
        {
            return type;
        }

        if (type is UnionType unionType && strictness == Strictness.Loose)
        {
            // Widen non-null members to a single type if they're all literal types of the same type, or null.
            // (If they're not all of the same literal type, either it's invalid or it's a tagged union.)
            var nonNullMembers = NonNullUnionMembers(unionType).ToArray();
            if (nonNullMembers.Select(m => WidenLiteralType(m).Name).Distinct().Count() == 1) // using "Name" comparison because we don't care about ValidationFlags
            {
                var widenedType = WidenLiteralType(nonNullMembers[0]);
                if (TypeHelper.IsNullable(unionType))
                {
                    // If it had "|null" before, add it back
                    widenedType = TypeHelper.MakeNullable(widenedType);
                }
                return widenedType;
            }
            else
            {
                return type;
            }
        }

        // ... otherwise (medium and loose) just widen literals to simple types
        return type switch
        {
            StringLiteralType => LanguageConstants.String,
            IntegerLiteralType => LanguageConstants.Int,
            BooleanLiteralType => LanguageConstants.Bool,
            _ => type,
        };
    }

    private static string Arrayify(TypeSymbol type, Strictness strictness, TypeSymbol[] visitedTypes)
    {
        string stringifiedType = StringifyCore(type, null, strictness, visitedTypes);
        bool needsParentheses = NeedsParentheses(type, strictness);

        return needsParentheses ? $"({stringifiedType})[]" : $"{stringifiedType}[]";
    }

    private static string Nullableify(TypeSymbol type, Strictness strictness, TypeSymbol[] visitedTypes)
    {
        string stringifiedType = StringifyCore(type, null, strictness, visitedTypes);
        bool needsParentheses = NeedsParentheses(type, strictness);

        return needsParentheses ? $"({stringifiedType})?" : $"{stringifiedType}?";
    }

    private static bool NeedsParentheses(TypeSymbol type, Strictness strictness)
    {
        // If the type is '1|2', with loose/medium, we need to check whether 'int' needs parentheses, not '1|2'
        // Therefore, widen first
        var widenedType = WidenType(type, strictness);
        bool needsParentheses = widenedType switch
        {
            UnionType { Members.Length: > 1 } => true, // also works for nullable types
            _ => false
        };
        return needsParentheses;
    }

    private static bool IsLiteralType(TypeSymbol type) => WidenLiteralType(type) != type;

    private static TypeSymbol WidenLiteralType(TypeSymbol type) => type switch
    {
        // Perhaps we should have a LiteralType class
        StringLiteralType => LanguageConstants.String,
        BooleanLiteralType => LanguageConstants.Bool,
        IntegerLiteralType => LanguageConstants.Int,
        _ => type
    };

    private static IEnumerable<TypeSymbol> NonNullUnionMembers(UnionType unionType) =>
        unionType.Members.Where(m => m.Type is not NullType).Select(tr => tr.Type);

    // True if "{}" (which allows no additional properties) instead of "object"
    private static bool IsEmptyObjectLiteral(ObjectType objectType)
    {
        return objectType.Properties.Count == 0 && !objectType.HasExplicitAdditionalPropertiesType;
    }

    private static NamedTypeProperty[] GetWriteableProperties(ObjectType objectType) =>
        objectType.Properties.Select(p => p.Value)
        .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly))
        .ToArray();

    private static string GetFormattedTypeProperty(NamedTypeProperty property, Strictness strictness, TypeSymbol[] visitedTypes)
    {
        return
            $"{StringUtils.EscapeBicepPropertyName(property.Name)}: {StringifyCore(property.TypeReference.Type, property, strictness, visitedTypes)}";
    }
}
