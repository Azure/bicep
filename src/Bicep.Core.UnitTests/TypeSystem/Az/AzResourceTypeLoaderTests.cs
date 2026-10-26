// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzTypes = Azure.Bicep.Types.Concrete;

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeLoaderTests
    {
        [TestMethod]
        public void LoadType_includes_only_list_prefixed_resource_functions()
        {
            var factory = new TypeFactory([]);
            var stringType = factory.Create(() => new AzTypes.StringType());

            var resourceBodyType = factory.Create(() => new AzTypes.ObjectType(
                "TestResourceBody",
                new Dictionary<string, ObjectTypeProperty>
                {
                    ["name"] = new(factory.GetReference(stringType), ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "Resource name"),
                },
                null,
                null));

            var resourceType = factory.Create(() => new AzTypes.ResourceType(
                "Test.Rp/testResources@2020-01-01",
                factory.GetReference(resourceBodyType),
                functions: null,
                writableScopes_in: ScopeType.ResourceGroup,
                readableScopes_in: ScopeType.ResourceGroup,
                scopeType: null,
                readOnlyScopes: null,
                flags: null));

            var listFunctionType = factory.Create(() => new ResourceFunctionType(
                "listSecrets",
                "Test.Rp/testResources",
                "2020-01-01",
                factory.GetReference(stringType),
                factory.GetReference(stringType)));

            var nonListFunctionType = factory.Create(() => new ResourceFunctionType(
                "getSecrets",
                "Test.Rp/testResources",
                "2020-01-01",
                factory.GetReference(stringType),
                factory.GetReference(stringType)));

            var resourceRef = new CrossFileTypeReference("types.json", factory.GetIndex(resourceType));
            var listFunctionRef = new CrossFileTypeReference("types.json", factory.GetIndex(listFunctionType));
            var nonListFunctionRef = new CrossFileTypeReference("types.json", factory.GetIndex(nonListFunctionType));

            var index = new TypeIndex(
                resources: new Dictionary<string, CrossFileTypeReference>
                {
                    [resourceType.Name] = resourceRef,
                },
                resourceFunctions: new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>
                {
                    ["Test.Rp/testResources"] = new Dictionary<string, IReadOnlyList<CrossFileTypeReference>>
                    {
                        ["2020-01-01"] = [listFunctionRef, nonListFunctionRef],
                    },
                },
                namespaceFunctions: [],
                settings: new TypeSettings("Test", "1.0.0", isSingleton: false, isPreview: null, isDeprecated: null, configurationType: null),
                fallbackResourceType: null);

            var typeLoader = StrictMock.Of<ITypeLoader>();
            typeLoader.Setup(x => x.LoadTypeIndex()).Returns(index);
            typeLoader.Setup(x => x.LoadResourceType(resourceRef)).Returns(resourceType);
            typeLoader.Setup(x => x.LoadResourceFunctionType(listFunctionRef)).Returns(listFunctionType);
            typeLoader.Setup(x => x.LoadResourceFunctionType(nonListFunctionRef)).Returns(nonListFunctionType);

            var loader = new AzResourceTypeLoader(typeLoader.Object);

            var loaded = loader.LoadType(ResourceTypeReference.Parse(resourceType.Name));
            var bodyType = loaded.Body.Type.Should().BeOfType<Bicep.Core.TypeSystem.Types.ObjectType>().Subject;
            var functionSymbols = bodyType.MethodResolver.GetKnownFunctions();

            functionSymbols.Keys.Should().BeEquivalentTo(["listSecrets"]);
            functionSymbols["listSecrets"].Overloads.Should().HaveCount(2);
        }
    }
}
