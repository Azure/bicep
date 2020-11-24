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

        private ITypeReference GetTypeReference(Azure.Bicep.Types.Concrete.ITypeReference input, bool isResourceBodyType)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type, isResourceBodyType));

        private TypeProperty GetTypeProperty(string name, Azure.Bicep.Types.Concrete.ObjectProperty input)
        {
            var type = input.Type ?? throw new ArgumentException();

            return new TypeProperty(name, GetTypeReference(type, false), GetTypePropertyFlags(input));
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
                    var name = objectType.Name ?? string.Empty;
                    var properties = objectType.Properties ?? new Dictionary<string, Azure.Bicep.Types.Concrete.ObjectProperty>();
                    var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties, false) : null;

                    return new NamedObjectType(name, GetValidationFlags(isResourceBodyType), properties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value)), additionalProperties, TypePropertyFlags.None);
                }
                case Azure.Bicep.Types.Concrete.ArrayType arrayType:
                {
                    var itemType = arrayType.ItemType ?? throw new ArgumentException();

                    return new TypedArrayType(GetTypeReference(itemType, false), GetValidationFlags(isResourceBodyType));
                }
                case Azure.Bicep.Types.Concrete.ResourceType resourceType:
                {
                    var name = resourceType.Name ?? throw new ArgumentException();
                    var body = resourceType.Body ?? throw new ArgumentException();                    
                    var resourceTypeReference = ResourceTypeReference.Parse(name);

                    return new ResourceType(resourceTypeReference, GetTypeReference(body, true));
                }
                case Azure.Bicep.Types.Concrete.UnionType unionType:
                {
                    var elements = unionType.Elements ?? throw new ArgumentException();
                    return UnionType.Create(elements.Select(x => GetTypeReference(x, false)));
                }
                case Azure.Bicep.Types.Concrete.StringLiteralType stringLiteralType:
                    var value = stringLiteralType.Value ?? throw new ArgumentException();
                    return new StringLiteralType(value);
                case Azure.Bicep.Types.Concrete.DiscriminatedObjectType discriminatedObjectType:
                {
                    var name = discriminatedObjectType.Name ?? throw new ArgumentException();
                    var discriminator = discriminatedObjectType.Discriminator ?? throw new ArgumentException();
                    var elements = discriminatedObjectType.Elements ?? throw new ArgumentException();
                    var baseProperties = discriminatedObjectType.BaseProperties ?? throw new ArgumentException();

                    var elementReferences = elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(discriminatedObjectType.BaseProperties, kvp.Key, kvp.Value, isResourceBodyType)));

                    return new DiscriminatedObjectType(name, GetValidationFlags(isResourceBodyType), discriminator, elementReferences);
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

            var properties = objectType.Properties ?? throw new ArgumentException();
            var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties, false) : null;

            var extendedProperties = properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
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