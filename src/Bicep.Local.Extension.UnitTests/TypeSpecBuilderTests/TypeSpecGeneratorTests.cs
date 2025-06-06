// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Local.Extension.Host.Attributes;
using Bicep.Local.Extension.Host.TypeSpecBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests.TypeSpecBuilderTests;

[TestClass]
public class TypeSpecGeneratorTests
{
    // Test classes for property type mapping
    private class TestStringProperty { public string Name { get; set; } = ""; }
    private class TestBoolProperty { public bool Flag { get; set; } }
    private class TestIntProperty { public int Count { get; set; } }
    private class TestUnsupportedProperty { public DateTime When { get; set; } }
    private class NestedClassProperty { public TestStringProperty Nested { get; set; } = new(); }
    private class MultipleStringProperties
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }

    private TypeSettings typeSettings = new(
        name: "TestSettings",
        version: "2025-01-01",
        isSingleton: true,
        configurationType: new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
    private TypeFactory typeFactory = new([]);
    private MockRepository mockRepository = null!;
    private Mock<ITypeProvider> typeProviderMock = null!;

    [TestInitialize]
    public void Setup()
    {
        mockRepository = new MockRepository(MockBehavior.Strict);
        typeProviderMock = mockRepository.Create<ITypeProvider>();
    }

    private TypeSpecGenerator CreateGenerator(ImmutableDictionary<Type, Func<TypeBase>> map, Type[] resourceTypes)
    {
        typeProviderMock.Setup(p => p.GetResourceTypes()).Returns(resourceTypes);
        return new TypeSpecGenerator(typeSettings, typeFactory, typeProviderMock.Object, map);
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeSettings_Is_Null()
    {
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Assert.ThrowsException<ArgumentNullException>(() =>
            new TypeSpecGenerator(null!, factory, typeProvider, map));
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeFactory_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Assert.ThrowsException<ArgumentNullException>(() =>
            new TypeSpecGenerator(typeSettings, null!, typeProvider, map));
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeProvider_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Assert.ThrowsException<ArgumentNullException>(() =>
            new TypeSpecGenerator(typeSettings, factory, null!, map));
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeToTypeBaseMap_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;

        Assert.ThrowsException<ArgumentNullException>(() =>
            new TypeSpecGenerator(typeSettings, factory, typeProvider, null!));
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeToTypeBaseMap_Is_Empty()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var emptyMap = ImmutableDictionary<Type, Func<TypeBase>>.Empty;

        Assert.ThrowsException<ArgumentNullException>(() =>
            new TypeSpecGenerator(typeSettings, factory, typeProvider, emptyMap));
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Valid_Arguments()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        var generator = new TypeSpecGenerator(typeSettings, factory, typeProvider, map);

        Assert.IsNotNull(generator);
    }



    [TestMethod]
    public void GenerateBicepResourceTypes_Throws_On_Unsupported_Property_Type()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(TestUnsupportedProperty) };
        var generator = CreateGenerator(map, resourceTypes);

        Assert.ThrowsException<NotImplementedException>(() =>
            generator.GenerateBicepResourceTypes());
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_StringType_Property()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(TestStringProperty) };
        var generator = CreateGenerator(map, resourceTypes);

        var typeSpec = generator.GenerateBicepResourceTypes();

        Assert.IsTrue(typeSpec.TypesJson.Contains("Name", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_BooleanType_Property()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(bool), () => new BooleanType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(TestBoolProperty) };
        var generator = CreateGenerator(map, resourceTypes);

        var typeSpec = generator.GenerateBicepResourceTypes();

        Assert.IsTrue(typeSpec.TypesJson.Contains("Flag", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_IntegerType_Property()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(int), () => new IntegerType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(TestIntProperty) };
        var generator = CreateGenerator(map, resourceTypes);

        var typeSpec = generator.GenerateBicepResourceTypes();

        Assert.IsTrue(typeSpec.TypesJson.Contains("Count", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_Nested_ObjectType_Property()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(NestedClassProperty) };
        var generator = CreateGenerator(map, resourceTypes);

        var typeSpec = generator.GenerateBicepResourceTypes();

        Assert.IsTrue(typeSpec.TypesJson.Contains("Nested", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(typeSpec.TypesJson.Contains("Name", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void GenerateBicepResourceTypes_Emits_Multiple_Properties_Of_Same_Type()
    {
        var map = new Dictionary<Type, Func<TypeBase>>
        {
            { typeof(string), () => new StringType() }
        }.ToImmutableDictionary();

        var resourceTypes = new[] { typeof(MultipleStringProperties) };
        var generator = CreateGenerator(map, resourceTypes);

        var typeSpec = generator.GenerateBicepResourceTypes();

        Assert.IsTrue(typeSpec.TypesJson.Contains("Name", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(typeSpec.TypesJson.Contains("Description", StringComparison.OrdinalIgnoreCase));
    }


}
