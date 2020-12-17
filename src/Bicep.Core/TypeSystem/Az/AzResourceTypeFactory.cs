// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeFactory
    {
        private readonly Dictionary<Azure.Bicep.Types.Concrete.TypeBase, TypeSymbol> typeCache;

        public AzResourceTypeFactory()
        {
            typeCache = new Dictionary<Azure.Bicep.Types.Concrete.TypeBase, TypeSymbol>();
        }

        public ResourceType GetResourceType(Azure.Bicep.Types.Concrete.ResourceType resourceType)
        {
            var output = GetTypeSymbol(resourceType, false) as ResourceType;

            return output ?? throw new ArgumentException("Unable to deserialize resource type", nameof(resourceType));
        }

        private TypeSymbol GetTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase serializedType, bool isResourceBodyType)
        {
            if (!typeCache.TryGetValue(serializedType, out var typeSymbol))
            {
                typeSymbol = ToTypeSymbol(serializedType, isResourceBodyType);
                typeCache[serializedType] = typeSymbol;
            }

            return typeSymbol;
        }

        private ITypeReference GetTypeReference(Azure.Bicep.Types.Concrete.ITypeReference input)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type, false));

        private TypeProperty GetTypeProperty(string name, Azure.Bicep.Types.Concrete.ObjectProperty input)
        {
            return new TypeProperty(name, GetTypeReference(input.Type), GetTypePropertyFlags(input));
        }

        private static TypePropertyFlags GetTypePropertyFlags(Azure.Bicep.Types.Concrete.ObjectProperty input)
        {
            var flags = TypePropertyFlags.None;

            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectPropertyFlags.Required))
            {
                flags |= TypePropertyFlags.Required;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectPropertyFlags.ReadOnly))
            {
                flags |= TypePropertyFlags.ReadOnly;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectPropertyFlags.WriteOnly))
            {
                flags |= TypePropertyFlags.WriteOnly;
            }
            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectPropertyFlags.DeployTimeConstant))
            {
                flags |= TypePropertyFlags.SkipInlining;
            }

            return flags;
        }

        private static ObjectType AddBicepResourceProperties(ObjectType objectType, Azure.Bicep.Types.Concrete.ScopeType scope)
        {
            var properties = objectType.Properties.Values.ToList();
            if (scope.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Extension) || scope == Azure.Bicep.Types.Concrete.ScopeType.Unknown)
            {
                var scopeRequiredFlag = (scope == Azure.Bicep.Types.Concrete.ScopeType.Extension) ? TypePropertyFlags.Required : TypePropertyFlags.None;
                properties.Add(new TypeProperty("scope", new ResourceScopeReference("resource", ResourceScopeType.ResourceScope), TypePropertyFlags.WriteOnly | scopeRequiredFlag));
            }

            // TODO: remove 'dependsOn' from the type library and add it here

            return new NamedObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                properties,
                objectType.AdditionalPropertiesType,
                objectType.AdditionalPropertiesFlags);
        }

        private TypeSymbol ToTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase typeBase, bool isResourceBodyType)
        {
            switch (typeBase)
            {
                case Azure.Bicep.Types.Concrete.BuiltInType builtInType:
                    return builtInType.Kind switch {
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

                    return new NamedObjectType(objectType.Name, GetValidationFlags(isResourceBodyType), properties, additionalProperties, TypePropertyFlags.None);
                }
                case Azure.Bicep.Types.Concrete.ArrayType arrayType:
                {
                    return new TypedArrayType(GetTypeReference(arrayType.ItemType), GetValidationFlags(isResourceBodyType));
                }
                case Azure.Bicep.Types.Concrete.ResourceType resourceType:
                {
                    var resourceTypeReference = ResourceTypeReference.Parse(resourceType.Name);
                    var bodyType = GetTypeSymbol(resourceType.Body.Type, true);

                    switch (bodyType)
                    {
                        case ObjectType bodyObjectType:
                            bodyType = AddBicepResourceProperties(bodyObjectType, resourceType.ScopeType);
                            break;
                        case DiscriminatedObjectType bodyDiscriminatedType:
                            var bodyTypes = bodyDiscriminatedType.UnionMembersByKey.Values.ToList().Select(x => x.Type as ObjectType ?? throw new ArgumentException());
                            bodyTypes = bodyTypes.Select(x => AddBicepResourceProperties(x, resourceType.ScopeType));
                            bodyType = new DiscriminatedObjectType(
                                bodyDiscriminatedType.Name,
                                bodyDiscriminatedType.ValidationFlags,
                                bodyDiscriminatedType.DiscriminatorKey,
                                bodyTypes);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    return new ResourceType(resourceTypeReference, bodyType);
                }
                case Azure.Bicep.Types.Concrete.UnionType unionType:
                {
                    return UnionType.Create(unionType.Elements.Select(x => GetTypeReference(x)));
                }
                case Azure.Bicep.Types.Concrete.StringLiteralType stringLiteralType:
                    return new StringLiteralType(stringLiteralType.Value);
                case Azure.Bicep.Types.Concrete.DiscriminatedObjectType discriminatedObjectType:
                {
                    var elementReferences = discriminatedObjectType.Elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(discriminatedObjectType.BaseProperties, kvp.Key, kvp.Value, isResourceBodyType)));

                    return new DiscriminatedObjectType(discriminatedObjectType.Name, GetValidationFlags(isResourceBodyType), discriminatedObjectType.Discriminator, elementReferences);
                }
                default:
                    throw new ArgumentException();
            }
        }

        private NamedObjectType ToCombinedType(IEnumerable<KeyValuePair<string, Azure.Bicep.Types.Concrete.ObjectProperty>> baseProperties, string name, Azure.Bicep.Types.Concrete.ITypeReference extendedType, bool isResourceBodyType)
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

            return new NamedObjectType(name, GetValidationFlags(isResourceBodyType), extendedProperties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value)), additionalProperties, TypePropertyFlags.None);
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
    }
}