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
    public void Constructor_Throws_On_Null_TypeSettings()
    {
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        Action act = () => new TypeDefinitionBuilder(null!, typeFactory, typeProvider, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_TypeFactory()
    {
        var settings = CreateTypeSettings();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        Action act = () => new TypeDefinitionBuilder(settings, null!, typeProvider, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_TypeProvider()
    {
        var typeFactory = CreateTypeFactory();
        var settings = CreateTypeSettings();
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        Action act = () => new TypeDefinitionBuilder(settings, typeFactory, null!, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_TypeToTypeBaseMap()
    {
        var settings = CreateTypeSettings();
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;

        Action act = () => new TypeDefinitionBuilder(settings, typeFactory, typeProvider, null!);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_Throws_On_Empty_TypeToTypeBaseMap()
    {
        var settings = CreateTypeSettings();
        var typeFactory = CreateTypeFactory();
        var typeProvider = StrictMock.Of<ITypeProvider>().Object;
        var emptyMap = new Dictionary<Type, Func<TypeBase>>();

        Action act = () => new TypeDefinitionBuilder(settings, typeFactory, typeProvider, emptyMap);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Valid_Arguments()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        var generator = new TypeDefinitionBuilder(typeSettings, factory, typeProvider, map);

        generator.Should().NotBeNull();
    }

    #endregion Constructor Tests


    [TestMethod]
    public void GenerateBicepResourceTypes_Returns_Empty_When_TypeProvider_Has_No_Types()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes()).Returns([]);

        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder(settings, factory, typeProviderMock.Object, map);

        var result = builder.GenerateBicepResourceTypes();
        

        result.Should().NotBeNull();        
        result.IndexJson.Should().NotBeNullOrEmpty();
        result.TypesJsons.Values.Single().Should().NotBeNullOrEmpty();
        result.TypesJsons.Values.Single().Should().Contain("[]", because: "the types JSON should be and empty array '[]' when no resource types are generated");        
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_Resource_When_TypeProvider_Has_Types()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes()).Returns([(typeof(SimpleResource), new("SimpleResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder(settings, factory, typeProviderMock.Object, map);

        var result = builder.GenerateBicepResourceTypes();

        result.Should().NotBeNull();
        result.IndexJson.Should().Contain("SimpleResource");
        result.TypesJsons.Values.Single().Should().Contain("SimpleResource");
        result.TypesJsons.Values.Single().Should().Contain("name", because: "the property should be present in the resource type definition");        
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Throws_On_Unsupported_Property_Type()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes()).Returns([(typeof(TestUnsupportedProperty), new("TestUnsupportedProperty"))]);

        var builder = new TypeDefinitionBuilder(settings, factory, typeProviderMock.Object, map);

        Action act = () => builder.GenerateBicepResourceTypes();
        act.Should().Throw<NotImplementedException>();
    }

    private record ArrayResource(string[] Items);

    private record EnumerableResource(IEnumerable<string> Items);

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_ArrayType_For_ArrayProperty()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes()).Returns([(typeof(ArrayResource), new("ArrayResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder(settings, factory, typeProviderMock.Object, map);

        var result = builder.GenerateBicepResourceTypes();

        result.Should().NotBeNull();
        result.TypesJsons.Values.Single().Should().Contain("ArrayResource");
        result.TypesJsons.Values.Single().Should().Contain("items", because: "the array property should be present in the resource type definition");
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_ArrayType_For_IEnumerableProperty()
    {
        var settings = CreateTypeSettings();
        var factory = CreateTypeFactory();
        var typeProviderMock = StrictMock.Of<ITypeProvider>();
        typeProviderMock.Setup(tp => tp.GetResourceTypes()).Returns([(typeof(EnumerableResource), new("EnumerableResource"))]);
        var map = new Dictionary<Type, Func<TypeBase>> { { typeof(string), () => new StringType() } };

        var builder = new TypeDefinitionBuilder(settings, factory, typeProviderMock.Object, map);

        var result = builder.GenerateBicepResourceTypes();

        result.Should().NotBeNull();
        result.TypesJsons.Values.Single().Should().Contain("EnumerableResource");
        result.TypesJsons.Values.Single().Should().Contain("items", because: "the enumerable property should be present in the resource type definition");
    }
}
