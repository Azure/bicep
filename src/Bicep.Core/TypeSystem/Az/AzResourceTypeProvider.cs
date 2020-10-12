// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.SerializedTypes.Az;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        private readonly IDictionary<string, AzResourceTypeFactory?> typeFactories = new Dictionary<string, AzResourceTypeFactory?>(StringComparer.OrdinalIgnoreCase);
        private readonly Func<string, string, IEnumerable<SerializedTypes.Concrete.TypeBase>> loadTypeFunc;

        public AzResourceTypeProvider(Func<string, string, IEnumerable<Bicep.SerializedTypes.Concrete.TypeBase>> loadTypeFunc)
        {
            this.loadTypeFunc = loadTypeFunc;
        }

        public AzResourceTypeProvider()
            : this(TypeLoader.LoadTypes)
        {
        }

        public ResourceType GetType(ResourceTypeReference typeReference)
        {
            var resourceType = TryGetResource(typeReference);

            if (resourceType != null)
            {
                return resourceType;
            }

            // TODO move default definition into types assembly
            return new ResourceType(typeReference, new NamedObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(typeReference), null), TypeSymbolValidationFlags.Default);
        }

        public bool HasType(ResourceTypeReference typeReference)
            => TryGetResource(typeReference) != null;

        private AzResourceTypeFactory? GetTypeFactory(ResourceTypeReference typeReference)
        {
            var key = $"{typeReference.Namespace}@{typeReference.ApiVersion}";

            if (!typeFactories.TryGetValue(key, out var typeFactory))
            {
                var types = loadTypeFunc(typeReference.Namespace, typeReference.ApiVersion);
                typeFactory = types.Any() ? new AzResourceTypeFactory(types, typeReference.ApiVersion) : null;
                typeFactories[key] = typeFactory;
            }

            return typeFactory;
        }

        private ResourceType? TryGetResource(ResourceTypeReference typeReference)
        {
            var typeFactory = GetTypeFactory(typeReference);
            if (typeFactory == null)
            {
                return null;
            }

            return typeFactory.TryGetResourceType(typeReference);
        }
    }
}