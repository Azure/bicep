// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Configuration;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Extensibility
{
    public class ExtensionResourceTypeLoader : IResourceTypeLoader
    {
        public record NamespaceConfiguration(string Name, string Version, bool IsSingleton, ObjectLikeType? ConfigurationType);

        private readonly ITypeLoader typeLoader;
        private readonly ExtensionResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;
        private readonly ImmutableArray<CrossFileTypeReference> namespaceFunctions;
        private readonly TypeSettings? typeSettings;
        private readonly CrossFileTypeReference? fallbackResourceType;

        public ExtensionResourceTypeLoader(ITypeLoader typeLoader, TypeIndex? typeIndex = null)
        {
            typeIndex ??= typeLoader.LoadTypeIndex();
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new ExtensionResourceTypeFactory(typeIndex.Settings);
            this.availableTypes = typeIndex.Resources.ToImmutableDictionary(x => ResourceTypeReference.Parse(x.Key), x => x.Value);
            this.namespaceFunctions = [.. typeIndex.NamespaceFunctions];
            this.typeSettings = typeIndex.Settings;
            this.fallbackResourceType = typeIndex.FallbackResourceType;
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);

            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }

        public ResourceTypeComponents? LoadFallbackResourceType()
        {
            if (fallbackResourceType != null)
            {
                var serializedResourceType = typeLoader.LoadResourceType(fallbackResourceType);

                return resourceTypeFactory.GetResourceType(serializedResourceType);
            }

            // No fallback type provided in JSON
            return null;
        }

        public NamespaceConfiguration LoadNamespaceConfiguration()
        {
            if (typeSettings == null)
            {
                throw new ArgumentException($"Please provide the following Settings properties: Name, Version, & IsSingleton.");
            }

            if (typeSettings.ConfigurationType is { } reference)
            {
                var serializedConfigurationType = typeLoader.LoadType(reference);

                if (resourceTypeFactory.GetConfigurationType(serializedConfigurationType) is not ObjectLikeType configurationType)
                {
                    throw new InvalidOperationException($"Extension configuration type at index {reference.Index} in \"{reference.RelativePath}\" is not a valid ObjectLikeType.");
                }

                return new(
                    typeSettings.Name,
                    typeSettings.Version,
                    typeSettings.IsSingleton,
                    configurationType);
            }

            return new(
                typeSettings.Name,
                typeSettings.Version,
                typeSettings.IsSingleton,
                null);
        }

        public ImmutableArray<FunctionOverload> LoadNamespaceFunctions()
        {
            var overloads = ImmutableArray.CreateBuilder<FunctionOverload>();
            foreach (var functionLocation in namespaceFunctions)
            {
                var serializedFunctionType = typeLoader.LoadType(functionLocation);
                if (serializedFunctionType is not NamespaceFunctionType functionType)
                {
                    throw new InvalidOperationException($"Namespace function type at index {functionLocation.Index} in \"{functionLocation.RelativePath}\" is not a valid NamespaceFunctionType.");
                }

                var overload = resourceTypeFactory.GetNamespaceFunctionOverload(functionType);
                overloads.Add(overload);
            }

            return overloads.ToImmutable();
        }
    }
}
