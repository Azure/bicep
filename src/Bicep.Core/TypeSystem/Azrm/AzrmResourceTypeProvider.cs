// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Azrm
{
    public class AzrmResourceTypeProvider : IResourceTypeProvider
    {
        private static IEnumerable<TypeProperty> GetCommonResourceProperties(ResourceTypeReference reference)
            => new []
            {
                new TypeProperty("id", LanguageConstants.String, TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.SkipInlining),
                new TypeProperty("type", new StringLiteralType(reference.FullyQualifiedType), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining),
                new TypeProperty("apiVersion", new StringLiteralType(reference.ApiVersion), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining),
            };

        private static (ResourceTypeReference, Func<ResourceType>) Get_Microsoft_Resources_resourceGroups_2020_06_01()
        {
            // hand crafted from https://github.com/Azure/azure-resource-manager-schemas/blob/1beac911/schemas/2020-06-01/Microsoft.Resources.json
            var reference = ResourceTypeReference.Parse("Microsoft.Resources/resourceGroups@2020-06-01");

            return (reference, () => new ResourceType(
                reference,
                GetCommonResourceProperties(reference).Concat(
                    new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("tags", LanguageConstants.Tags, TypePropertyFlags.None)
                )));
        }

        private readonly IReadOnlyDictionary<ResourceTypeReference, Func<ResourceType>> manualTypeDefinitions = new []
        {
            Get_Microsoft_Resources_resourceGroups_2020_06_01(),
        }.ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2, ResourceTypeReferenceComparer.Instance);

        private ResourceType? TryLookupResource(ResourceTypeReference typeReference)
            => manualTypeDefinitions.TryGetValue(typeReference, out var builderFunc) ? builderFunc() : null;

        public ResourceType LookupType(ResourceTypeReference typeReference)
        {
            var resourceType = TryLookupResource(typeReference);

            if (resourceType != null)
            {
                return resourceType;
            }

            // TODO move default definition into types assembly
            return new ResourceType(typeReference, LanguageConstants.CreateResourceProperties(typeReference));
        }

        public bool HasTypeDefined(ResourceTypeReference typeReference)
            => TryLookupResource(typeReference) != null;
    }
}