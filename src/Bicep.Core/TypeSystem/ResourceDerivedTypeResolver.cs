// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public class ResourceDerivedTypeResolver
{
    private static readonly StringComparer PointerSegmentComparer = StringComparer.OrdinalIgnoreCase;

    private readonly Stack<TypeSymbol> currentlyResolving = new();
    private readonly ConcurrentDictionary<TypeSymbol, TypeSymbol> resolvedTypes = new();
    private readonly Stack<TypeSymbol> currentlySearchingForUnresolvedTypes = new();
    private readonly ConcurrentDictionary<TypeSymbol, bool> containsUnresolvedTypesCache = new();
    private readonly IBinder binder;

    public ResourceDerivedTypeResolver(IBinder binder)
    {
        this.binder = binder;
    }

    public TypeSymbol ResolveResourceDerivedTypes(TypeSymbol potentiallyUnresolved)
        => resolvedTypes.GetOrAdd(potentiallyUnresolved, ResolveType);

    private TypeSymbol ResolveType(TypeSymbol potentiallyUnresolved)
    {
        if (!ContainsUnresolvedTypes(potentiallyUnresolved))
        {
            return potentiallyUnresolved;
        }

        currentlyResolving.Push(potentiallyUnresolved);

        var bound = potentiallyUnresolved switch
        {
            IUnresolvedResourceDerivedType resourceDerivedType => ResolveType(resourceDerivedType),
            TupleType tuple => ResolveType(tuple),
            ArrayType array => ResolveType(array),
            DiscriminatedObjectType taggedUnion => ResolveType(taggedUnion),
            ObjectType @object => ResolveType(@object),
            UnionType union => ResolveType(union),
            TypeType typeType => ResolveType(typeType),
            LambdaType lambda => ResolveType(lambda),
            _ when IsPrimitiveType(potentiallyUnresolved) => potentiallyUnresolved,
            _ => throw new UnreachableException($"Unexpected type ({potentiallyUnresolved.GetType().FullName}) encountered"),
        };

        currentlyResolving.Pop();

        return bound;
    }

    private TypeSymbol ResolveType(IUnresolvedResourceDerivedType unresolved)
    {
        // TODO support types derived from resources other than the `az` extension. This will require some refactoring of how extension artifacts are restored
        if (binder.NamespaceResolver.GetMatchingResourceTypes(unresolved.TypeReference, ResourceTypeGenerationFlags.None)
            .Where(resourceType => LanguageConstants.IdentifierComparer.Equals(resourceType.DeclaringNamespace.ExtensionName, AzNamespaceType.BuiltInName))
            .FirstOrDefault()
            ?.Body.Type is TypeSymbol bodyType)
        {
            var current = bodyType;
            for (int i = 0; i < unresolved.PointerSegments.Length; i++)
            {
                switch (unresolved.PointerSegments[i].ToLowerInvariant())
                {
                    case "properties" when current is ObjectType @object &&
                        TryGetNamedProperty(@object, unresolved.PointerSegments[++i]) is TypeProperty namedProperty:
                        current = namedProperty.TypeReference.Type;
                        continue;
                    case "additionalproperties" when current is ObjectType @object &&
                        @object.AdditionalProperties?.TypeReference.Type is { } additionalPropertiesType:
                        current = additionalPropertiesType;
                        continue;
                    case "discriminator" when current is DiscriminatedObjectType discriminatedObject &&
                        unresolved.PointerSegments[++i].Equals("mapping", StringComparison.OrdinalIgnoreCase) &&
                        discriminatedObject.UnionMembersByKey.TryGetValue(StringUtils.EscapeBicepString(unresolved.PointerSegments[++i]), out var variant):
                        current = variant;
                        continue;
                    case "prefixitems" when current is TupleType tuple &&
                        int.TryParse(unresolved.PointerSegments[++i], out int index) &&
                        0 <= index &&
                        index < tuple.Items.Length:
                        current = tuple.Items[index].Type;
                        continue;
                    case "items" when current is ArrayType array:
                        current = array.Item.Type;
                        continue;
                }

                return unresolved.FallbackType;
            }

            return unresolved.Variant switch
            {
                ResourceDerivedTypeVariant.Input => TypeHelper.RemovePropertyFlagsRecursively(current, TypePropertyFlags.WriteOnly),
                ResourceDerivedTypeVariant.Output => TypeHelper.RemovePropertyFlagsRecursively(current, TypePropertyFlags.ReadOnly),
                _ => current,
            };
        }

        return unresolved.FallbackType;
    }

    private static TypeProperty? TryGetNamedProperty(ObjectType @object, string propertyName)
    {
        if (@object.Properties.TryGetValue(propertyName, out var property))
        {
            return property;
        }

        if (@object.Properties.Where(p => PointerSegmentComparer.Equals(propertyName, p.Key)).FirstOrDefault() is { } caseInsensitiveMatch)
        {
            return caseInsensitiveMatch.Value;
        }

        return null;
    }

    private TupleType ResolveType(TupleType unresolved)
    {
        var resolvedItemTypes = ImmutableArray.CreateBuilder<ITypeReference>(unresolved.Items.Length);
        var hasChanges = false;

        for (int i = 0; i < unresolved.Items.Length; i++)
        {
            var unresolvedItemType = unresolved.Items[i].Type;
            if (currentlyResolving.Contains(unresolvedItemType))
            {
                resolvedItemTypes.Add(new DeferredTypeReference(() => resolvedTypes[unresolvedItemType]));
                hasChanges = hasChanges || ContainsUnresolvedTypes(unresolvedItemType); ;
            }
            else
            {
                resolvedItemTypes.Add(ResolveResourceDerivedTypes(unresolvedItemType));
                hasChanges = hasChanges || !ReferenceEquals(resolvedItemTypes[i].Type, unresolvedItemType);
            }
        }

        return hasChanges
            ? new(unresolved.Name, resolvedItemTypes.ToImmutable(), unresolved.ValidationFlags)
            : unresolved;
    }

    private ArrayType ResolveType(ArrayType unresolved)
    {
        var unresolvedItemType = unresolved.Item.Type;
        if (currentlyResolving.Contains(unresolvedItemType))
        {
            return new TypedArrayType(unresolved.Name,
                new DeferredTypeReference(() => resolvedTypes[unresolvedItemType]),
                unresolved.ValidationFlags,
                unresolved.MinLength,
                unresolved.MaxLength);
        }

        var resolvedItemType = ResolveResourceDerivedTypes(unresolved.Item.Type);

        return ReferenceEquals(resolvedItemType, unresolved.Item.Type)
            ? unresolved
            : new TypedArrayType(unresolved.Name, resolvedItemType, unresolved.ValidationFlags, unresolved.MinLength, unresolved.MaxLength);
    }

    private DiscriminatedObjectType ResolveType(DiscriminatedObjectType unresolved)
    {
        var resolvedUnionMembers = ImmutableArray.CreateBuilder<ITypeReference>(unresolved.UnionMembersByKey.Count);
        var hasChanges = false;

        foreach (var unresolvedMember in unresolved.UnionMembersByKey.Values)
        {
            var resolvedMember = ResolveResourceDerivedTypes(unresolvedMember.Type);
            hasChanges = hasChanges || !ReferenceEquals(resolvedMember, unresolvedMember.Type);
            resolvedUnionMembers.Add(resolvedMember);
        }

        return hasChanges
            ? new(unresolved.Name, unresolved.ValidationFlags, unresolved.DiscriminatorKey, resolvedUnionMembers.ToImmutable())
            : unresolved;
    }

    private ObjectType ResolveType(ObjectType unresolved)
    {
        var resolvedProperties = ImmutableArray.CreateBuilder<NamedTypeProperty>(unresolved.Properties.Count);
        var hasChanges = false;

        foreach (var unresolvedProperty in unresolved.Properties.Values)
        {
            var unboundPropertyType = unresolvedProperty.TypeReference.Type;
            if (currentlyResolving.Contains(unboundPropertyType))
            {
                resolvedProperties.Add(new(unresolvedProperty.Name,
                    new DeferredTypeReference(() => resolvedTypes[unboundPropertyType]),
                    unresolvedProperty.Flags,
                    unresolvedProperty.Description));
                hasChanges = hasChanges || ContainsUnresolvedTypes(unboundPropertyType);
            }
            else
            {
                var resolvedPropertyType = ResolveResourceDerivedTypes(unboundPropertyType);
                resolvedProperties.Add(new(unresolvedProperty.Name,
                    resolvedPropertyType,
                    unresolvedProperty.Flags,
                    unresolvedProperty.Description));
                hasChanges = hasChanges || !ReferenceEquals(unboundPropertyType, resolvedPropertyType);
            }
        }

        var addlPropertiesType = unresolved.AdditionalProperties?.TypeReference.Type;
        if (addlPropertiesType is not null)
        {
            var resolvedAddlPropertiesType = ResolveResourceDerivedTypes(addlPropertiesType.Type);
            hasChanges = hasChanges || !ReferenceEquals(addlPropertiesType.Type, resolvedAddlPropertiesType);
            addlPropertiesType = resolvedAddlPropertiesType;
        }

        return hasChanges
            ? new(
                unresolved.Name,
                unresolved.ValidationFlags,
                resolvedProperties.ToImmutable(),
                addlPropertiesType is not null && unresolved.AdditionalProperties is { } additionalProperties
                    ? new TypeProperty(addlPropertiesType, additionalProperties.Flags, additionalProperties.Description)
                    : null)
            : unresolved;
    }

    private UnionType ResolveType(UnionType unresolved)
    {
        var resolvedMembers = ImmutableArray.CreateBuilder<ITypeReference>(unresolved.Members.Length);
        var hasChanges = false;

        for (int i = 0; i < unresolved.Members.Length; i++)
        {
            var unboundMemberType = unresolved.Members[i].Type;
            if (currentlyResolving.Contains(unboundMemberType))
            {
                resolvedMembers.Add(new DeferredTypeReference(() => resolvedTypes[unboundMemberType]));
                hasChanges = hasChanges || ContainsUnresolvedTypes(unboundMemberType); ;
            }
            else
            {
                resolvedMembers.Add(ResolveResourceDerivedTypes(unboundMemberType));
                hasChanges = hasChanges || !ReferenceEquals(resolvedMembers[i].Type, unboundMemberType);
            }
        }

        return hasChanges
            ? new(unresolved.Name, resolvedMembers.ToImmutable())
            : unresolved;
    }

    private TypeType ResolveType(TypeType unresolved)
    {
        var resolvedUnwrapped = ResolveResourceDerivedTypes(unresolved.Unwrapped);

        return ReferenceEquals(resolvedUnwrapped, unresolved.Unwrapped)
            ? unresolved
            : new(resolvedUnwrapped);
    }

    private LambdaType ResolveType(LambdaType unresolved)
    {
        var resolvedParameterTypes = ImmutableArray.CreateBuilder<ITypeReference>(unresolved.ArgumentTypes.Length);
        var hasChanges = false;

        for (int i = 0; i < unresolved.MaximumArgCount; i++)
        {
            var unboundParameterType = unresolved.GetArgumentType(i).Type;

            if (currentlyResolving.Contains(unboundParameterType))
            {
                resolvedParameterTypes.Add(new DeferredTypeReference(() => resolvedTypes[unboundParameterType]));
                hasChanges = hasChanges || ContainsUnresolvedTypes(unboundParameterType);
            }
            else
            {
                resolvedParameterTypes.Add(ResolveResourceDerivedTypes(unboundParameterType));
                hasChanges = hasChanges || !ReferenceEquals(resolvedParameterTypes[i].Type, unboundParameterType);
            }
        }

        var resolvedReturnType = ResolveResourceDerivedTypes(unresolved.ReturnType.Type);
        hasChanges = hasChanges || !ReferenceEquals(resolvedReturnType, unresolved.ReturnType.Type);

        return hasChanges
            ? new(
                [.. resolvedParameterTypes.Take(unresolved.ArgumentTypes.Length)],
                [.. resolvedParameterTypes.Skip(unresolved.ArgumentTypes.Length)],
                resolvedReturnType)
            : unresolved;
    }

    private bool ContainsUnresolvedTypes(TypeSymbol type)
    {
        if (currentlySearchingForUnresolvedTypes.Contains(type))
        {
            // types may be recursive, so cut out early if we hit a recursion point
            return false;
        }

        return containsUnresolvedTypesCache.GetOrAdd(type, type =>
        {
            currentlySearchingForUnresolvedTypes.Push(type);

            var containsUnresolvedTypes = type switch
            {
                IUnresolvedResourceDerivedType => true,
                TupleType tuple => ContainsUnboundTypes(tuple),
                ArrayType array => ContainsUnboundTypes(array),
                DiscriminatedObjectType taggedUnion => ContainsUnboundTypes(taggedUnion),
                ObjectType @object => ContainsUnboundTypes(@object),
                UnionType union => ContainsUnboundTypes(union),
                TypeType typeType => ContainsUnboundTypes(typeType),
                LambdaType lambda => ContainsUnboundTypes(lambda),
                ResourceType or ModuleType or ResourceScopeType or ResourceParentType => false,
                _ when IsPrimitiveType(type) => false,
                _ => throw new UnreachableException($"Unexpected type ({type.GetType().FullName}) encountered"),
            };

            currentlySearchingForUnresolvedTypes.Pop();

            return containsUnresolvedTypes;
        });
    }

    private bool ContainsUnboundTypes(TupleType tuple)
        => tuple.Items.Any(item => ContainsUnresolvedTypes(item.Type));

    private bool ContainsUnboundTypes(ArrayType array)
        => ContainsUnresolvedTypes(array.Item.Type);

    private bool ContainsUnboundTypes(DiscriminatedObjectType discriminatedObject)
        => discriminatedObject.UnionMembersByKey.Values.Any(variant => ContainsUnresolvedTypes(variant as TypeSymbol));

    private bool ContainsUnboundTypes(ObjectType objectType)
        => objectType.Properties.Values.Any(property => ContainsUnresolvedTypes(property.TypeReference.Type)) ||
            (objectType.AdditionalProperties?.TypeReference.Type is TypeSymbol addlPropertiesType && ContainsUnresolvedTypes(addlPropertiesType));

    private bool ContainsUnboundTypes(UnionType union)
        => union.Members.Any(member => ContainsUnresolvedTypes(member.Type));

    private bool ContainsUnboundTypes(TypeType typeType)
        => ContainsUnresolvedTypes(typeType.Unwrapped);

    private bool ContainsUnboundTypes(LambdaType lambda)
        => lambda.ArgumentTypes.Any(argType => ContainsUnresolvedTypes(argType.Type)) ||
            ContainsUnresolvedTypes(lambda.ReturnType.Type);

    private static bool IsPrimitiveType(TypeSymbol type) => type is AnyType or
        BooleanLiteralType or
        BooleanType or
        ErrorType or
        IntegerLiteralType or
        IntegerType or
        NullType or
        StringLiteralType or
        StringType;
}
