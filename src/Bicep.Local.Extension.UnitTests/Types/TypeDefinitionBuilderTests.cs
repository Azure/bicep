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
using Bicep.Local.Extension.Builder.Models;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Attributes;
using FluentAssertions;

namespace Bicep.Local.Extension.UnitTests.TypesTests;

[TestClass]
public class TypeDefinitionBuilderTests
{
    private record TestUnsupportedProperty(DateTime When);

    private record SimpleResource(string Name = "", string AnotherString = "");

    private static readonly ExtensionInfo extensionInfo = new("TestSettings", "2025-01-01", true);

    [TestMethod]
    public void GenerateTypeDefinition_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);
        var result = GenerateTypes(builder);
        result.GetAvailableTypes().Should().BeEmpty();
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_On_Unsupported_Property_Type()
    {
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(TestUnsupportedProperty), new("TestUnsupportedProperty"))]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>();
    }

    private record ArrayResource(string[] Items);

    private record EnumerableResource(IEnumerable<string> Items);

    private record ArrayLikeResource(
        ImmutableArray<string> ImmutableItems,
        HashSet<string> HashSetItems);

    public enum SampleEnum
    {
        First,
        Second,
        Third
    }

    private record EnumResource(
        SampleEnum NonNullableEnum,
        SampleEnum? NullableEnum);

    private record DictionaryResource(
        Dictionary<string, string> Dict,
        ImmutableDictionary<string, string> ImmutableDict);

    private record PrimitiveResource
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; } = "";
        public bool BoolProp { get; init; }
        public int? NullableIntProp { get; init; }
        public bool? NullableBoolProp { get; init; }
        [TypeProperty("Secure string", isSecure: true)]
        public string SecureStringProp { get; init; } = "";
    }

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
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);
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
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

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
    public void GenerateTypeDefinition_handles_enum_types()
    {
        var resourceType = CreateResourceType(typeof(EnumResource), nameof(EnumResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["nonNullableEnum"].TypeReference.Type.Should().BeOfType<UnionType>()
            .Which.Members.Select(m => m.Type).Should().AllBeOfType<StringLiteralType>()
            .Which.Should().SatisfyRespectively(
                x => x.RawStringValue.Should().Be("First"),
                x => x.RawStringValue.Should().Be("Second"),
                x => x.RawStringValue.Should().Be("Third"));
        body.Properties["nullableEnum"].TypeReference.Type.Should().BeOfType<UnionType>()
            .Which.Members.Select(m => m.Type).Should().SatisfyRespectively(
                x => x.Should().BeOfType<StringLiteralType>().Which.RawStringValue.Should().Be("First"),
                x => x.Should().BeOfType<StringLiteralType>().Which.RawStringValue.Should().Be("Second"),
                x => x.Should().BeOfType<StringLiteralType>().Which.RawStringValue.Should().Be("Third"),
                x => x.Should().BeOfType<NullType>());
    }

    [TestMethod]
    public void GenerateTypeDefinition_handles_dictionary_types()
    {
        var resourceType = CreateResourceType(typeof(DictionaryResource), nameof(DictionaryResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        var dictType = body.Properties["dict"].TypeReference.Type.Should().BeOfType<ObjectType>().Subject;
        dictType.Properties.Should().BeEmpty();
        dictType.AdditionalProperties!.TypeReference.Type.Should().BeOfType<StringType>();
        var immutableDictType = body.Properties["immutableDict"].TypeReference.Type.Should().BeOfType<ObjectType>().Subject;
        immutableDictType.Properties.Should().BeEmpty();
        immutableDictType.AdditionalProperties!.TypeReference.Type.Should().BeOfType<StringType>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_handles_primitive_types()
    {
        var resourceType = CreateResourceType(typeof(PrimitiveResource), nameof(PrimitiveResource));

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["intProp"].TypeReference.Type.Should().BeOfType<IntegerType>();
        body.Properties["stringProp"].TypeReference.Type.Should().BeOfType<StringType>();
        body.Properties["boolProp"].TypeReference.Type.Should().BeOfType<BooleanType>();
        body.Properties["nullableIntProp"].TypeReference.Type.Should().BeOfType<UnionType>()
            .Which.Members.Should().SatisfyRespectively(
                x => x.Type.Should().BeOfType<IntegerType>(),
                x => x.Type.Should().BeOfType<NullType>());
        body.Properties["nullableBoolProp"].TypeReference.Type.Should().BeOfType<UnionType>()
            .Which.Members.Should().SatisfyRespectively(
                x => x.Type.Should().BeOfType<BooleanType>(),
                x => x.Type.Should().BeOfType<NullType>());
        body.Properties["secureStringProp"].TypeReference.Type.Should().BeOfType<StringType>()
            .Which.ValidationFlags.Should().HaveFlag(TypeSymbolValidationFlags.IsSecure);
    }

    #region JSON Structure Validation Tests

    // Dedicated resource types for JSON structure tests - isolated from other tests
    private record JsonTestResource1(string Property1 = "");
    private record JsonTestResource2(int Property2 = 0);
    private record JsonTestResource3(bool Property3 = false);

    [TestMethod]
    public void GenerateTypeDefinition_IndexFile_Contains_Resources_Block()
    {
        // Arrange
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(JsonTestResource1), new ResourceTypeAttribute("TestResource", "v1"))
        ]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);

        // Act
        var result = builder.GenerateTypeDefinition();

        // Assert
        result.IndexFileContent.Should().NotBeNullOrEmpty("index file must be generated");

        var indexJson = System.Text.Json.JsonDocument.Parse(result.IndexFileContent);
        var root = indexJson.RootElement;

        // Verify resources block exists
        var hasResources = root.TryGetProperty("resources", out var resourcesElement);
        hasResources.Should().BeTrue("index.json must contain 'resources' block");

        resourcesElement.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Object,
            "resources must be an object");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Resources_Block_Contains_Expected_Resource()
    {
        // Arrange
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(JsonTestResource1), new ResourceTypeAttribute("TestResource", "v1"))
        ]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);

        // Act
        var result = builder.GenerateTypeDefinition();

        // Assert
        var indexJson = System.Text.Json.JsonDocument.Parse(result.IndexFileContent);
        var resourcesElement = indexJson.RootElement.GetProperty("resources");

        // Should contain the resource key
        var hasResourceEntry = resourcesElement.TryGetProperty("TestResource@v1", out var resourceEntry);
        hasResourceEntry.Should().BeTrue("resources block should contain entry for 'TestResource@v1'");

        // Should have $ref property
        var hasRef = resourceEntry.TryGetProperty("$ref", out var refElement);
        hasRef.Should().BeTrue("resource entry must have '$ref' property");

        refElement.GetString().Should().StartWith("types.json#/",
            "resource should reference types.json");
    }

    [TestMethod]
    public void GenerateTypeDefinition_IndexFile_Has_All_Required_Top_Level_Properties()
    {
        // Arrange
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(JsonTestResource1), new ResourceTypeAttribute("TestResource"))
        ]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);

        // Act
        var result = builder.GenerateTypeDefinition();

        // Assert
        var indexJson = System.Text.Json.JsonDocument.Parse(result.IndexFileContent);
        var root = indexJson.RootElement;

        // Verify all required top-level properties exist
        root.TryGetProperty("resources", out _).Should().BeTrue(
            "index.json must contain 'resources' property");

        root.TryGetProperty("resourceFunctions", out _).Should().BeTrue(
            "index.json must contain 'resourceFunctions' property");

        root.TryGetProperty("settings", out _).Should().BeTrue(
            "index.json must contain 'settings' property");
    }

    [TestMethod]
    public void GenerateTypeDefinition_ResourceType_Entry_Has_Required_Properties()
    {
        // Arrange
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(JsonTestResource1), new ResourceTypeAttribute("TestResource", "v1"))
        ]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(extensionInfo, typeProviderMock.Object);

        // Act
        var result = builder.GenerateTypeDefinition();

        // Assert
        var typesJson = System.Text.Json.JsonDocument.Parse(result.TypeFileContents["types.json"]);
        var resourceType = typesJson.RootElement.EnumerateArray()
            .FirstOrDefault(t => t.TryGetProperty("$type", out var tp) && tp.GetString() == "ResourceType");

        resourceType.ValueKind.Should().NotBe(System.Text.Json.JsonValueKind.Undefined,
            "ResourceType entry should exist");

        var hasName = resourceType.TryGetProperty("name", out _);
        hasName.Should().BeTrue("ResourceType must have 'name' property");

        var hasBody = resourceType.TryGetProperty("body", out var bodyProperty);
        hasBody.Should().BeTrue("ResourceType must have 'body' property");

        var hasRef = bodyProperty.TryGetProperty("$ref", out _);
        hasRef.Should().BeTrue("ResourceType body must have '$ref' property");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Settings_Contains_Extension_Info()
    {
        // Arrange
        var customExtensionInfo = new ExtensionInfo("TestExtension", "2025-01-01", true);
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.SetupGet(tp => tp.ConfigurationType).Returns(() => null!);
        typeProviderMock.SetupGet(tp => tp.FallbackType).Returns(() => null!);

        var builder = new TypeDefinitionBuilder(customExtensionInfo, typeProviderMock.Object);

        // Act
        var result = builder.GenerateTypeDefinition();

        // Assert
        var indexJson = System.Text.Json.JsonDocument.Parse(result.IndexFileContent);
        var settings = indexJson.RootElement.GetProperty("settings");

        settings.GetProperty("name").GetString().Should().Be("TestExtension");
        settings.GetProperty("version").GetString().Should().Be("2025-01-01");
        settings.GetProperty("isSingleton").GetBoolean().Should().BeTrue();
    }

    #endregion
}
