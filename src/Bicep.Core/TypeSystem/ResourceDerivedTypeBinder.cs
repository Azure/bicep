// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.TypeSystem;

public class ResourceDerivedTypeBinder
{
    private readonly Stack<TypeSymbol> currentlyBinding = new();
    private readonly ConcurrentDictionary<TypeSymbol, TypeSymbol> boundTypes = new();
    private readonly Stack<TypeSymbol> currentlySearchingForUnboundTypes = new();
    private readonly ConcurrentDictionary<TypeSymbol, bool> containsUnboundTypesCache = new();
    private readonly IBinder binder;
    private readonly IDiagnosticWriter diagnostics;
    private readonly IPositionable diagnosticTarget;

    public ResourceDerivedTypeBinder(IBinder binder, IDiagnosticWriter diagnostics, IPositionable diagnosticTarget)
    {
        this.binder = binder;
        this.diagnostics = diagnostics;
        this.diagnosticTarget = diagnosticTarget;
    }

    public TypeSymbol BindResourceDerivedTypes(TypeSymbol unbound) => boundTypes.GetOrAdd(unbound, CalculateTypeBinding);

    private TypeSymbol CalculateTypeBinding(TypeSymbol unbound)
    {
        if (!ContainsUnboundTypes(unbound))
        {
            return unbound;
        }

        currentlyBinding.Push(unbound);

        var bound = unbound switch
        {
            IUnboundResourceDerivedType resourceDerivedType => CalculateTypeBinding(resourceDerivedType),
            TupleType tuple => CalculateTypeBinding(tuple),
            ArrayType array => CalculateTypeBinding(array),
            DiscriminatedObjectType taggedUnion => CalculateTypeBinding(taggedUnion),
            ObjectType @object => CalculateTypeBinding(@object),
            UnionType union => CalculateTypeBinding(union),
            TypeType typeType => CalculateTypeBinding(typeType),
            _ when IsPrimitiveType(unbound) => unbound,
            _ => throw new UnreachableException($"Unexpected type ({unbound.GetType().FullName}) encountered"),
        };

        currentlyBinding.Pop();

        return bound;
    }

    private TypeSymbol CalculateTypeBinding(IUnboundResourceDerivedType unbound)
    {
        // TODO support types derived from resources other than the `az` provider. This will require some refactoring of how provider artifacts are restored
        var bound = binder.NamespaceResolver.GetMatchingResourceTypes(unbound.TypeReference, ResourceTypeGenerationFlags.None)
            .Where(resourceType => LanguageConstants.IdentifierComparer.Equals(resourceType.DeclaringNamespace.ProviderName, AzNamespaceType.BuiltInName))
            .FirstOrDefault();

        if (bound is null)
        {
            diagnostics.Write(DiagnosticBuilder.ForPosition(diagnosticTarget).ResourceTypesUnavailable(unbound.TypeReference));
            return unbound.FallbackType;
        }

        return bound.Body.Type;
    }

    private TupleType CalculateTypeBinding(TupleType unbound)
    {
        var boundItemTypes = ImmutableArray.CreateBuilder<ITypeReference>(unbound.Items.Length);
        var hasChanges = false;

        for (int i = 0; i < unbound.Items.Length; i++)
        {
            var unboundItemType = unbound.Items[i].Type;
            if (currentlyBinding.Contains(unboundItemType))
            {
                boundItemTypes.Add(new DeferredTypeReference(() => boundTypes[unboundItemType]));
                hasChanges = hasChanges || ContainsUnboundTypes(unboundItemType);;
            }
            else
            {
                boundItemTypes.Add(BindResourceDerivedTypes(unboundItemType));
                hasChanges = hasChanges || !ReferenceEquals(boundItemTypes[i].Type, unboundItemType);
            }
        }

        return hasChanges
            ? new(unbound.Name, boundItemTypes.ToImmutable(), unbound.ValidationFlags)
            : unbound;
    }

    private ArrayType CalculateTypeBinding(ArrayType unbound)
    {
        var unboundItemType = unbound.Item.Type;
        if (currentlyBinding.Contains(unboundItemType))
        {
            return new TypedArrayType(unbound.Name,
                new DeferredTypeReference(() => boundTypes[unboundItemType]),
                unbound.ValidationFlags,
                unbound.MinLength,
                unbound.MaxLength);
        }

        var boundItemType = BindResourceDerivedTypes(unbound.Item.Type);

        return ReferenceEquals(boundItemType, unbound.Item.Type)
            ? unbound
            : new TypedArrayType(unbound.Name, boundItemType, unbound.ValidationFlags, unbound.MinLength, unbound.MaxLength);
    }

    private DiscriminatedObjectType CalculateTypeBinding(DiscriminatedObjectType unbound)
    {
        var boundUnionMembers = ImmutableArray.CreateBuilder<ITypeReference>(unbound.UnionMembersByKey.Count);
        var hasChanges = false;

        foreach (var unboundMember in unbound.UnionMembersByKey.Values)
        {
            var boundMember = BindResourceDerivedTypes(unboundMember.Type);
            hasChanges = hasChanges || !ReferenceEquals(boundMember, unboundMember.Type);
            boundUnionMembers.Add(boundMember);
        }

        return hasChanges
            ? new(unbound.Name, unbound.ValidationFlags, unbound.DiscriminatorKey, boundUnionMembers.ToImmutable())
            : unbound;
    }

    private ObjectType CalculateTypeBinding(ObjectType unbound)
    {
        var boundProperties = ImmutableArray.CreateBuilder<TypeProperty>(unbound.Properties.Count);
        var hasChanges = false;

        foreach (var unboundProperty in unbound.Properties.Values)
        {
            var unboundPropertyType = unboundProperty.TypeReference.Type;
            if (currentlyBinding.Contains(unboundPropertyType))
            {
                boundProperties.Add(new(unboundProperty.Name,
                    new DeferredTypeReference(() => boundTypes[unboundPropertyType]),
                    unboundProperty.Flags,
                    unboundProperty.Description));
                hasChanges = hasChanges || ContainsUnboundTypes(unboundPropertyType);
            }
            else
            {
                var boundPropertyType = BindResourceDerivedTypes(unboundPropertyType);
                boundProperties.Add(new(unboundProperty.Name,
                    boundPropertyType,
                    unboundProperty.Flags,
                    unboundProperty.Description));
                hasChanges = hasChanges || !ReferenceEquals(unboundPropertyType, boundPropertyType);
            }
        }

        var addlPropertiesType = unbound.AdditionalPropertiesType;
        if (addlPropertiesType is not null)
        {
            var boundAddlPropertiesType = BindResourceDerivedTypes(addlPropertiesType.Type);
            hasChanges = hasChanges || !ReferenceEquals(addlPropertiesType.Type, boundAddlPropertiesType);
            addlPropertiesType = boundAddlPropertiesType;
        }

        return hasChanges
            ? new(unbound.Name, unbound.ValidationFlags, boundProperties.ToImmutable(), addlPropertiesType, unbound.AdditionalPropertiesFlags)
            : unbound;
    }

    private UnionType CalculateTypeBinding(UnionType unbound)
    {
        var boundMembers = ImmutableArray.CreateBuilder<ITypeReference>(unbound.Members.Length);
        var hasChanges = false;

        for (int i = 0; i < unbound.Members.Length; i++)
        {
            var unboundMemberType = unbound.Members[i].Type;
            if (currentlyBinding.Contains(unboundMemberType))
            {
                boundMembers.Add(new DeferredTypeReference(() => boundTypes[unboundMemberType]));
                hasChanges = hasChanges || ContainsUnboundTypes(unboundMemberType);;
            }
            else
            {
                boundMembers.Add(BindResourceDerivedTypes(unboundMemberType));
                hasChanges = hasChanges || !ReferenceEquals(boundMembers[i].Type, unboundMemberType);
            }
        }

        return hasChanges
            ? new(unbound.Name, boundMembers.ToImmutable())
            : unbound;
    }

    private TypeType CalculateTypeBinding(TypeType unbound)
    {
        var boundUnwrapped = BindResourceDerivedTypes(unbound.Unwrapped);

        return ReferenceEquals(boundUnwrapped, unbound.Unwrapped)
            ? unbound
            : new(boundUnwrapped);
    }

    private bool ContainsUnboundTypes(TypeSymbol type)
    {
        if (currentlySearchingForUnboundTypes.Contains(type))
        {
            // types may be recursive, so cut out early if we hit a recursion point
            return false;
        }

        return containsUnboundTypesCache.GetOrAdd(type, type =>
        {
            currentlySearchingForUnboundTypes.Push(type);

            var containsUnboundTypes = type switch
            {
                IUnboundResourceDerivedType => true,
                TupleType tuple => ContainsUnboundTypes(tuple),
                ArrayType array => ContainsUnboundTypes(array),
                DiscriminatedObjectType taggedUnion => ContainsUnboundTypes(taggedUnion),
                ObjectType @object => ContainsUnboundTypes(@object),
                UnionType union => ContainsUnboundTypes(union),
                TypeType typeType => ContainsUnboundTypes(typeType),
                _ when IsPrimitiveType(type) => false,
                _ => throw new UnreachableException($"Unexpected type ({type.GetType().FullName}) encountered"),
            };

            currentlySearchingForUnboundTypes.Pop();

            return containsUnboundTypes;
        });
    }

    private bool ContainsUnboundTypes(TupleType tuple)
        => tuple.Items.Any(item => ContainsUnboundTypes(item.Type));

    private bool ContainsUnboundTypes(ArrayType array)
        => ContainsUnboundTypes(array.Item.Type);

    private bool ContainsUnboundTypes(DiscriminatedObjectType discriminatedObject)
        => discriminatedObject.UnionMembersByKey.Values.Any(variant => ContainsUnboundTypes(variant as TypeSymbol));

    private bool ContainsUnboundTypes(ObjectType objectType)
        => objectType.Properties.Values.Any(property => ContainsUnboundTypes(property.TypeReference.Type)) ||
            (objectType.AdditionalPropertiesType?.Type is TypeSymbol addlPropertiesType && ContainsUnboundTypes(addlPropertiesType));

    private bool ContainsUnboundTypes(UnionType union)
        => union.Members.Any(member => ContainsUnboundTypes(member.Type));

    private bool ContainsUnboundTypes(TypeType typeType)
        => ContainsUnboundTypes(typeType.Unwrapped);

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
