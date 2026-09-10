// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.UnitTests.Mock;

namespace Bicep.McpServer.UnitTests.Helpers;

public static class ResourceTypeCatalogHelper
{
    public static ITypeLoader CreateTypeLoader(params string[] resourceTypeNames)
    {
        var factory = new TypeFactory([]);
        var stringType = factory.Create(() => new StringType());
        var bodyType = factory.Create(() => new ObjectType(
            "TestResourceBody",
            new Dictionary<string, ObjectTypeProperty>
            {
                ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "Resource name"),
                ["id"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.ReadOnly, "Resource ID"),
            },
            null,
            null));

        var typeLoader = StrictMock.Of<ITypeLoader>();
        var resources = new Dictionary<string, CrossFileTypeReference>();
        var resourceFunctions = new Dictionary<string, Dictionary<string, IReadOnlyList<CrossFileTypeReference>>>();

        foreach (var resourceTypeName in resourceTypeNames)
        {
            var reference = ResourceTypeReference.Parse(resourceTypeName);
            var resourceType = factory.Create(() => new ResourceType(
                resourceTypeName,
                factory.GetReference(bodyType),
                functions: null,
                writableScopes_in: ScopeType.ResourceGroup,
                readableScopes_in: ScopeType.ResourceGroup,
                scopeType: null,
                readOnlyScopes: null,
                flags: null));
            var resourceReference = new CrossFileTypeReference("types.json", factory.GetIndex(resourceType));
            resources.Add(resourceTypeName, resourceReference);
            typeLoader.Setup(loader => loader.LoadResourceType(resourceReference)).Returns(resourceType);

            if (reference.ApiVersion is { } apiVersion)
            {
                var functionType = factory.Create(() => new ResourceFunctionType(
                    "listSecrets",
                    reference.FormatType(),
                    apiVersion,
                    factory.GetReference(stringType),
                    factory.GetReference(stringType)));
                var functionReference = new CrossFileTypeReference("types.json", factory.GetIndex(functionType));
                if (!resourceFunctions.TryGetValue(reference.FormatType(), out var functionsByVersion))
                {
                    functionsByVersion = new Dictionary<string, IReadOnlyList<CrossFileTypeReference>>();
                    resourceFunctions.Add(reference.FormatType(), functionsByVersion);
                }

                functionsByVersion.Add(apiVersion, [functionReference]);
                typeLoader.Setup(loader => loader.LoadResourceFunctionType(functionReference)).Returns(functionType);
            }
        }

        var index = new TypeIndex(
            resources: resources,
            resourceFunctions: resourceFunctions.ToDictionary(
                entry => entry.Key,
                entry => (IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>)entry.Value),
            namespaceFunctions: [],
            settings: null,
            fallbackResourceType: null);
        typeLoader.Setup(loader => loader.LoadTypeIndex()).Returns(index);

        return typeLoader.Object;
    }
}