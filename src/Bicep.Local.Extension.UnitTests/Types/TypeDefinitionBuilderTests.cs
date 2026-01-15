// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Builder;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Models;
using Bicep.Local.Extension.Types.Attributes;
using FluentAssertions;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;

using static Microsoft.WindowsAzure.ResourceStack.Common.Utilities.FastActivator;

namespace Bicep.Local.Extension.UnitTests.TypesTests;

[TestClass]
public class TypeDefinitionBuilderTests
{
    private record TestUnsupportedProperty(DateTime When);

    private record SimpleResource(string Name = "", string AnotherString = "");

    private record SimpleConfiguration(string ConfigValue = "", int ConfigNumber = 0);

    private record ComplexConfiguration(
        string RequiredSetting = "",
        string? OptionalSetting = null,
        int MaxRetries = 3,
        bool EnableFeature = false);

    private record NestedConfiguration(
        string Name = "",
        SimpleConfiguration? Nested = null);

    private record FallbackResource(string Message = "", int Code = 0);

    private static BicepExtensionInfo CreateExtensionInfo() =>
        new(Name: "TestExtension", Version: "2025-01-01", IsSingleton: true);

    private TypeFactory CreateTypeFactory() => new([]);

    private TypeDefinitionBuilderOptions CreateOptions((Type Type, Func<TypeBase> Factory)[]? map = null, Type? configurationType = null)
    {
        var dictionary = map is null
           ? FrozenDictionary<Type, Func<TypeBase>>.Empty
           : map.ToFrozenDictionary(kvp => kvp.Type, kvp => kvp.Factory);
        return new TypeDefinitionBuilderOptions(dictionary, configurationType);
    }

    /// <summary>
    /// Creates the common test infrastructure components used across most tests.
    /// </summary>
    /// <returns>A tuple containing extension info, type factory, and mocked type provider.</returns>
    private (BicepExtensionInfo ExtensionInfo, TypeFactory Factory, Mock<ITypeProvider> TypeProviderMock) CreateTestInfrastructure()
    {
        var extensionInfo = CreateExtensionInfo();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();

        return (extensionInfo, factory, typeProviderMock);
    }

    #region Constructor Tests
    [TestMethod]
    public void GenerateTypeDefinition_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var extensionInfo = CreateExtensionInfo();
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        var options = CreateOptions();

        Action act = () => new TypeDefinitionBuilder(extensionInfo, typeFactory, typeProvider, options);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_On_Unsupported_Property_Type()
    {
        var extensionInfo = CreateExtensionInfo();
        var factory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        var options = CreateOptions([(typeof(string), () => new StringType())]);
        var generator = new TypeDefinitionBuilder(extensionInfo, factory, typeProvider, options);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_With_ConfigurationType()
    {
        var extensionInfo = CreateExtensionInfo();
        var factory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        var options = CreateOptions(
            map: [(typeof(string), () => new StringType()), (typeof(int), () => new IntegerType())],
            configurationType: typeof(SimpleConfiguration));

        var generator = new TypeDefinitionBuilder(extensionInfo, factory, typeProvider, options);

        generator.Should().NotBeNull();
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_Options()
    {
        var extensionInfo = CreateExtensionInfo();
        var factory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        Action act = () => new TypeDefinitionBuilder(extensionInfo, factory, typeProvider, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion Constructor Tests

    #region Configuration Type Tests

    [TestMethod]
    public void GenerateTypeDefinition_Without_ConfigurationType_DoesNotIncludeConfigInIndex()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(
            map: [(typeof(string), () => new StringType())],
            configurationType: null);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().NotBeNullOrEmpty();
        result.IndexFileContent.Should().Contain("\"configurationType\": null",
            because: "no configuration type was specified");
    }

    [TestMethod]
    public void GenerateTypeDefinition_With_SimpleConfigurationType_IncludesConfigInIndex()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(
            map: [(typeof(string), () => new StringType()), (typeof(int), () => new IntegerType())],
            configurationType: typeof(SimpleConfiguration));

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("configurationType",
            because: "a configuration type was specified");
        result.TypeFileContents.Values.Single().Should().Contain("SimpleConfiguration",
            because: "the configuration type should be included in types");
        result.TypeFileContents.Values.Single().Should().Contain("configValue",
            because: "configuration properties should be camelCased");
        result.TypeFileContents.Values.Single().Should().Contain("configNumber",
            because: "all configuration properties should be included");
    }

    [TestMethod]
    public void GenerateTypeDefinition_With_ComplexConfigurationType_IncludesAllProperties()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(
            map: [
                (typeof(string), () => new StringType()),
                (typeof(int), () => new IntegerType()),
                (typeof(bool), () => new BooleanType())
            ],
            configurationType: typeof(ComplexConfiguration));

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();

        typesContent.Should().Contain("ComplexConfiguration");
        typesContent.Should().Contain("requiredSetting");
        typesContent.Should().Contain("optionalSetting");
        typesContent.Should().Contain("maxRetries");
        typesContent.Should().Contain("enableFeature");
    }

    [TestMethod]
    public void GenerateTypeDefinition_With_NestedConfigurationType_HandlesNestedObjects()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(
            map: [
                (typeof(string), () => new StringType()),
                (typeof(int), () => new IntegerType())
            ],
            configurationType: typeof(NestedConfiguration));

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();

        typesContent.Should().Contain("NestedConfiguration");
        typesContent.Should().Contain("SimpleConfiguration",
            because: "nested configuration types should be included");
        typesContent.Should().Contain("nested");
    }

    [TestMethod]
    public void GenerateTypeDefinition_With_ConfigurationType_And_Resources_IncludesBoth()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(SimpleResource),
                Attribute: new ResourceTypeAttribute("SimpleResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(
            map: [
                (typeof(string), () => new StringType()),
                (typeof(int), () => new IntegerType())
            ],
            configurationType: typeof(SimpleConfiguration));

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("SimpleResource",
            because: "resource types should be in the index");
        result.IndexFileContent.Should().Contain("configurationType",
            because: "configuration type should be in the index");

        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("SimpleResource");
        typesContent.Should().Contain("SimpleConfiguration");
    }

    #endregion Configuration Type Tests

    #region Resource Type Tests

    [TestMethod]
    public void GenerateTypeDefinition_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions([(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

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

        result.Should().NotBeNull();
        result.IndexFileContent.Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().Contain("[]",
            because: "the types JSON should be an empty array '[]' when no resource types are generated");
    }

    private static ResourceTypeComponents CreateResourceType(Type type, string typeName, string? apiVersion = null)
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(SimpleResource),
                Attribute: new ResourceTypeAttribute("SimpleResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions([(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        resourceType.TypeReference.Name.Should().Be(typeName);
        resourceType.TypeReference.ApiVersion.Should().Be(apiVersion);

        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("SimpleResource");
        result.TypeFileContents.Values.Single().Should().Contain("SimpleResource");
        result.TypeFileContents.Values.Single().Should().Contain("name",
            because: "the property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_Multiple_Resources()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(SimpleResource),
                Attribute: new ResourceTypeAttribute("SimpleResource")),
            new ResourceTypeDefinitionDetails(
                Type: typeof(ArrayResource),
                Attribute: new ResourceTypeAttribute("ArrayResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions([(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("SimpleResource");
        result.IndexFileContent.Should().Contain("ArrayResource");

        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("SimpleResource");
        typesContent.Should().Contain("ArrayResource");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_On_Unsupported_Property_Type()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(TestUnsupportedProperty),
                Attribute: new ResourceTypeAttribute("TestUnsupportedProperty"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions([(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>()
            .WithMessage("*DateTime*", because: "DateTime is not a supported property type");
    }

    #endregion Resource Type Tests

    #region Array and Collection Tests

    private record ArrayResource(string[] Items);

        var body = resourceType.Body.Type.Should().BeOfType<ObjectType>().Subject;
        body.Properties["items"].TypeReference.Type.Should().BeOfType<TypedArrayType>()
            .Which.Item.Type.Should().BeOfType<StringType>();
    }

    private record ComplexArrayResource(SimpleResource[] Resources);

    [TestMethod]
    public void GenerateTypeDefinition_handles_collection_types()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(ArrayResource),
                Attribute: new ResourceTypeAttribute("ArrayResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

    [TestMethod]
    public void GenerateTypeDefinition_handles_enum_types()
    {
        var resourceType = CreateResourceType(typeof(EnumResource), nameof(EnumResource));

        result.Should().NotBeNull();
        result.TypeFileContents.Values.Single().Should().Contain("ArrayResource");
        result.TypeFileContents.Values.Single().Should().Contain("items",
            because: "the array property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_handles_dictionary_types()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(EnumerableResource),
                Attribute: new ResourceTypeAttribute("EnumerableResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);
        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

    [TestMethod]
    public void GenerateTypeDefinition_handles_primitive_types()
    {
        var resourceType = CreateResourceType(typeof(PrimitiveResource), nameof(PrimitiveResource));

        result.Should().NotBeNull();
        result.TypeFileContents.Values.Single().Should().Contain("EnumerableResource");
        result.TypeFileContents.Values.Single().Should().Contain("items",
            because: "the enumerable property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_ComplexObjectArray()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(ComplexArrayResource),
                Attribute: new ResourceTypeAttribute("ComplexArrayResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);
        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("ComplexArrayResource");
        typesContent.Should().Contain("SimpleResource",
            because: "the array element type should be defined");
        typesContent.Should().Contain("resources");
    }

    #endregion Array and Collection Tests

    #region Enum Tests

    private enum TestEnum { Value1, Value2, Value3 }

    private record EnumResource(TestEnum? Status);

    [TestMethod]
    public void GenerateTypeDefinition_Emits_UnionType_For_NullableEnum()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(EnumResource),
                Attribute: new ResourceTypeAttribute("EnumResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("Value1");
        typesContent.Should().Contain("Value2");
        typesContent.Should().Contain("Value3");
    }

    #endregion Enum Tests

    #region Property Name Tests

    private record CamelCaseTestResource(string FirstName, string LastName, string XMLData);

    [TestMethod]
    public void GenerateTypeDefinition_Converts_PropertyNames_To_CamelCase()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(CamelCaseTestResource),
                Attribute: new ResourceTypeAttribute("CamelCaseTest"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("firstName");
        typesContent.Should().Contain("lastName");
        typesContent.Should().Contain("xMLData",
            because: "property names should be converted to camelCase");
    }

    #endregion Property Name Tests

    #region Type Mapping Tests

    private record MultiTypeResource(string Text, int Number, bool Flag);

    [TestMethod]
    public void GenerateTypeDefinition_Maps_Multiple_Types_Correctly()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(MultiTypeResource),
                Attribute: new ResourceTypeAttribute("MultiTypeResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(map: [
            (typeof(string), () => new StringType()),
            (typeof(int), () => new IntegerType()),
            (typeof(bool), () => new BooleanType())
        ]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        var typesContent = result.TypeFileContents.Values.Single();
        typesContent.Should().Contain("text");
        typesContent.Should().Contain("number");
        typesContent.Should().Contain("flag");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_When_Type_Not_In_Map()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            new ResourceTypeDefinitionDetails(
                Type: typeof(MultiTypeResource),
                Attribute: new ResourceTypeAttribute("MultiTypeResource"))
        ]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        // Only provide string mapping, missing int and bool
        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>();
    }

    #endregion Type Mapping Tests

    #region Extension Info Tests

    [TestMethod]
    public void GenerateTypeDefinition_Includes_ExtensionInfo_In_Index()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();

        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
        typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();
        
        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("TestExtension");
        result.IndexFileContent.Should().Contain("2025-01-01");
    }

        #endregion Extension Info Tests

        #region Fallback Type Tests

        [TestMethod]
        public void GenerateTypeDefinition_Without_FallbackType_DoesNotIncludeFallbackInIndex()
        {
            var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
            typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
            typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns((ResourceTypeDefinitionDetails?)null);

            var options = CreateOptions([(typeof(string), () => new StringType())]);
            var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

            var result = builder.GenerateTypeDefinition();

            result.Should().NotBeNull();
            result.IndexFileContent.Should().NotBeNullOrEmpty();
            result.IndexFileContent.Should().Contain("\"fallbackResourceType\": null",
                because: "no fallback resource type was specified");
        }

        [TestMethod]
        public void GenerateTypeDefinition_With_FallbackType_IncludesFallbackInIndex()
        {
            var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
            typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);
            typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns(
                new ResourceTypeDefinitionDetails(
                    Type: typeof(FallbackResource),
                    Attribute: new ResourceTypeAttribute("FallbackResource")));

            var options = CreateOptions([
                (typeof(string), () => new StringType()),
                (typeof(int), () => new IntegerType())
            ]);
            var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

            var result = builder.GenerateTypeDefinition();

            result.Should().NotBeNull();
            result.IndexFileContent.Should().Contain("fallbackResourceType",
                because: "a fallback resource type was specified");
            result.TypeFileContents.Values.Single().Should().Contain("FallbackResource",
                because: "the fallback resource type should be included in types");
            result.TypeFileContents.Values.Single().Should().Contain("message",
                because: "fallback resource properties should be camelCased");
            result.TypeFileContents.Values.Single().Should().Contain("code",
                because: "all fallback resource properties should be included");
        }

        [TestMethod]
        public void GenerateTypeDefinition_With_FallbackType_And_Resources_IncludesBoth()
        {
            var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
            typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
                new ResourceTypeDefinitionDetails(
                    Type: typeof(SimpleResource),
                    Attribute: new ResourceTypeAttribute("SimpleResource"))
            ]);
            typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns(
                new ResourceTypeDefinitionDetails(
                    Type: typeof(FallbackResource),
                    Attribute: new ResourceTypeAttribute("FallbackResource")));

            var options = CreateOptions([
                (typeof(string), () => new StringType()),
                (typeof(int), () => new IntegerType())
            ]);
            var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

            var result = builder.GenerateTypeDefinition();

            result.Should().NotBeNull();
            result.IndexFileContent.Should().Contain("SimpleResource",
                because: "resource types should be in the index");
            result.IndexFileContent.Should().Contain("fallbackResourceType",
                because: "fallback resource type should be in the index");

            var typesContent = result.TypeFileContents.Values.Single();
            typesContent.Should().Contain("SimpleResource");
            typesContent.Should().Contain("FallbackResource");
        }

        [TestMethod]
        public void GenerateTypeDefinition_With_FallbackType_ConfigurationType_And_Resources_IncludesAll()
        {
            var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
            typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
                new ResourceTypeDefinitionDetails(
                    Type: typeof(SimpleResource),
                    Attribute: new ResourceTypeAttribute("SimpleResource"))
            ]);
            typeProviderMock.Setup(tp => tp.GetFallbackType()).Returns(
                new ResourceTypeDefinitionDetails(
                    Type: typeof(FallbackResource),
                    Attribute: new ResourceTypeAttribute("FallbackResource")));

            var options = CreateOptions(
                map: [
                    (typeof(string), () => new StringType()),
                    (typeof(int), () => new IntegerType())
                ],
                configurationType: typeof(SimpleConfiguration));
            var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

            var result = builder.GenerateTypeDefinition();

            result.Should().NotBeNull();
            result.IndexFileContent.Should().Contain("SimpleResource",
                because: "resource types should be in the index");
            result.IndexFileContent.Should().Contain("configurationType",
                because: "configuration type should be in the index");
            result.IndexFileContent.Should().Contain("fallbackResourceType",
                because: "fallback resource type should be in the index");

            var typesContent = result.TypeFileContents.Values.Single();
            typesContent.Should().Contain("SimpleResource");
            typesContent.Should().Contain("SimpleConfiguration");
            typesContent.Should().Contain("FallbackResource");
        }

        #endregion Fallback Type Tests
    }
