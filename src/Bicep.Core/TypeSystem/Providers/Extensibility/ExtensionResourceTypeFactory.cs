// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.TypeSystem.Providers.Extensibility
{
    public class ExtensionResourceTypeFactory
    {
        private readonly ConcurrentDictionary<Azure.Bicep.Types.Concrete.TypeBase, TypeSymbol> typeCache;
        private readonly bool shouldWarnOnResourceTypeMismatch;

        public ExtensionResourceTypeFactory(TypeSettings? typeSettings)
        {
            this.typeCache = new();

            // TODO: Make this a new type setting.
            this.shouldWarnOnResourceTypeMismatch =
                typeSettings is not null &&
                MicrosoftGraphExtensionFacts.IsMicrosoftGraphExtension(typeSettings.Name);
        }

        public ResourceTypeComponents GetResourceType(Azure.Bicep.Types.Concrete.ResourceType resourceType)
        {
            var resourceTypeReference = ResourceTypeReference.Parse(resourceType.Name);
            var bodyType = GetTypeSymbol(resourceType.Body.Type, true);

            if (bodyType is ObjectType objectType &&
                GetResourceFunctionOverloads(resourceType) is { } resourceFunctions &&
                resourceFunctions.Any())
            {
                bodyType = new ObjectType(bodyType.Name, bodyType.ValidationFlags, objectType.Properties.Values, objectType.AdditionalProperties, resourceFunctions);
            }

            var (readableScopes, writableScopes) = GetScopeInfo(resourceType);
            var readOnlyScopes = readableScopes & ~writableScopes;

            // Temporary fix for https://github.com/Azure/bicep/issues/18158
            var validParentScopes = readableScopes | writableScopes;

            return new ResourceTypeComponents(resourceTypeReference, validParentScopes, readOnlyScopes, ToResourceFlags(resourceType), bodyType);
        }

        public TypeSymbol GetConfigurationType(Azure.Bicep.Types.Concrete.TypeBase configurationType)
        {
            if (configurationType is not Azure.Bicep.Types.Concrete.ObjectType and not Azure.Bicep.Types.Concrete.DiscriminatedObjectType)
            {
                throw new ArgumentException("Configuration type must be an ObjectType or a DiscriminatedObjectType.");
            }

            var bodyType = GetTypeSymbol(configurationType, false);

            return bodyType;
        }

        private IEnumerable<FunctionOverload> GetResourceFunctionOverloads(Azure.Bicep.Types.Concrete.ResourceType resourceType)
        {
            if (resourceType.Functions is null)
            {
                yield break;
            }

            foreach (var (key, value) in resourceType.Functions)
            {
                if (value.Type.Type is not Azure.Bicep.Types.Concrete.FunctionType functionType)
                {
                    throw new ArgumentException();
                }

                var builder = new FunctionOverloadBuilder(key);
                if (value.Description is { })
                {
                    builder = builder.WithDescription(value.Description);
                }

                var returnType = GetTypeSymbol(functionType.Output.Type, false);
                builder = builder.WithReturnType(returnType);

                foreach (var parameter in functionType.Parameters)
                {
                    var paramType = GetTypeSymbol(parameter.Type.Type, false);
                    builder = builder.WithRequiredParameter(parameter.Name, paramType, parameter.Description ?? "");
                }

                builder = builder.WithFlags(FunctionFlags.RequiresInlining);

                yield return builder.Build();
            }
        }

        private TypeSymbol GetTypeSymbol(Azure.Bicep.Types.Concrete.TypeBase serializedType, bool isResourceBodyType)
            => typeCache.GetOrAdd(serializedType, serializedType => ToTypeSymbol(serializedType, isResourceBodyType));

        private ITypeReference GetTypeReference(Azure.Bicep.Types.Concrete.ITypeReference input, bool isResourceBodyType)
            => new DeferredTypeReference(() => GetTypeSymbol(input.Type, isResourceBodyType));

        private NamedTypeProperty GetTypeProperty(string name, Azure.Bicep.Types.Concrete.ObjectTypeProperty input, bool isResourceBodyType)
        {
            return new NamedTypeProperty(name, GetTypeReference(input.Type, isResourceBodyType), GetTypePropertyFlags(input), input.Description);
        }

        private static TypePropertyFlags GetTypePropertyFlags(Azure.Bicep.Types.Concrete.ObjectTypeProperty input)
        {
            var flags = TypePropertyFlags.None;

            if (input.Flags.HasFlag(Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.Identifier))
            {
                flags |= TypePropertyFlags.ResourceIdentifier;
                flags |= TypePropertyFlags.LoopVariant;
            }
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
                case Azure.Bicep.Types.Concrete.AnyType:
                    return LanguageConstants.Any;
                case Azure.Bicep.Types.Concrete.NullType:
                    return LanguageConstants.Null;
                case Azure.Bicep.Types.Concrete.BooleanType:
                    return LanguageConstants.Bool;
                case Azure.Bicep.Types.Concrete.IntegerType @int:
                    return TypeFactory.CreateIntegerType(@int.MinValue, @int.MaxValue, this.GetValidationFlags(isResourceBodyType, isSensitive: false));
                case Azure.Bicep.Types.Concrete.StringType @string:
                    return TypeFactory.CreateStringType(
                        @string.MinLength,
                        @string.MaxLength,
                        TypeHelper.AsOptionalValidFiniteRegexPattern(@string.Pattern),
                        GetValidationFlags(isResourceBodyType, isSensitive: @string.Sensitive ?? false));
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
                        var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties, isResourceBodyType) : null;
                        var properties = objectType.Properties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value, isResourceBodyType));

                        return new ObjectType(objectType.Name, this.GetValidationFlags(isResourceBodyType, isSensitive: objectType.Sensitive ?? false), properties, additionalProperties is not null ? new(additionalProperties) : null);
                    }
                case Azure.Bicep.Types.Concrete.ArrayType arrayType:
                    {
                        return new TypedArrayType(GetTypeReference(arrayType.ItemType, isResourceBodyType), GetValidationFlags(isResourceBodyType, isSensitive: false));
                    }
                case Azure.Bicep.Types.Concrete.UnionType unionType:
                    {
                        return TypeHelper.CreateTypeUnion(unionType.Elements.Select(x => GetTypeReference(x, isResourceBodyType)));
                    }
                case Azure.Bicep.Types.Concrete.StringLiteralType stringLiteralType:
                    return TypeFactory.CreateStringLiteralType(stringLiteralType.Value);
                case Azure.Bicep.Types.Concrete.DiscriminatedObjectType discriminatedObjectType:
                    {
                        var elementReferences = discriminatedObjectType.Elements.Select(kvp => new DeferredTypeReference(() => ToCombinedType(discriminatedObjectType.BaseProperties, kvp.Key, kvp.Value, isResourceBodyType)));

                        return new DiscriminatedObjectType(discriminatedObjectType.Name, GetValidationFlags(isResourceBodyType, isSensitive: false), discriminatedObjectType.Discriminator, elementReferences);
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

            var additionalProperties = objectType.AdditionalProperties != null ? GetTypeReference(objectType.AdditionalProperties, isResourceBodyType) : null;

            var extendedProperties = objectType.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var property in baseProperties.Where(x => !extendedProperties.ContainsKey(x.Key)))
            {
                extendedProperties[property.Key] = property.Value;
            }

            return new ObjectType(name, GetValidationFlags(isResourceBodyType, isSensitive: objectType.Sensitive ?? false), extendedProperties.Select(kvp => GetTypeProperty(kvp.Key, kvp.Value, isResourceBodyType)), additionalProperties is not null ? new(additionalProperties) : null);
        }

        private TypeSymbolValidationFlags GetValidationFlags(bool isResourceBodyType, bool isSensitive)
        {
            // TODO: Add this to ResourceFlags
            var flags = TypeSymbolValidationFlags.Default;

            if (isResourceBodyType && this.shouldWarnOnResourceTypeMismatch)
            {
                flags |= TypeSymbolValidationFlags.WarnOnTypeMismatch | TypeSymbolValidationFlags.WarnOnPropertyTypeMismatch;
            }

            if (isSensitive)
            {
                flags |= TypeSymbolValidationFlags.IsSecure;
            }

            return flags;
        }

        private static ResourceFlags ToResourceFlags(Azure.Bicep.Types.Concrete.ResourceType input)
        {
            var output = ResourceFlags.None;
            var (readableScopes, writableScopes) = GetScopeInfo(input);

            // Resource is ReadOnly if there are no writable scopes but has readable scopes
            if (writableScopes == ResourceScope.None && readableScopes != ResourceScope.None)
            {
                output |= ResourceFlags.ReadOnly;
            }

            // Resource is WriteOnly if there are no readable scopes but has writable scopes
            // This is specifically for extensions which cannot be referenced as existing
            if (readableScopes == ResourceScope.None && writableScopes != ResourceScope.None)
            {
                output |= ResourceFlags.WriteOnly;
            }

            return output;
        }

        private static (ResourceScope readableScopes, ResourceScope writableScopes) GetScopeInfo(Azure.Bicep.Types.Concrete.ResourceType resourceType)
        {
            var readableScopes = ToResourceScope(resourceType.ReadableScopes);
            var writableScopes = ToResourceScope(resourceType.WritableScopes);

            if (readableScopes == ResourceScope.None && writableScopes == ResourceScope.None)
            {
                readableScopes = ToResourceScope(Azure.Bicep.Types.Concrete.ScopeType.All);
                writableScopes = ToResourceScope(Azure.Bicep.Types.Concrete.ScopeType.All);
            }

            return (readableScopes, writableScopes);
        }

        private static ResourceScope ToResourceScope(Azure.Bicep.Types.Concrete.ScopeType input)
        {
            if (input == Azure.Bicep.Types.Concrete.ScopeType.None)
            {
                // ScopeType.None means no valid scopes
                return ResourceScope.None;
            }

            if (input == Azure.Bicep.Types.Concrete.ScopeType.All)
            {
                return ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource | ResourceScope.Local;
            }

            var output = ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Extension) ? ResourceScope.Resource : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Tenant) ? ResourceScope.Tenant : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.ManagementGroup) ? ResourceScope.ManagementGroup : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.Subscription) ? ResourceScope.Subscription : ResourceScope.None;
            output |= input.HasFlag(Azure.Bicep.Types.Concrete.ScopeType.ResourceGroup) ? ResourceScope.ResourceGroup : ResourceScope.None;

            return output;
        }
    }
}
