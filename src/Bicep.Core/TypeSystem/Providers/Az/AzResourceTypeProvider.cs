// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using Azure.Bicep.Types.Az;
using Bicep.Core.Emit;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class AzResourceTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly RegexOptions PatternRegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private static readonly Regex ResourceTypePattern = new(@"^(?<namespace>[a-z0-9][a-z0-9\.]*)(/(?<type>[a-z0-9\-]+))+$", PatternRegexOptions);
        private static readonly Regex ApiVersionPattern = new(@"^\d{4}-\d{2}-\d{2}(|-(preview|alpha|beta|rc|privatepreview))$", PatternRegexOptions);

        public const string ResourceIdPropertyName = "id";
        public const string ResourceLocationPropertyName = "location";
        public const string ResourceNamePropertyName = "name";
        public const string ResourceTypePropertyName = "type";
        public const string ResourceApiVersionPropertyName = "apiVersion";

        public const string ResourceTypeDeployments = "Microsoft.Resources/deployments";
        public const string ResourceTypeResourceGroup = "Microsoft.Resources/resourceGroups";
        public const string ResourceTypeManagementGroup = "Microsoft.Management/managementGroups";
        public const string ResourceTypeKeyVault = "Microsoft.KeyVault/vaults";
        public const string GetSecretFunctionName = "getSecret";
        /*
         * The following top-level properties must be set deploy-time constant values,
         * and it is safe to read them at deploy-time because their values cannot be changed.
         */
        public static readonly ImmutableSortedSet<string> ReadWriteDeployTimeConstantPropertyNames
            = ImmutableSortedSet.Create(LanguageConstants.IdentifierComparer,
                ResourceIdPropertyName,
                ResourceNamePropertyName,
                ResourceTypePropertyName,
                ResourceApiVersionPropertyName);

        /*
         * The following top-level properties must be set deploy-time constant values
         * when declared in resource bodies. However, it is not safe to read their values
         * at deploy-time due to the fact that:
         *   - They can be changed by Policy Modify effect (e.g. tags, sku)
         *   - Their values may be normalized by RPs
         *   - Some RPs are doing Put-as-Patch
         */
        public static readonly string[] WriteOnlyDeployTimeConstantPropertyNames =
        [
            "location",
            "kind",
            "subscriptionId",
            "resourceGroup",
            "managedBy",
            "extendedLocation",
            "zones",
            "plan",
            "sku",
            "identity",
            "managedByExtended",
            "tags",
            "asserts",
        ];

        public static readonly TypeSymbol Tags = new ObjectType(nameof(Tags), TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.String, TypePropertyFlags.None));
        public static readonly TypeSymbol ResourceAsserts = new ObjectType(nameof(ResourceAsserts), TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.Bool, TypePropertyFlags.DeployTimeConstant));

        private readonly IResourceTypeLoader resourceTypeLoader;
        private readonly ResourceTypeCache definedTypeCache;
        private readonly ResourceTypeCache generatedTypeCache;

        public static readonly FrozenSet<string> UniqueIdentifierProperties = FrozenSet.ToFrozenSet(
        [
            ResourceNamePropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
        ]);

        public static IEnumerable<NamedTypeProperty> GetCommonResourceProperties(ResourceTypeReference reference)
        {
            // We don't expect any Az resource types without an API version
            var apiVersion = reference.ApiVersion ?? throw new ArgumentException($"Resource reference {reference.FormatName()} contains null API Version", nameof(reference));

            yield return new NamedTypeProperty(ResourceIdPropertyName, LanguageConstants.String, TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "The resource id");
            yield return new NamedTypeProperty(ResourceNamePropertyName, LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.LoopVariant | TypePropertyFlags.SystemProperty, "The resource name");
            yield return new NamedTypeProperty(ResourceTypePropertyName, TypeFactory.CreateStringLiteralType(reference.FormatType()), TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "The resource type");
            yield return new NamedTypeProperty(ResourceApiVersionPropertyName, TypeFactory.CreateStringLiteralType(apiVersion), TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "The resource api version");
        }

        public static IEnumerable<NamedTypeProperty> CreateResourceProperties(ResourceTypeReference resourceTypeReference)
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            foreach (var prop in GetCommonResourceProperties(resourceTypeReference))
            {
                yield return prop;
            }

            foreach (var prop in KnownTopLevelResourceProperties())
            {
                yield return prop;
            }
        }

        public static IEnumerable<NamedTypeProperty> KnownTopLevelResourceProperties()
        {
            yield return new NamedTypeProperty("location", LanguageConstants.String);

            yield return new NamedTypeProperty("tags", Tags);

            yield return new NamedTypeProperty("asserts", ResourceAsserts);

            yield return new NamedTypeProperty("properties", LanguageConstants.Object);

            yield return new NamedTypeProperty("sku", new ObjectType("sku", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("name", LanguageConstants.String),
                new NamedTypeProperty("tier", LanguageConstants.String),
                new NamedTypeProperty("size", LanguageConstants.String),
                new NamedTypeProperty("family", LanguageConstants.String),
                new NamedTypeProperty("model", LanguageConstants.String),
                new NamedTypeProperty("capacity", LanguageConstants.Int),
            }, null));

            yield return new NamedTypeProperty("kind", LanguageConstants.String);
            yield return new NamedTypeProperty("managedBy", LanguageConstants.String);

            var stringArray = new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default);
            yield return new NamedTypeProperty("managedByExtended", stringArray);

            var extendedLocationType = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("NotSpecified"),
                TypeFactory.CreateStringLiteralType("EdgeZone"),
                TypeFactory.CreateStringLiteralType("CustomLocation"),
                TypeFactory.CreateStringLiteralType("ArcZone"),
                LanguageConstants.String);

            yield return new NamedTypeProperty("extendedLocation", new ObjectType("extendedLocation", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("type", extendedLocationType, TypePropertyFlags.Required),
                new NamedTypeProperty("name", LanguageConstants.String),
            }, null));

            yield return new NamedTypeProperty("zones", stringArray);

            yield return new NamedTypeProperty("plan", LanguageConstants.Object);

            yield return new NamedTypeProperty("eTag", LanguageConstants.String);

            yield return new NamedTypeProperty("scale", new ObjectType("scale", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("capacity", LanguageConstants.Int, TypePropertyFlags.Required),
                new NamedTypeProperty("maximum", LanguageConstants.Int),
                new NamedTypeProperty("minimum", LanguageConstants.Int),
            }, null));

            var resourceIdentityType = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("NotSpecified"),
                TypeFactory.CreateStringLiteralType("SystemAssigned"),
                TypeFactory.CreateStringLiteralType("UserAssigned"),
                TypeFactory.CreateStringLiteralType("None"),
                TypeFactory.CreateStringLiteralType("Actor"),
                LanguageConstants.String);

            var userAssignedIdentity = new ObjectType("userAssignedIdentityProperties", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("principalId", LanguageConstants.String),
                new NamedTypeProperty("clientId", LanguageConstants.String)
            }, null);

            yield return new NamedTypeProperty("identity", new ObjectType("identity", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("principalId", LanguageConstants.String),
                new NamedTypeProperty("tenantId", LanguageConstants.String),
                new NamedTypeProperty("type", resourceIdentityType, TypePropertyFlags.Required),
                new NamedTypeProperty("identityIds", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)),
                new NamedTypeProperty("userAssignedIdentities", new ObjectType("userAssignedIdentities", TypeSymbolValidationFlags.Default, [], new(userAssignedIdentity))),
                new NamedTypeProperty("delegatedResources", LanguageConstants.Object),
            }, null));
        }

        public AzResourceTypeProvider(IResourceTypeLoader resourceTypeLoader)
            : base([.. resourceTypeLoader.GetAvailableTypes()])
        {
            this.resourceTypeLoader = resourceTypeLoader;
            definedTypeCache = new ResourceTypeCache();
            generatedTypeCache = new ResourceTypeCache();
        }

        private static ObjectType CreateGenericResourceBody(ResourceTypeReference typeReference, Func<string, bool> propertyFilter)
        {
            var properties = CreateResourceProperties(typeReference).Where(p => propertyFilter(p.Name));

            return new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, properties, null);
        }

        private static bool SupportsLiteralNames(ResourceTypeComponents resourceType, ResourceTypeGenerationFlags flags)
            => resourceType.TypeReference.TypeSegments.Length == 2 ||
            flags.HasFlag(ResourceTypeGenerationFlags.HasParentDefined);

        private static ResourceTypeComponents SetBicepResourceProperties(ResourceTypeComponents resourceType, ResourceTypeGenerationFlags flags)
        {
            var bodyType = resourceType.Body.Type;

            switch (bodyType)
            {
                case ObjectType bodyObjectType:
                    if (bodyObjectType.Properties.TryGetValue(ResourceNamePropertyName, out var nameProperty) &&
                        !ReferenceEquals(nameProperty.TypeReference.Type, LanguageConstants.String) &&
                        !SupportsLiteralNames(resourceType, flags))
                    {
                        // The 'name' property doesn't support fixed value names (e.g. we're in a top-level child resource declaration).
                        // Best we can do is return a regular 'string' field for it as we have no good way to reliably evaluate complex expressions (e.g. to check whether it terminates with '/<constantType>').
                        // Keep it simple for now - we eventually plan to phase out the 'top-level child' syntax.
                        bodyObjectType = new ObjectType(
                            bodyObjectType.Name,
                            bodyObjectType.ValidationFlags,
                            bodyObjectType.Properties.SetItem(ResourceNamePropertyName, new(nameProperty.Name, LanguageConstants.String, nameProperty.Flags | TypePropertyFlags.SystemProperty, nameProperty.Description)).Values,
                            bodyObjectType.AdditionalProperties,
                            bodyObjectType.MethodResolver.CopyToObject);

                        bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                        break;
                    }

                    bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    break;

                case DiscriminatedObjectType bodyDiscriminatedType:

                    if (bodyDiscriminatedType.TryGetDiscriminatorProperty(ResourceNamePropertyName) is not null &&
                        !SupportsLiteralNames(resourceType, flags))
                    {
                        // The 'name' property doesn't support fixed value names (e.g. we're in a top-level child resource declaration).
                        // If needed, we should be able to flatten the discriminated object and provide slightly better type validation here.
                        // Keep it simple for now - we eventually plan to phase out the 'top-level child' syntax.
                        var bodyObjectType = CreateGenericResourceBody(resourceType.TypeReference, p => bodyDiscriminatedType.UnionMembersByKey.Values.Any(x => x.Properties.ContainsKey(p)));

                        bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    }
                    else if (bodyDiscriminatedType.TryGetDiscriminatorProperty(ResourceNamePropertyName) is null &&
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
                    throw new ArgumentException($"Resource {resourceType.TypeReference.FormatName()} has unexpected body type {bodyType.GetType()}");
            }

            return resourceType with { Body = bodyType };
        }

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceScope validParentScopes, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            // Local function.
            static NamedTypeProperty UpdateFlags(NamedTypeProperty typeProperty, TypePropertyFlags flags) =>
                new(typeProperty.Name, typeProperty.TypeReference, flags, typeProperty.Description);

            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant | TypePropertyFlags.SystemProperty;
            if (validParentScopes == ResourceScope.Resource && typeReference.TypeSegments.Length < 2)
            {
                // resource can only be deployed as an extension resource - scope should be required
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            // TODO: remove 'dependsOn' from the type library
            properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny | TypePropertyFlags.SystemProperty));

            if (isExistingResource)
            {
                // we can refer to a resource at any scope if it is an existing resource not being deployed by this file
                properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, ScopeHelper.CreateExistingResourceScopeProperty(validParentScopes, scopePropertyFlags));
            }
            else
            {
                if (ScopeHelper.TryCreateNonExistingResourceScopeProperty(validParentScopes, scopePropertyFlags) is { } scopeProperty)
                {
                    properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, scopeProperty);
                }

                // TODO: move this to the type library.
                foreach (var propertyName in WriteOnlyDeployTimeConstantPropertyNames)
                {
                    if (properties.TryGetValue(propertyName, out var typeProperty))
                    {
                        // Update tags for deploy-time constant properties that are not readable at deploy-time.
                        properties = properties.SetItem(propertyName, UpdateFlags(typeProperty, typeProperty.Flags | TypePropertyFlags.DeployTimeConstant));
                    }
                }
            }

            // TODO: move this to the type library.
            foreach (var propertyName in ReadWriteDeployTimeConstantPropertyNames)
            {
                if (properties.TryGetValue(propertyName, out var typeProperty))
                {
                    // Update tags for standardized resource properties that are always readable at deploy-time.
                    properties = properties.SetItem(propertyName, UpdateFlags(typeProperty, typeProperty.Flags | TypePropertyFlags.ReadableAtDeployTime));
                }
            }

            // add the loop variant and system flags to the name property (if it exists)
            if (properties.TryGetValue(ResourceNamePropertyName, out var nameProperty))
            {
                properties = properties.SetItem(ResourceNamePropertyName, UpdateFlags(nameProperty, nameProperty.Flags | TypePropertyFlags.LoopVariant | TypePropertyFlags.SystemProperty));
            }

            // add the 'parent' property for child resource types that are not nested inside a parent resource
            if (typeReference.TypeSegments.Length > 2 && !flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
            {
                var parentType = new ResourceParentType(typeReference);
                var parentFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant | TypePropertyFlags.SystemProperty;

                properties = properties.SetItem(LanguageConstants.ResourceParentPropertyName, new(LanguageConstants.ResourceParentPropertyName, parentType, parentFlags));
            }

            // Deployments RP
            if (StringComparer.OrdinalIgnoreCase.Equals(typeReference.FormatType(), ResourceTypeDeployments))
            {
                properties = properties.SetItem("resourceGroup", new("resourceGroup", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty));
                properties = properties.SetItem("subscriptionId", new("subscriptionId", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty));
            }

            var functions = objectType.MethodResolver.functionOverloads.Concat(GetBicepMethods(typeReference));

            foreach (var item in KnownTopLevelResourceProperties())
            {
                if (!properties.ContainsKey(item.Name))
                {
                    properties = properties.Add(item.Name, new(item.Name, item.TypeReference, item.Flags | TypePropertyFlags.FallbackProperty));
                }
            }

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                isExistingResource && objectType.AdditionalProperties is not null
                    ? objectType.AdditionalProperties with { Flags = ConvertToReadOnly(objectType.AdditionalProperties.Flags) }
                    : objectType.AdditionalProperties,
                functions);
        }

        private static IEnumerable<FunctionOverload> GetBicepMethods(ResourceTypeReference resourceType)
        {
            yield return new FunctionWildcardOverloadBuilder("list*", AzConstants.ListWildcardFunctionRegex)
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("The syntax for this function varies by name of the list operations. Each implementation returns values for the resource type that supports a list operation. The operation name must start with list. Some common usages are `listKeys`, `listKeyValue`, and `listSecrets`.")
                .WithOptionalParameter("apiVersion", LanguageConstants.String, "API version of resource runtime state. Typically, in the format, yyyy-mm-dd.")
                .WithOptionalParameter("functionValues", LanguageConstants.Object, "An object that has values for the function. Only provide this object for functions that support receiving an object with parameter values, such as listAccountSas on a storage account. An example of passing function values is shown in this article.")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();

            if (StringComparer.OrdinalIgnoreCase.Equals(resourceType.FormatType(), ResourceTypeKeyVault))
            {
                yield return new FunctionOverloadBuilder(GetSecretFunctionName)
                    .WithReturnType(LanguageConstants.SecureString)
                    .WithDescription("Gets a reference to a key vault secret, which can be provided to a secure string module parameter")
                    .WithRequiredParameter("secretName", LanguageConstants.String, "Secret Name")
                    .WithOptionalParameter("secretVersion", LanguageConstants.String, "Secret Version")
                    .WithFlags(FunctionFlags.ModuleSecureParameterOnly)
                    .Build();
            }
        }

        private static IEnumerable<NamedTypeProperty> ConvertToReadOnly(IEnumerable<NamedTypeProperty> properties)
        {
            foreach (var property in properties)
            {
                // "name", "dependsOn", "scope" & "parent" can be set for existing resources - everything else should be read-only
                if (UniqueIdentifierProperties.Contains(property.Name) ||
                    property.Name.Equals(LanguageConstants.ResourceDependsOnPropertyName))
                {
                    yield return property;
                }
                else
                {
                    yield return new NamedTypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags));
                }
            }
        }

        private static TypePropertyFlags ConvertToReadOnly(TypePropertyFlags typePropertyFlags)
            => (typePropertyFlags | TypePropertyFlags.ReadOnly) & ~TypePropertyFlags.Required;

        public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            if (!HasDefinedType(typeReference))
            {
                return null;
            }

            // It's important to cache this result because generating the resource type is an expensive operation
            var resourceType = definedTypeCache.GetOrAdd(flags, typeReference, () =>
           {
               var resourceType = resourceTypeLoader.LoadType(typeReference);

               return SetBicepResourceProperties(resourceType, flags);
           });

            return new(
                declaringNamespace,
                resourceType.TypeReference,
                resourceType.ValidParentScopes,
                resourceType.ReadOnlyScopes,
                resourceType.Flags,
                resourceType.Body,
                UniqueIdentifierProperties);
        }

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            if (typeReference.ApiVersion is null ||
                !ResourceTypePattern.IsMatch(typeReference.FormatType()) ||
                !ApiVersionPattern.IsMatch(typeReference.ApiVersion))
            {
                return null;
            }

            // It's important to cache this result because generating the resource type is an expensive operation
            var resourceType = generatedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = new ResourceTypeComponents(
                    typeReference,
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                    ResourceScope.None,
                    ResourceFlags.None,
                    CreateGenericResourceBody(typeReference, p => true));

                return SetBicepResourceProperties(resourceType, flags);
            });

            return new(
                declaringNamespace,
                resourceType.TypeReference,
                resourceType.ValidParentScopes,
                resourceType.ReadOnlyScopes,
                resourceType.Flags,
                resourceType.Body,
                UniqueIdentifierProperties);
        }

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;
    }
}
