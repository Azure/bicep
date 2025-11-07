// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Builder;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Types;
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
    public void Constructor_Throws_On_Empty_TypeToTypeBaseMap()
    {
        var extensionInfo = CreateExtensionInfo();
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        var options = CreateOptions();

        Action act = () => new TypeDefinitionBuilder(extensionInfo, typeFactory, typeProvider, options);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Valid_Arguments()
    {
        var extensionInfo = CreateExtensionInfo();
        var factory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        var options = CreateOptions([(typeof(string), () => new StringType())]);
        var generator = new TypeDefinitionBuilder(extensionInfo, factory, typeProvider, options);

        generator.Should().NotBeNull();
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
            (typeof(SimpleResource), new ResourceTypeAttribute("SimpleResource"))
        ]);

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

        var options = CreateOptions([(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().Contain("[]",
            because: "the types JSON should be an empty array '[]' when no resource types are generated");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_Resource_When_TypeProvider_Has_Types()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(SimpleResource), new ResourceTypeAttribute("SimpleResource"))
        ]);

        var options = CreateOptions([(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

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
            (typeof(SimpleResource), new ResourceTypeAttribute("SimpleResource")),
            (typeof(ArrayResource), new ResourceTypeAttribute("ArrayResource"))
        ]);

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
            (typeof(TestUnsupportedProperty), new ResourceTypeAttribute("TestUnsupportedProperty"))
        ]);

        var options = CreateOptions([(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>()
            .WithMessage("*DateTime*", because: "DateTime is not a supported property type");
    }

    #endregion Resource Type Tests

    #region Array and Collection Tests

    private record ArrayResource(string[] Items);

    private record EnumerableResource(IEnumerable<string> Items);

    private record ComplexArrayResource(SimpleResource[] Resources);

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_ArrayProperty()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(ArrayResource), new ResourceTypeAttribute("ArrayResource"))
        ]);

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.TypeFileContents.Values.Single().Should().Contain("ArrayResource");
        result.TypeFileContents.Values.Single().Should().Contain("items",
            because: "the array property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_IEnumerableProperty()
    {
        var (extensionInfo, factory, typeProviderMock) = CreateTestInfrastructure();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([
            (typeof(EnumerableResource), new ResourceTypeAttribute("EnumerableResource"))
        ]);
        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);

        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();

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
            (typeof(ComplexArrayResource), new ResourceTypeAttribute("ComplexArrayResource"))
        ]);
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
            (typeof(EnumResource), new ResourceTypeAttribute("EnumResource"))
        ]);

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
            (typeof(CamelCaseTestResource), new ResourceTypeAttribute("CamelCaseTest"))
        ]);

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
            (typeof(MultiTypeResource), new ResourceTypeAttribute("MultiTypeResource"))
        ]);

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
            (typeof(MultiTypeResource), new ResourceTypeAttribute("MultiTypeResource"))
        ]);

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

        var options = CreateOptions(map: [(typeof(string), () => new StringType())]);
        var builder = new TypeDefinitionBuilder(extensionInfo, factory, typeProviderMock.Object, options);

        var result = builder.GenerateTypeDefinition();
        
        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("TestExtension");
        result.IndexFileContent.Should().Contain("2025-01-01");
    }

    #endregion Extension Info Tests
}
