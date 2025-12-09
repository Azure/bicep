// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Types;
using FluentAssertions;

namespace Bicep.Local.Extension.UnitTests.TypesTests;

[TestClass]
public class TypeDefinitionBuilderTests
{
    private record TestUnsupportedProperty(DateTime When);

    private record SimpleResource(string Name = "", string AnotherString = "");

    [TestMethod]
    public void GenerateTypeDefinition_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, typeProviderMock.Object);
        var result = GenerateTypes(builder);
        result.GetAvailableTypes().Should().BeEmpty();
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_On_Unsupported_Property_Type()
    {
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(TestUnsupportedProperty), new("TestUnsupportedProperty"))]);

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, typeProviderMock.Object);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>();
    }

    private record ArrayResource(string[] Items);

    private record EnumerableResource(IEnumerable<string> Items);

    private record ArrayLikeResource(
        ImmutableArray<string> ImmutableItems,
        HashSet<string> HashSetItems);

    private record DictionaryResource(
        Dictionary<string, string> Dict,
        ImmutableDictionary<string, string> ImmutableDict);

    private static IResourceTypeLoader GenerateTypes(TypeDefinitionBuilder builder)
    {
        var result = builder.GenerateTypeDefinition();
        var types = new Dictionary<string, string>(result.TypeFileContents);
        types["index.json"] = result.IndexFileContent;

        var typeLoader = new ArchivedTypeLoader(types.ToFrozenDictionary(x => x.Key, x => BinaryData.FromString(x.Value)));

        return new ExtensionResourceTypeLoader(typeLoader);
    }

    private static ResourceTypeComponents CreateResourceType(Type type, string typeName, string? apiVersion = null)
    {
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(type, new(typeName, apiVersion))]);

        var builder = new TypeDefinitionBuilder("TestSettings", "0.0.1", true, null, typeProviderMock.Object);
        var typeLoader = GenerateTypes(builder);
        var resourceType = typeLoader.LoadType(typeLoader.GetAvailableTypes().Single());

        resourceType.TypeReference.Name.Should().Be(typeName);
        resourceType.TypeReference.ApiVersion.Should().Be(apiVersion);

        return resourceType;
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_Resource_When_TypeProvider_Has_Types()
    {
        var resourceType = CreateResourceType(typeof(SimpleResource), nameof(SimpleResource));
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(SimpleResource), new("SimpleResource"))]);

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["name"].TypeReference.Type.Should().BeOfType<StringType>();
        body.Properties["anotherString"].TypeReference.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_ArrayProperty()
    {
        var resourceType = CreateResourceType(typeof(ArrayResource), nameof(ArrayResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["items"].TypeReference.Type.Should().BeOfType<TypedArrayType>()
            .Which.Item.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_IEnumerableProperty()
    {
        var resourceType = CreateResourceType(typeof(EnumerableResource), nameof(EnumerableResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["items"].TypeReference.Type.Should().BeOfType<TypedArrayType>()
            .Which.Item.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_handles_collection_types()
    {
        var resourceType = CreateResourceType(typeof(ArrayLikeResource), nameof(ArrayLikeResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["immutableItems"].TypeReference.Type.Should().BeOfType<TypedArrayType>()
            .Which.Item.Type.Should().BeOfType<StringType>();
        body.Properties["hashSetItems"].TypeReference.Type.Should().BeOfType<TypedArrayType>()
            .Which.Item.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_doesnt_support_dictionary_types()
    {
        Action act = () => CreateResourceType(typeof(DictionaryResource), nameof(DictionaryResource));
        act.Should().Throw<NotImplementedException>()
            .Which.Message.Should().Be("Property 'Dict' references unsupported dictionary type: 'System.Collections.Generic.Dictionary`2[System.String,System.String]'");
    }
}
