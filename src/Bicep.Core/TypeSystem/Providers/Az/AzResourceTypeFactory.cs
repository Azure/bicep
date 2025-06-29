// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class AzResourceTypeFactory
    {
        private readonly ConcurrentDictionary<(Azure.Bicep.Types.Concrete.TypeBase definedType, bool isResourceBodyType, bool isResourceBodyTopLevelPropertyType), TypeSymbol> typeCache;

        public AzResourceTypeFactory()
        {
            typeCache = new();
        }

        public ResourceTypeComponents GetResourceType(Azure.Bicep.Types.Concrete.ResourceType resourceType, IEnumerable<FunctionOverload> resourceFunctions)
        {
            var resourceTypeReference = ResourceTypeReference.Parse(resourceType.Name);
            var bodyType = GetTypeSymbol(resourceType.Body.Type, isResourceBodyType: true, isResourceBodyTopLevelPropertyType: false);
            var assertsProperty = new NamedTypeProperty(LanguageConstants.ResourceAssertPropertyName, AzResourceTypeProvider.ResourceAsserts);
            // DeclaredResourceMetadata.TypeReference.FormatType()
            if (bodyType is ObjectType objectType)
            {
                var properties = objectType.Properties.SetItem(LanguageConstants.ResourceAssertPropertyName, assertsProperty);
                if (resourceFunctions.Any())
                {
                    bodyType = new ObjectType(bodyType.Name, bodyType.ValidationFlags, properties.Values, objectType.AdditionalProperties, resourceFunctions);
                }
                else
                {
                    bodyType = new ObjectType(bodyType.Name, bodyType.ValidationFlags, properties.Values, objectType.AdditionalProperties);
                }
            }

            return new ResourceTypeComponents(resourceTypeReference, ToResourceScope(resourceType.ScopeType), ToResourceScope(resourceType.ReadOnlyScopes), ToResourceFlags(resourceType.Flags), bodyType);
        }

        public IEnumerable<FunctionOverload> GetResourceFunctionOverloads(Azure.Bicep.Types.Concrete.ResourceFunctionType resourceFunctionType)
        {
            yield return new FunctionOverloadBuilder(resourceFunctionType.Name)
                .WithReturnType(GetTypeSymbol(resourceFunctionType.Output.Type, false, false))
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            if (resourceFunctionType.Input is not null)
            {
                yield return new FunctionOverloadBuilder(resourceFunctionType.Name)
                    .WithRequiredParameter("apiVersion", TypeFactory.CreateStringLiteralType(resourceFunctionType.ApiVersion), "The api version")
                    .WithRequiredParameter("params", GetTypeSymbol(resourceFunctionType.Input.Type, false, false), $"{resourceFunctionType.Name} parameters")
                    .WithReturnType(GetTypeSymbol(resourceFunctionType.Output.Type, false, false))
                    .WithFlags(FunctionFlags.RequiresInlining)
                    .Build();
            }
        }

        private TypeSymbol GetTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase serializedType, bool isResourceBodyType, bool isResourceBodyTopLevelPropertyType)
            // The cache key should always include *all* arguments passed to this function
            => typeCache.GetOrAdd((serializedType, isResourceBodyType, isResourceBodyTopLevelPropertyType),
                t => ToTypeSymbol(t.definedType, t.isResourceBodyType, t.isResourceBodyTopLevelPropertyType));

        private ITypeReference GetTypeReference(Azure.Bicep.Types.Concrete.ITypeReference input, bool isResourceBodyType, bool isResourceBodyTopLevelPropertyType)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type, isResourceBodyType, isResourceBodyTopLevelPropertyType));

        private NamedTypeProperty GetTypeProperty(string name, Azure.Bicep.Types.Concrete.ObjectTypeProperty input, bool isResourceBodyTopLevelPropertyType)
        {
            return new NamedTypeProperty(name, GetTypeReference(input.Type, isResourceBodyType: false, isResourceBodyTopLevelPropertyType), GetTypePropertyFlags(input), input.Description);
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

        private TypeSymbol ToTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase typeBase, bool isResourceBodyType, bool isResourceBodyTopLevelPropertyType)
        {
            switch (typeBase)
            {
                case Azure.Bicep.Types.Concrete.AnyType:
                    return LanguageConstants.Any;
                case Azure.Bicep.Types.Concrete.NullType:
                    return LanguageConstants.Null;
                case Azure.Bicep.Types.Concrete.BooleanType:
                    return TypeFactory.CreateBooleanType(GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: false));
                case Azure.Bicep.Types.Concrete.IntegerType @int:
                    return TypeFactory.CreateIntegerType(@int.MinValue,
                        @int.MaxValue,
                        GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: false));
                case Azure.Bicep.Types.Concrete.StringType @string:
                    return TypeFactory.CreateStringType(@string.MinLength,
                        @string.MaxLength,
                        TypeHelper.AsOptionalValidFiniteRegexPattern(@string.Pattern),
                        GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: @string.Sensitive ?? false));
                case Azure.Bicep.Types.Concrete.BuiltInType builtInType:
                    return builtInType.Kind switch
                    {
#pragma warning disable 618
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Any => LanguageConstants.Any,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Null => LanguageConstants.Null,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Bool => LanguageConstants.Bool,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Int => LanguageConstants.Int,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.String => LanguageConstants.String,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Object => LanguageConstants.Object,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.Array => LanguageConstants.Array,
                        Azure.Bicep.Types.Concrete.BuiltInTypeKind.ResourceRef => LanguageConstants.ResourceRef,
#pragma warning restore 618
                        _ => throw new ArgumentException(),
                    };
                case Azure.Bicep.Types.Concrete.ObjectType objectType:
                    {
                        var additionalProperties = objectType.AdditionalProperties != null
                            ? GetTypeReference(
                                input: objectType.AdditionalProperties,
                                isResourceBodyType: false,
                                isResourceBodyTopLevelPropertyType: isResourceBodyType)
                            : null;
                        var properties = objectType.Properties.Select(kvp => GetTypeProperty(
                            name: kvp.Key,
                            input: kvp.Value,
                            isResourceBodyTopLevelPropertyType: isResourceBodyType));

                        return new ObjectType(objectType.Name,
                            GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: objectType.Sensitive ?? false),
                            properties,
                            additionalProperties is not null ? new(additionalProperties) : null);
                    }
                case Azure.Bicep.Types.Concrete.ArrayType arrayType:
                    {
                        return new TypedArrayType(GetTypeReference(arrayType.ItemType, false, false),
                            GetValidationFlags(isResourceBodyType: false, isResourceBodyTopLevelPropertyType: isResourceBodyType, isSensitive: false));
                    }
                case Azure.Bicep.Types.Concrete.UnionType unionType:
                    {
                        return TypeHelper.CreateTypeUnion(
                            unionType.Elements.Select(x => GetTypeReference(x, isResourceBodyType, isResourceBodyTopLevelPropertyType)));
                    }
                case Azure.Bicep.Types.Concrete.StringLiteralType stringLiteralType:
                    return TypeFactory.CreateStringLiteralType(stringLiteralType.Value,
                        GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: false));
                case Azure.Bicep.Types.Concrete.DiscriminatedObjectType discriminatedObjectType:
                    {
                        var elementReferences = discriminatedObjectType.Elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(
                            discriminatedObjectType.BaseProperties,
                            kvp.Key,
                            kvp.Value,
                            isResourceBodyType,
                            isResourceBodyTopLevelPropertyType)));

                        return new DiscriminatedObjectType(discriminatedObjectType.Name,
                            GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: false),
                            discriminatedObjectType.Discriminator,
                            elementReferences);
                    }
                default:
                    throw new ArgumentException();
            }
        }

        private ObjectType ToCombinedType(
            IEnumerable<KeyValuePair<string, Azure.Bicep.Types.Concrete.ObjectTypeProperty>> baseProperties,
            string name,
            Azure.Bicep.Types.Concrete.ITypeReference extendedType,
            bool isResourceBodyType,
            bool isResourceBodyTopLevelPropertyType)
        {
            if (extendedType.Type is not Azure.Bicep.Types.Concrete.ObjectType objectType)
            {
                throw new ArgumentException();
            }

            var additionalProperties = objectType.AdditionalProperties != null
                ? GetTypeReference(objectType.AdditionalProperties, isResourceBodyType: false, isResourceBodyTopLevelPropertyType: isResourceBodyType)
                : null;

            var extendedProperties = objectType.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var property in baseProperties.Where(x => !extendedProperties.ContainsKey(x.Key)))
            {
                extendedProperties[property.Key] = property.Value;
            }

            return new ObjectType(name,
                GetValidationFlags(isResourceBodyType, isResourceBodyTopLevelPropertyType, isSensitive: objectType.Sensitive ?? false),
                extendedProperties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value, isResourceBodyType)),
                additionalProperties is not null ? new(additionalProperties) : null);
        }

        private static TypeSymbolValidationFlags GetValidationFlags(bool isResourceBodyType, bool isResourceBodyTopLevelPropertyType, bool isSensitive)
        {
            var flags = TypeSymbolValidationFlags.Default;

            if (!isResourceBodyType)
            {
                flags |= TypeSymbolValidationFlags.WarnOnPropertyTypeMismatch;
            }

            // strict validation on top-level resource properties, as 'custom' top-level properties are not supported by the platform
            if (!isResourceBodyType && !isResourceBodyTopLevelPropertyType)
            {
                // in all other places, we should allow some wiggle room so that we don't block compilation if there are any swagger inaccuracies
                flags |= TypeSymbolValidationFlags.WarnOnTypeMismatch;
            }

            if (isSensitive)
            {
                flags |= TypeSymbolValidationFlags.IsSecure;
            }

            return flags;
        }

        private static ResourceFlags ToResourceFlags(Azure.Bicep.Types.Concrete.ResourceFlags input)
        {
            var output = ResourceFlags.None;
            if (input.HasFlag(Azure.Bicep.Types.Concrete.ResourceFlags.ReadOnly))
            {
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
