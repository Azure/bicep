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
        private readonly Dictionary<SerializedTypes.Concrete.TypeBase, TypeSymbol> typeCache;
        private readonly IReadOnlyDictionary<string, SerializedTypes.Concrete.ResourceType> resourceTypes;
        private readonly string apiVersion;

        public AzResourceTypeFactory(IEnumerable<SerializedTypes.Concrete.TypeBase> serializedTypes, string apiVersion)
        {
            typeCache = new Dictionary<SerializedTypes.Concrete.TypeBase, TypeSymbol>();
            resourceTypes = serializedTypes.OfType<SerializedTypes.Concrete.ResourceType>()
                .GroupBy(x => x.Name ?? throw new ArgumentException())
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
            this.apiVersion = apiVersion;
        }

        public ResourceType? TryGetResourceType(ResourceTypeReference resourceTypeReference)
        {
            var typeKey = $"{resourceTypeReference.FullyQualifiedType}@{resourceTypeReference.ApiVersion}";
            if (!resourceTypes.TryGetValue(typeKey, out var resourceType))
            {
                return null;
            }

            return GetTypeSymbol(resourceType) as ResourceType;
        }

        private TypeSymbol GetTypeSymbol(SerializedTypes.Concrete.TypeBase serializedType)
        {
            if (!typeCache.TryGetValue(serializedType, out var typeSymbol))
            {
                typeSymbol = ToTypeSymbol(serializedType);
                typeCache[serializedType] = typeSymbol;
            }

            return typeSymbol;
        }

        private ITypeReference GetTypeReference(SerializedTypes.Concrete.ITypeReference input)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type));

        private TypeProperty GetTypeProperty(string name, SerializedTypes.Concrete.ObjectProperty input)
        {
            var type = input.Type ?? throw new ArgumentException();

            return new TypeProperty(name, GetTypeReference(type), GetTypePropertyFlags(input));
        }

        private static TypePropertyFlags GetTypePropertyFlags(SerializedTypes.Concrete.ObjectProperty input)
        {
            var flags = TypePropertyFlags.None;

            if (input.Flags.HasFlag(SerializedTypes.Concrete.ObjectPropertyFlags.Required))
            {
                flags |= TypePropertyFlags.Required;
            }
            if (input.Flags.HasFlag(SerializedTypes.Concrete.ObjectPropertyFlags.ReadOnly))
            {
                flags |= TypePropertyFlags.ReadOnly;
            }
            if (input.Flags.HasFlag(SerializedTypes.Concrete.ObjectPropertyFlags.WriteOnly))
            {
                flags |= TypePropertyFlags.WriteOnly;
            }
            if (input.Flags.HasFlag(SerializedTypes.Concrete.ObjectPropertyFlags.DeployTimeConstant))
            {
                flags |= TypePropertyFlags.SkipInlining;
            }

            return flags;
        }

        private TypeSymbol ToTypeSymbol(SerializedTypes.Concrete.TypeBase typeBase)
        {
            switch (typeBase)
            {
                case SerializedTypes.Concrete.BuiltInType builtInType:
                    return builtInType.Kind switch {
                        SerializedTypes.Concrete.BuiltInTypeKind.Any => LanguageConstants.Any,
                        SerializedTypes.Concrete.BuiltInTypeKind.Null => LanguageConstants.Null,
                        SerializedTypes.Concrete.BuiltInTypeKind.Bool => LanguageConstants.Bool,
                        SerializedTypes.Concrete.BuiltInTypeKind.Int => LanguageConstants.Int,
                        SerializedTypes.Concrete.BuiltInTypeKind.String => LanguageConstants.String,
                        SerializedTypes.Concrete.BuiltInTypeKind.Object => LanguageConstants.Object,
                        SerializedTypes.Concrete.BuiltInTypeKind.Array => LanguageConstants.Array,
                        SerializedTypes.Concrete.BuiltInTypeKind.ResourceRef => LanguageConstants.ResourceRef,
                        _ => throw new ArgumentException(),
                    };
                case SerializedTypes.Concrete.ObjectType objectType:
                {
                    var name = objectType.Name ?? string.Empty; // TODO
                    var properties = objectType.Properties ?? new Dictionary<string, SerializedTypes.Concrete.ObjectProperty>();
                    var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties) : null;

                    return new NamedObjectType(name, properties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value)), additionalProperties, TypePropertyFlags.None);
                }
                case SerializedTypes.Concrete.ArrayType arrayType:
                {
                    var itemType = arrayType.ItemType ?? throw new ArgumentException();

                    return new TypedArrayType(GetTypeReference(itemType));
                }
                case SerializedTypes.Concrete.ResourceType resourceType:
                {
                    var name = resourceType.Name ?? throw new ArgumentException();
                    var body = resourceType.Body ?? throw new ArgumentException();                    
                    var resourceTypeReference = ResourceTypeReference.Parse(name);

                    return new ResourceType(resourceTypeReference, GetTypeReference(body));
                }
                case SerializedTypes.Concrete.UnionType unionType:
                {
                    var elements = unionType.Elements ?? throw new ArgumentException();
                    return UnionType.Create(elements.Select(GetTypeReference));
                }
                case SerializedTypes.Concrete.StringLiteralType stringLiteralType:
                    var value = stringLiteralType.Value ?? throw new ArgumentException();
                    return new StringLiteralType(value);
                case SerializedTypes.Concrete.DiscriminatedObjectType discriminatedObjectType:
                {
                    var name = discriminatedObjectType.Name ?? throw new ArgumentException();
                    var discriminator = discriminatedObjectType.Discriminator ?? throw new ArgumentException();
                    var elements = discriminatedObjectType.Elements ?? throw new ArgumentException();
                    var baseProperties = discriminatedObjectType.BaseProperties ?? throw new ArgumentException();

                    var elementReferences = elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(discriminatedObjectType.BaseProperties, kvp.Key, kvp.Value)));

                    return new DiscriminatedObjectType(name, discriminator, elementReferences);
                }
                default:
                    throw new ArgumentException();
            }
        }

        private NamedObjectType ToCombinedType(IEnumerable<KeyValuePair<string, SerializedTypes.Concrete.ObjectProperty>> baseProperties, string name, SerializedTypes.Concrete.ITypeReference extendedType)
        {
            if (!(extendedType.Type is SerializedTypes.Concrete.ObjectType objectType))
            {
                throw new ArgumentException();
            }

            var properties = objectType.Properties ?? throw new ArgumentException();
            var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties) : null;

            var extendedProperties = baseProperties.Concat(properties);

            return new NamedObjectType(name, extendedProperties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value)), additionalProperties, TypePropertyFlags.None);
        }
    }
}