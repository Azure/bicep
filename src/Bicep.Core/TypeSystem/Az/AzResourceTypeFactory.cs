// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeFactory
    {
        private readonly ConcurrentDictionary<Azure.Bicep.Types.Concrete.TypeBase, TypeSymbol> typeCache;

        public AzResourceTypeFactory()
        {
            typeCache = new();
        }

        public ResourceTypeComponents GetResourceType(Azure.Bicep.Types.Concrete.ResourceType resourceType, IEnumerable<FunctionOverload> resourceFunctions)
        {
            var resourceTypeReference = ResourceTypeReference.Parse(resourceType.Name);
            var bodyType = GetTypeSymbol(resourceType.Body.Type, true);
            var assertsProperty = new TypeProperty(LanguageConstants.ResourceAssertPropertyName, AzResourceTypeProvider.ResourceAsserts);
            // DeclaredResourceMetadata.TypeReference.FormatType()
            if (bodyType is ObjectType objectType)
            {
                var properties = objectType.Properties.SetItem(LanguageConstants.ResourceAssertPropertyName, assertsProperty);
                if (resourceFunctions.Any())
                {
                    bodyType = new ObjectType(bodyType.Name, bodyType.ValidationFlags, properties.Values, objectType.AdditionalPropertiesType, objectType.AdditionalPropertiesFlags, resourceFunctions);
                }
                else
                {
                    bodyType = new ObjectType(bodyType.Name, bodyType.ValidationFlags, properties.Values, objectType.AdditionalPropertiesType, objectType.AdditionalPropertiesFlags);
                }
            }

            return new ResourceTypeComponents(resourceTypeReference, ToResourceScope(resourceType.ScopeType), ToResourceScope(resourceType.ReadOnlyScopes), ToResourceFlags(resourceType.Flags), bodyType);
        }

        public IEnumerable<FunctionOverload> GetResourceFunctionOverloads(Azure.Bicep.Types.Concrete.ResourceFunctionType resourceFunctionType)
        {
            yield return new FunctionOverloadBuilder(resourceFunctionType.Name)
                .WithReturnType(GetTypeSymbol(resourceFunctionType.Output.Type, false))
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            if (resourceFunctionType.Input is not null)
            {
                yield return new FunctionOverloadBuilder(resourceFunctionType.Name)
                    .WithRequiredParameter("apiVersion", TypeFactory.CreateStringLiteralType(resourceFunctionType.ApiVersion), "The api version")
                    .WithRequiredParameter("params", GetTypeSymbol(resourceFunctionType.Input.Type, false), $"{resourceFunctionType.Name} parameters")
                    .WithReturnType(GetTypeSymbol(resourceFunctionType.Output.Type, false))
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();
            }
        }

        private TypeSymbol GetTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase serializedType, bool isResourceBodyType)
            => typeCache.GetOrAdd(serializedType, serializedType => ToTypeSymbol(serializedType, isResourceBodyType));

        private ITypeReference GetTypeReference(Azure.Bicep.Types.Concrete.ITypeReference input)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type, false));

        private TypeProperty GetTypeProperty(string name, Azure.Bicep.Types.Concrete.ObjectTypeProperty input)
        {
            return new TypeProperty(name, GetTypeReference(input.Type), GetTypePropertyFlags(input), input.Description);
        }

        private static TypePropertyFlags GetTypePropertyFlags(Azure.Bicep.Types.Concrete.ObjectTypeProperty input)
        {
            var flags = TypePropertyFlags.None;

            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.Required))
            {
                flags |= TypePropertyFlags.Required;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.ReadOnly))
            {
                flags |= TypePropertyFlags.ReadOnly;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.WriteOnly))
            {
                flags |= TypePropertyFlags.WriteOnly;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.DeployTimeConstant))
            {
                flags |= TypePropertyFlags.DeployTimeConstant;
            }
            if (!input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.Required) && !input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.ReadOnly))
            {
                // for non-required and non-readonly resource properties, we allow null assignment
                flags |= TypePropertyFlags.AllowImplicitNull;
            }

            return flags;
        }

        private TypeSymbol ToTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase typeBase, bool isResourceBodyType)
        {
            switch (typeBase)
            {
                case Azure.Bicep.Types.Concrete.BuiltInType builtInType:
                    return builtInType.Kind switch
                    {
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Any => LanguageConstants.Any,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Null => LanguageConstants.Null,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Bool => LanguageConstants.Bool,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Int => LanguageConstants.Int,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.String => LanguageConstants.String,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Object => LanguageConstants.Object,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Array => LanguageConstants.Array,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.ResourceRef => LanguageConstants.ResourceRef,
                        _ => throw new ArgumentException(),
                    };
                case Azure.Bicep.Types.Concrete.ObjectType objectType:
                    {
                        var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties) : null;
                        var properties = objectType.Properties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value));

                        return new ObjectType(objectType.Name, GetValidationFlags(isResourceBodyType), properties, additionalProperties, TypePropertyFlags.None);
                    }
                case Azure.Bicep.Types.Concrete.ArrayType arrayType:
                    {
                        return new TypedArrayType(GetTypeReference(arrayType.ItemType), GetValidationFlags(isResourceBodyType));
                    }
                case Azure.Bicep.Types.Concrete.UnionType unionType:
                    {
                        return TypeHelper.CreateTypeUnion(unionType.Elements.Select(x => GetTypeReference(x)));
                    }
                case Azure.Bicep.Types.Concrete.StringLiteralType stringLiteralType:
                    return TypeFactory.CreateStringLiteralType(stringLiteralType.Value);
                case Azure.Bicep.Types.Concrete.DiscriminatedObjectType discriminatedObjectType:
                    {
                        var elementReferences = discriminatedObjectType.Elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(discriminatedObjectType.BaseProperties, kvp.Key, kvp.Value, isResourceBodyType)));

                        return new DiscriminatedObjectType(discriminatedObjectType.Name, GetValidationFlags(isResourceBodyType), discriminatedObjectType.Discriminator, elementReferences);
                    }
                default:
                    throw new ArgumentException();
            }
        }

        private ObjectType ToCombinedType(IEnumerable<KeyValuePair<string, Azure.Bicep.Types.Concrete.ObjectTypeProperty>> baseProperties, string name, Azure.Bicep.Types.Concrete.ITypeReference extendedType, bool isResourceBodyType)
        {
            if (!(extendedType.Type is Azure.Bicep.Types.Concrete.ObjectType objectType))
            {
                throw new ArgumentException();
            }

            var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties) : null;

            var extendedProperties = objectType.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var property in baseProperties.Where(x => !extendedProperties.ContainsKey(x.Key)))
            {
                extendedProperties[property.Key] = property.Value;
            }

            return new ObjectType(name, GetValidationFlags(isResourceBodyType), extendedProperties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value)), additionalProperties, TypePropertyFlags.None);
        }

        private static TypeSymbolValidationFlags GetValidationFlags(bool isResourceBodyType)
        {
            if (isResourceBodyType)
            {
                // strict validation on top-level resource properties, as 'custom' top-level properties are not supported by the platform
                return TypeSymbolValidationFlags.Default;
            }

            // in all other places, we should allow some wiggle room so that we don't block compilation if there are any swagger inaccuracies
            return TypeSymbolValidationFlags.WarnOnTypeMismatch;
        }

        private static ResourceFlags ToResourceFlags(Azure.Bicep.Types.Concrete.ResourceFlags input)
        {
            var output = ResourceFlags.None;
            if (input.HasFlag(Azure.Bicep.Types.Concrete.ResourceFlags.ReadOnly)) {
                output |= ResourceFlags.ReadOnly;
            }

            return output;
        }

        private static ResourceScope ToResourceScope(Azure.Bicep.Types.Concrete.ScopeType input)
        {
            if (input == Azure.Bicep.Types.Concrete.ScopeType.Unknown)
            {
                return ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource;
            }

            var output = ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Extension) ? ResourceScope.Resource : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Tenant) ? ResourceScope.Tenant : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.ManagementGroup) ? ResourceScope.ManagementGroup : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Subscription) ? ResourceScope.Subscription : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.ResourceGroup) ? ResourceScope.ResourceGroup : ResourceScope.None;

            return output;
        }

        private static ResourceScope ToResourceScope(Azure.Bicep.Types.Concrete.ScopeType? input)
            => input.HasValue ? ToResourceScope(input.Value) : ResourceScope.None;
    }
}
