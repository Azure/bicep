// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using Bicep.Core.Emit;
using System.Text.RegularExpressions;

namespace Bicep.Core.TypeSystem.Az
{

    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        private class ResourceTypeCache
        {
            private class KeyComparer : IEqualityComparer<(ResourceTypeGenerationFlags flags, ResourceTypeReference type)>
            {
                public static IEqualityComparer<(ResourceTypeGenerationFlags flags, ResourceTypeReference type)> Instance { get; }
                    = new KeyComparer();

                public bool Equals((ResourceTypeGenerationFlags flags, ResourceTypeReference type) x, (ResourceTypeGenerationFlags flags, ResourceTypeReference type) y)
                    => x.flags == y.flags &&
                        ResourceTypeReferenceComparer.Instance.Equals(x.type, y.type);

                public int GetHashCode((ResourceTypeGenerationFlags flags, ResourceTypeReference type) x)
                    => x.flags.GetHashCode() ^
                        ResourceTypeReferenceComparer.Instance.GetHashCode(x.type);
            }

            private readonly ConcurrentDictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceType> cache
                = new ConcurrentDictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceType>(KeyComparer.Instance);

            public ResourceType GetOrAdd(ResourceTypeGenerationFlags flags, ResourceTypeReference typeReference, Func<ResourceType> buildFunc)
            {
                var cacheKey = (flags, typeReference);

                return cache.GetOrAdd(cacheKey, cacheKey => buildFunc());
            }
        }

        public const string ResourceTypeDeployments = "Microsoft.Resources/deployments";
        public const string ResourceTypeResourceGroup = "Microsoft.Resources/resourceGroups";
        public const string ResourceTypeManagementGroup = "Microsoft.Management/managementGroups";

        private readonly IAzResourceTypeLoader resourceTypeLoader;
        private readonly ImmutableHashSet<ResourceTypeReference> availableResourceTypes;
        private readonly ResourceTypeCache definedTypeCache;
        private readonly ResourceTypeCache generatedTypeCache;

        private static readonly ImmutableHashSet<string> WritableExistingResourceProperties = new[]
        {
            LanguageConstants.ResourceNamePropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
        }.ToImmutableHashSet();

        public AzResourceTypeProvider(IAzResourceTypeLoader resourceTypeLoader)
        {
            this.resourceTypeLoader = resourceTypeLoader;
            this.availableResourceTypes = resourceTypeLoader.GetAvailableTypes().ToImmutableHashSet(ResourceTypeReferenceComparer.Instance);
            this.definedTypeCache = new ResourceTypeCache();
            this.generatedTypeCache = new ResourceTypeCache();
        }

        private static ObjectType CreateGenericResourceBody(ResourceTypeReference typeReference, Func<string, bool> propertyFilter)
        {
            var properties = LanguageConstants.CreateResourceProperties(typeReference).Where(p => propertyFilter(p.Name));

            return new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, properties, null);
        }

        private static ResourceType SetBicepResourceProperties(ResourceType resourceType, ResourceTypeGenerationFlags flags)
        {
            var bodyType = resourceType.Body.Type;

            switch (bodyType)
            {
                case ObjectType bodyObjectType:
                    if (bodyObjectType.Properties.TryGetValue(LanguageConstants.ResourceNamePropertyName, out var nameProperty) &&
                        nameProperty.TypeReference.Type is not PrimitiveType { Name: LanguageConstants.TypeNameString } &&
                        !flags.HasFlag(ResourceTypeGenerationFlags.PermitLiteralNameProperty))
                    {
                        // The 'name' property doesn't support fixed value names (e.g. we're in a top-level child resource declaration).
                        // Best we can do is return a regular 'string' field for it as we have no good way to reliably evaluate complex expressions (e.g. to check whether it terminates with '/<constantType>').
                        // Keep it simple for now - we eventually plan to phase out the 'top-level child' syntax.
                        bodyObjectType = new ObjectType(
                            bodyObjectType.Name,
                            bodyObjectType.ValidationFlags,
                            bodyObjectType.Properties.SetItem(LanguageConstants.ResourceNamePropertyName, new TypeProperty(nameProperty.Name, LanguageConstants.String, nameProperty.Flags)).Values,
                            bodyObjectType.AdditionalPropertiesType,
                            bodyObjectType.AdditionalPropertiesFlags,
                            bodyObjectType.MethodResolver.CopyToObject);

                        bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                        break;
                    }

                    bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    break;

                case DiscriminatedObjectType bodyDiscriminatedType:

                    if (bodyDiscriminatedType.TryGetDiscriminatorProperty(LanguageConstants.ResourceNamePropertyName) is not null &&
                        !flags.HasFlag(ResourceTypeGenerationFlags.PermitLiteralNameProperty))
                    {
                        // The 'name' property doesn't support fixed value names (e.g. we're in a top-level child resource declaration).
                        // If needed, we should be able to flatten the discriminated object and provide slightly better type validation here.
                        // Keep it simple for now - we eventually plan to phase out the 'top-level child' syntax.
                        var bodyObjectType = CreateGenericResourceBody(resourceType.TypeReference, p => bodyDiscriminatedType.UnionMembersByKey.Values.Any(x => x.Properties.ContainsKey(p)));

                        bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    }
                    else if (bodyDiscriminatedType.TryGetDiscriminatorProperty(LanguageConstants.ResourceNamePropertyName) is null &&
                             flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource))
                    {
                        // This reference to existing resource and discriminator is not a name.
                        // TODO: Implement merging properties in case of a name/TypeReference clash should make a union type out of TypeReference
                        // For now, we just make a generic type. It's better than compilation error

                        var bodyObjectType = CreateGenericResourceBody(resourceType.TypeReference, p => bodyDiscriminatedType.UnionMembersByKey.Values.Any(x => x.Properties.ContainsKey(p)));
                        bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    }
                    else
                    {
                        var bodyTypes = bodyDiscriminatedType.UnionMembersByKey.Values
                            .Select(x => SetBicepResourceProperties(x, resourceType.ValidParentScopes, resourceType.TypeReference, flags));
                        bodyType = new DiscriminatedObjectType(
                            bodyDiscriminatedType.Name,
                            bodyDiscriminatedType.ValidationFlags,
                            bodyDiscriminatedType.DiscriminatorKey,
                            bodyTypes);
                    }
                    break;
                default:
                    // we exhaustively test deserialization of every resource type during CI, and this happens in a deterministic fashion,
                    // so this exception should never occur in the released product
                    throw new ArgumentException($"Resource {resourceType.Name} has unexpected body type {bodyType.GetType()}");
            }

            return new ResourceType(resourceType.TypeReference, resourceType.ValidParentScopes, bodyType);
        }

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceScope validParentScopes, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            // Local function.
            static TypeProperty UpdateFlags(TypeProperty typeProperty, TypePropertyFlags flags) =>
                new(typeProperty.Name, typeProperty.TypeReference, flags, typeProperty.Description);

            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant;
            if (validParentScopes == ResourceScope.Resource)
            {
                // resource can only be deployed as an extension resource - scope should be required
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            if (isExistingResource)
            {
                // we can refer to a resource at any scope if it is an existing resource not being deployed by this file
                properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, ScopeHelper.CreateExistingResourceScopeProperty(validParentScopes, scopePropertyFlags));
            }
            else
            {
                // TODO: remove 'dependsOn' from the type library
                properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new TypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny));

                if (ScopeHelper.TryCreateNonExistingResourceScopeProperty(validParentScopes, scopePropertyFlags) is { } scopeProperty)
                {
                    properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, scopeProperty);
                }

                // TODO: move this to the type library.
                foreach (var propertyName in LanguageConstants.WriteOnlyDeployTimeConstantPropertyNames)
                {
                    if (properties.TryGetValue(propertyName, out var typeProperty))
                    {
                        // Update tags for deploy-time constant properties that are not readable at deploy-time.
                        properties = properties.SetItem(propertyName, UpdateFlags(typeProperty, typeProperty.Flags | TypePropertyFlags.DeployTimeConstant));
                    }
                }
            }

            // TODO: move this to the type library.
            foreach (var propertyName in LanguageConstants.ReadWriteDeployTimeConstantPropertyNames)
            {
                if (properties.TryGetValue(propertyName, out var typeProperty))
                {
                    // Update tags for standardized resource properties that are always readable at deploy-time.
                    properties = properties.SetItem(propertyName, UpdateFlags(typeProperty, typeProperty.Flags | TypePropertyFlags.ReadableAtDeployTime));
                }
            }

            // add the loop variant flag to the name property (if it exists)
            if (properties.TryGetValue(LanguageConstants.ResourceNamePropertyName, out var nameProperty))
            {
                properties = properties.SetItem(LanguageConstants.ResourceNamePropertyName, UpdateFlags(nameProperty, nameProperty.Flags | TypePropertyFlags.LoopVariant));
            }

            // add the 'parent' property for child resource types that are not nested inside a parent resource
            if (!typeReference.IsRootType && !flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
            {
                var parentType = new ResourceParentType(typeReference);
                var parentFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant;

                properties = properties.SetItem(LanguageConstants.ResourceParentPropertyName, new TypeProperty(LanguageConstants.ResourceParentPropertyName, parentType, parentFlags));
            }

            // Deployments RP
            if (StringComparer.OrdinalIgnoreCase.Equals(typeReference.FullyQualifiedType, ResourceTypeDeployments))
            {
                properties = properties.SetItem("resourceGroup", new TypeProperty("resourceGroup", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
                properties = properties.SetItem("subscriptionId", new TypeProperty("subscriptionId", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
            }

            var functions = GetBicepMethods(typeReference);

            foreach (var item in LanguageConstants.KnownTopLevelResourceProperties())
            {
                if (!properties.ContainsKey(item.Name))
                {
                    properties = properties.Add(item.Name, new TypeProperty(item.Name, item.TypeReference, item.Flags | TypePropertyFlags.FallbackProperty));
                }
            }

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                objectType.AdditionalPropertiesType,
                isExistingResource ? ConvertToReadOnly(objectType.AdditionalPropertiesFlags) : objectType.AdditionalPropertiesFlags,
                functions);
        }

        private static IEnumerable<FunctionOverload> GetBicepMethods(ResourceTypeReference resourceType)
        {
            yield return new FunctionWildcardOverloadBuilder("list*", new Regex("^list[a-zA-Z]*"))
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("The syntax for this function varies by name of the list operations. Each implementation returns values for the resource type that supports a list operation. The operation name must start with list. Some common usages are `listKeys`, `listKeyValue`, and `listSecrets`.")
                .WithOptionalParameter("apiVersion", LanguageConstants.String, "API version of resource runtime state. Typically, in the format, yyyy-mm-dd.")
                .WithOptionalParameter("functionValues", LanguageConstants.Object, "An object that has values for the function. Only provide this object for functions that support receiving an object with parameter values, such as listAccountSas on a storage account. An example of passing function values is shown in this article.")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            switch (resourceType.FullyQualifiedType.ToLowerInvariant())
            {
                case "microsoft.keyvault/vaults":
                    yield return new FunctionOverloadBuilder("getSecret")
                        .WithReturnType(LanguageConstants.SecureString)
                        .WithDescription("Gets a reference to a key vault secret, which can be provided to a secure string module parameter")
                        .WithRequiredParameter("secretName", LanguageConstants.String, "Secret Name")
                        .WithOptionalParameter("secretVersion", LanguageConstants.String, "Secret Version")
                        .WithFlags(FunctionFlags.ModuleSecureParameterOnly)
                        .Build();
                    break;
            }
        }

        private static IEnumerable<TypeProperty> ConvertToReadOnly(IEnumerable<TypeProperty> properties)
        {
            foreach (var property in properties)
            {
                // "name", "scope" & "parent" can be set for existing resources - everything else should be read-only
                if (WritableExistingResourceProperties.Contains(property.Name))
                {
                    yield return property;
                }
                else
                {
                    yield return new TypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags));
                }
            }
        }

        private static TypePropertyFlags ConvertToReadOnly(TypePropertyFlags typePropertyFlags)
            => (typePropertyFlags | TypePropertyFlags.ReadOnly) & ~TypePropertyFlags.Required;

        public ResourceType? TryGetDefinedType(ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            if (!HasDefinedType(typeReference))
            {
                return null;
            }

            // It's important to cache this result because generating the resource type is an expensive operation
            return definedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = this.resourceTypeLoader.LoadType(typeReference);

                return SetBicepResourceProperties(resourceType, flags);
            });
        }

        public ResourceType? TryGenerateDefaultType(ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            // It's important to cache this result because generating the resource type is an expensive operation
            return generatedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = new ResourceType(
                    typeReference,
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                    CreateGenericResourceBody(typeReference, p => true));

                return SetBicepResourceProperties(resourceType, flags);
            });
        }

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;
    }
}
