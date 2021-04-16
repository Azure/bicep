// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Az
{

    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        public static IResourceTypeProvider CreateWithAzTypes()
            => CreateWithLoader(new AzResourceTypeLoader(), true);

        public static IResourceTypeProvider CreateWithLoader(IResourceTypeLoader resourceTypeLoader, bool warnOnMissingType)
            => new AzResourceTypeProvider(resourceTypeLoader, warnOnMissingType);

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

            private readonly IDictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceType> cache 
                = new Dictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceType>(KeyComparer.Instance);

            public ResourceType GetOrAdd(ResourceTypeGenerationFlags flags, ResourceTypeReference typeReference, Func<ResourceType> buildFunc)
            {
                var cacheKey = (flags, typeReference);
                if (!cache.TryGetValue(cacheKey, out var value))
                {
                    value = buildFunc();
                    cache[cacheKey] = value;
                }

                return value;
            }
        }

        public const string ResourceTypeDeployments = "Microsoft.Resources/deployments";
        public const string ResourceTypeResourceGroup = "Microsoft.Resources/resourceGroups";

        private readonly IResourceTypeLoader resourceTypeLoader;
        private readonly ImmutableHashSet<ResourceTypeReference> availableResourceTypes;
        private readonly ResourceTypeCache loadedTypeCache;
        private readonly bool warnOnMissingType;

        private static readonly ImmutableHashSet<string> WritableExistingResourceProperties = new []
        {
            LanguageConstants.ResourceNamePropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
        }.ToImmutableHashSet();

        private AzResourceTypeProvider(IResourceTypeLoader resourceTypeLoader, bool warnOnMissingType)
        {
            this.resourceTypeLoader = resourceTypeLoader;
            this.availableResourceTypes = resourceTypeLoader.GetAvailableTypes().ToImmutableHashSet(ResourceTypeReferenceComparer.Instance);
            this.loadedTypeCache = new ResourceTypeCache();
            this.warnOnMissingType = warnOnMissingType;
        }

        private static ObjectType CreateGenericResourceBody(ResourceTypeReference typeReference, Func<string, bool> propertyFilter)
        {
            var properties = LanguageConstants.CreateResourceProperties(typeReference).Where(p => propertyFilter(p.Name));

            return new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, properties, null);
        }

        private ResourceType GenerateResourceType(ResourceTypeReference typeReference)
        {
            if (availableResourceTypes.Contains(typeReference))
            {
                return this.resourceTypeLoader.LoadType(typeReference);
            }

            return new ResourceType(
                typeReference,
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                CreateGenericResourceBody(typeReference, p => true));
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
                            bodyObjectType.MethodResolver);

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
                        break;
                    }

                    var bodyTypes = bodyDiscriminatedType.UnionMembersByKey.Values
                        .Select(x => SetBicepResourceProperties(x, resourceType.ValidParentScopes, resourceType.TypeReference, flags));
                    bodyType = new DiscriminatedObjectType(
                        bodyDiscriminatedType.Name,
                        bodyDiscriminatedType.ValidationFlags,
                        bodyDiscriminatedType.DiscriminatorKey,
                        bodyTypes);
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
            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant;
            if (validParentScopes == ResourceScope.Resource)
            {
                // resource can only be deployed as an extension resource - scope should be required
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            if (isExistingResource)
            {
                // we can refer to a resource at any scope if it is an existing resource not being deployed by this file
                var scopeReference = LanguageConstants.CreateResourceScopeReference(validParentScopes);
                properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopePropertyFlags));

                if (properties.TryGetValue(LanguageConstants.ResourceLocationPropertyName, out var locationTypeProperty))
                {
                    // An existing resource's location is not a deployment-time constant, because it requires runtime evaluation get the value.
                    properties = properties.SetItem(LanguageConstants.ResourceLocationPropertyName, new TypeProperty(LanguageConstants.ResourceLocationPropertyName, locationTypeProperty.TypeReference, locationTypeProperty.Flags & ~TypePropertyFlags.DeployTimeConstant));
                }
            }
            else
            {
                // TODO: remove 'dependsOn' from the type library
                properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new TypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly));

                if (properties.TryGetValue(LanguageConstants.ResourceLocationPropertyName, out var locationTypeProperty))
                {
                    properties = properties.SetItem(LanguageConstants.ResourceLocationPropertyName, new TypeProperty(LanguageConstants.ResourceLocationPropertyName, locationTypeProperty.TypeReference, locationTypeProperty.Flags | TypePropertyFlags.DeployTimeConstant));
                }
                
                // we only support scope for extension resources (or resources where the scope is unknown and thus may be an extension resource)
                if (validParentScopes.HasFlag(ResourceScope.Resource))
                {
                    var scopeReference = LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource);
                    properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopePropertyFlags));
                }
            }

            // add the 'parent' property for child resource types
            if (!typeReference.IsRootType)
            {
                var parentType = LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource);
                var parentFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant;

                properties = properties.SetItem(LanguageConstants.ResourceParentPropertyName, new TypeProperty(LanguageConstants.ResourceParentPropertyName, parentType, parentFlags));
            }

            // Deployments RP
            if (StringComparer.OrdinalIgnoreCase.Equals(typeReference.FullyQualifiedType, ResourceTypeDeployments))
            {
                properties = properties.SetItem("resourceGroup", new TypeProperty("resourceGroup", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
                properties = properties.SetItem("subscriptionId", new TypeProperty("subscriptionId", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
            }

            var functions = GetTypeBicepFunctions(typeReference);

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                objectType.AdditionalPropertiesType,
                isExistingResource ? ConvertToReadOnly(objectType.AdditionalPropertiesFlags) : objectType.AdditionalPropertiesFlags,
                functions);
        }

        private static IEnumerable<FunctionOverload> GetTypeBicepFunctions(ResourceTypeReference resourceType)
        {
            switch (resourceType.FullyQualifiedType.ToLowerInvariant())
            {
                case "microsoft.keyvault/vaults":
                    yield return new FunctionOverloadBuilder("getSecret")
                        .WithReturnType(LanguageConstants.KeyVaultSecretReference)
                        .WithDescription("References a secret from this key vault to be binded to a secure string module parameter")
                        .WithFlags(FunctionFlags.ModuleParamsAssignmentOnly)
                        .WithRequiredParameter("secretName", LanguageConstants.String, "Secret Name")
                        .WithOptionalParameter("secretVersion", LanguageConstants.String, "Secret Version")
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

        public ResourceType GetType(ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            // It's important to cache this result because GenerateResourceType is an expensive operation
            return loadedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = GenerateResourceType(typeReference);

                return SetBicepResourceProperties(resourceType, flags);
            });
        }

        public bool HasType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference) || !warnOnMissingType;

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;
    }
}