// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Microsoft.WindowsAzure.ResourceStack.Common.Utilities.FastActivator;

namespace Bicep.Local.Extension.UnitTests.TypesTests;

[TestClass]
public class TypeDefinitionBuilderTests
{
    private record TestUnsupportedProperty(DateTime When);

    private record SimpleResource(string Name = "", string AnotherString = "");

    private static TypeSettings CreateTypeSettings() =>
        new("TestSettings", "2025-01-01", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));

    private TypeFactory CreateTypeFactory() => new([]);

    #region Constructor Tests
    [TestMethod]
    public void Constructor_Throws_On_Empty_TypeToTypeBaseMap()
    {
        var settings = CreateTypeSettings();
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;
        var emptyMap = new Dictionary<Type, Func<TypeBase>>();

        Action act = () => new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, typeFactory, typeProvider, emptyMap);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Valid_Arguments()
    {
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        var generator = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProvider, map);

        generator.Should().NotBeNull();
    }

    #endregion Constructor Tests


    [TestMethod]
    public void GenerateTypeDefinition_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([]);

        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProviderMock.Object, map);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().NotBeNullOrEmpty();
        result.TypeFileContents.Values.Single().Should().Contain("[]", because: "the types JSON should be and empty array '[]' when no resource types are generated");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_Resource_When_TypeProvider_Has_Types()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(SimpleResource), new("SimpleResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProviderMock.Object, map);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.IndexFileContent.Should().Contain("SimpleResource");
        result.TypeFileContents.Values.Single().Should().Contain("SimpleResource");
        result.TypeFileContents.Values.Single().Should().Contain("name", because: "the property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Throws_On_Unsupported_Property_Type()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(TestUnsupportedProperty), new("TestUnsupportedProperty"))]);

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProviderMock.Object, map);

        Action act = () => builder.GenerateTypeDefinition();
        act.Should().Throw<NotImplementedException>();
    }

    private record ArrayResource(string[] Items);

    private record EnumerableResource(IEnumerable<string> Items);

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_ArrayProperty()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(ArrayResource), new("ArrayResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProviderMock.Object, map);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.TypeFileContents.Values.Single().Should().Contain("ArrayResource");
        result.TypeFileContents.Values.Single().Should().Contain("items", because: "the array property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateTypeDefinition_Emits_ArrayType_For_IEnumerableProperty()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes(true)).Returns([(typeof(EnumerableResource), new("EnumerableResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder("TestSettings", "2025-01-01", true, null, factory, typeProviderMock.Object, map);

        var result = builder.GenerateTypeDefinition();

        result.Should().NotBeNull();
        result.TypeFileContents.Values.Single().Should().Contain("EnumerableResource");
        result.TypeFileContents.Values.Single().Should().Contain("items", because: "the enumerable property should be present in the resource type definition");
    }
}
