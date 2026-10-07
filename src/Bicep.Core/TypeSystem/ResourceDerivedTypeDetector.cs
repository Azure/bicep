// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

/// <summary>
/// Determines whether a type expression resolves to a resource-derived type, following type aliases, compile-time imports,
/// and property, index, additional properties, and items accessors.
/// </summary>
public static class ResourceDerivedTypeDetector
{
    private abstract record Target;

    private record ResourceDerivedTarget : Target
    {
        public static readonly ResourceDerivedTarget Instance = new();
    }

    private record SyntaxTarget(SyntaxBase Syntax, IBinder Binder, ITypeManager TypeManager) : Target;

    private record TypeSymbolTarget(TypeSymbol Type) : Target;

    private abstract record Accessor;

    private record PropertyAccessor(string PropertyName) : Accessor;

    private record IndexAccessor(long Index) : Accessor;

    private record AdditionalPropertiesAccessor : Accessor
    {
        public static readonly AdditionalPropertiesAccessor Instance = new();
    }

    private record ItemsAccessor : Accessor
    {
        public static readonly ItemsAccessor Instance = new();
    }

    public static bool IsResourceDerivedType(SyntaxBase? typeSyntax, IBinder binder, ITypeManager typeManager)
        => typeSyntax is not null && Resolve(typeSyntax, binder, typeManager, []) is ResourceDerivedTarget;

    private static Target? Resolve(SyntaxBase syntax, IBinder binder, ITypeManager typeManager, HashSet<TypeAliasSymbol> visited) => syntax switch
    {
        NullableTypeSyntax nullable => Resolve(nullable.Base, binder, typeManager, visited),
        NonNullableTypeSyntax nonNullable => Resolve(nonNullable.Base, binder, typeManager, visited),
        ParenthesizedTypeSyntax parenthesized => Resolve(parenthesized.Expression, binder, typeManager, visited),
        ParameterizedTypeInstantiationSyntaxBase parameterized => typeManager.TryGetReifiedType(parameterized) is ResourceDerivedTypeExpression
            ? ResourceDerivedTarget.Instance
            : null,
        TypeVariableAccessSyntax variableAccess => binder.GetSymbolInfo(variableAccess) switch
        {
            TypeAliasSymbol alias when visited.Add(alias) => Resolve(alias.Value, binder, typeManager, visited),
            ImportedTypeSymbol imported when imported.OriginalSymbolName is { } originalName => ResolveExport(imported.SourceModel, originalName, visited),
            _ => null,
        },
        TypePropertyAccessSyntax propertyAccess => ResolvePropertyAccess(propertyAccess.BaseExpression, propertyAccess.PropertyName.IdentifierName, binder, typeManager, visited),
        TypeArrayAccessSyntax arrayAccess => typeManager.GetTypeInfo(arrayAccess.IndexExpression) switch
        {
            StringLiteralType @string => ResolvePropertyAccess(arrayAccess.BaseExpression, @string.RawStringValue, binder, typeManager, visited),
            IntegerLiteralType @int => Access(Resolve(arrayAccess.BaseExpression, binder, typeManager, visited), new IndexAccessor(@int.Value), visited),
            _ => null,
        },
        TypeAdditionalPropertiesAccessSyntax additionalPropertiesAccess
            => Access(Resolve(additionalPropertiesAccess.BaseExpression, binder, typeManager, visited), AdditionalPropertiesAccessor.Instance, visited),
        TypeItemsAccessSyntax itemsAccess
            => Access(Resolve(itemsAccess.BaseExpression, binder, typeManager, visited), ItemsAccessor.Instance, visited),
        _ => new SyntaxTarget(syntax, binder, typeManager),
    };

    private static Target? ResolvePropertyAccess(SyntaxBase baseExpression, string propertyName, IBinder binder, ITypeManager typeManager, HashSet<TypeAliasSymbol> visited)
        => binder.GetSymbolInfo(baseExpression) is WildcardImportSymbol wildcardImport
            ? ResolveExport(wildcardImport.SourceModel, propertyName, visited)
            : Access(Resolve(baseExpression, binder, typeManager, visited), new PropertyAccessor(propertyName), visited);

    private static Target? ResolveExport(ISemanticModel sourceModel, string exportName, HashSet<TypeAliasSymbol> visited)
    {
        if (sourceModel is SemanticModel bicepModel)
        {
            return bicepModel.Root.TypeDeclarations.FirstOrDefault(t => LanguageConstants.IdentifierComparer.Equals(t.Name, exportName)) is { } alias && visited.Add(alias)
                ? Resolve(alias.Value, bicepModel.Binder, bicepModel.TypeManager, visited)
                : null;
        }

        // Types exported from ARM templates carry resource-derived type markers until resolved by the importing model.
        return sourceModel.Exports.TryGetValue(exportName, out var export) && export is ExportedTypeMetadata
            ? FromTypeReference(export.TypeReference)
            : null;
    }

    private static Target? Access(Target? target, Accessor accessor, HashSet<TypeAliasSymbol> visited) => target switch
    {
        ResourceDerivedTarget => target,
        SyntaxTarget syntaxTarget => AccessSyntax(syntaxTarget, accessor, visited),
        TypeSymbolTarget typeTarget => AccessType(typeTarget.Type, accessor),
        _ => null,
    };

    private static Target? AccessSyntax(SyntaxTarget target, Accessor accessor, HashSet<TypeAliasSymbol> visited)
    {
        var memberSyntax = (target.Syntax, accessor) switch
        {
            (ObjectTypeSyntax @object, PropertyAccessor property)
                => @object.Properties.FirstOrDefault(p => LanguageConstants.IdentifierComparer.Equals(p.TryGetKeyText(), property.PropertyName))?.Value,
            (ObjectTypeSyntax @object, AdditionalPropertiesAccessor) => @object.AdditionalProperties?.Value,
            (TupleTypeSyntax tuple, IndexAccessor index) when index.Index >= 0 && index.Index <= int.MaxValue
                => tuple.Items.ElementAtOrDefault((int)index.Index)?.Value,
            (ArrayTypeSyntax array, ItemsAccessor) => array.Item.Value,
            _ => null,
        };

        return memberSyntax is not null ? Resolve(memberSyntax, target.Binder, target.TypeManager, visited) : null;
    }

    private static Target? AccessType(TypeSymbol type, Accessor accessor)
    {
        ITypeReference? member = (TypeHelper.TryRemoveNullability(type) ?? type, accessor) switch
        {
            (ObjectType @object, PropertyAccessor property) => @object.Properties.TryGetValue(property.PropertyName, out var namedProperty)
                ? namedProperty.TypeReference
                : null,
            (ObjectType @object, AdditionalPropertiesAccessor) => @object.AdditionalProperties?.TypeReference,
            (TupleType tuple, IndexAccessor index) when index.Index >= 0 && index.Index < tuple.Items.Length => tuple.Items[(int)index.Index],
            (ArrayType array, ItemsAccessor) => array.Item,
            _ => null,
        };

        return member is not null ? FromTypeReference(member) : null;
    }

    private static Target FromTypeReference(ITypeReference typeReference)
    {
        if (typeReference is UnparsableResourceDerivedType)
        {
            return ResourceDerivedTarget.Instance;
        }

        var type = typeReference.Type is TypeType typeType ? typeType.Unwrapped : typeReference.Type;

        return (TypeHelper.TryRemoveNullability(type) ?? type) is IUnresolvedResourceDerivedType
            ? ResourceDerivedTarget.Instance
            : new TypeSymbolTarget(type);
    }
}
