// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host.Attributes;
using Bicep.Local.Extension.Host.TypeDefinitionBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests.TypeSpecBuilderTests;

[TestClass]
public class TypeDefinitionGeneratorTests
{
    // Test classes for property type mapping
    private record TestStringProperty { public string Name { get; set; } = ""; }
    private record TestBoolProperty { public bool Flag { get; set; } }
    private record TestIntProperty { public int Count { get; set; } }
    private record TestUnsupportedProperty { public DateTime When { get; set; } }
    private record NestedClassProperty { public TestStringProperty Nested { get; set; } = new(); }
    private record MultipleStringProperties
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
    private Mock<ITypeProvider> typeProviderMock = StrictMock.Of<ITypeProvider>();

    private TypeDefinitionGenerator CreateGenerator(ImmutableDictionary<Type, Func<TypeBase>> map, Type[] resourceTypes)
    {
        typeProviderMock.Setup(p => p.GetResourceTypes()).Returns(resourceTypes);
        return new TypeDefinitionGenerator(typeSettings, typeFactory, typeProviderMock.Object, map);
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeSettings_Is_Null()
    {
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Action act = () => new TypeDefinitionGenerator(null!, factory, typeProvider, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeFactory_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Action act = () => new TypeDefinitionGenerator(typeSettings, null!, typeProvider, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeProvider_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        Action act = () => new TypeDefinitionGenerator(typeSettings, factory, null!, map);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeToTypeBaseMap_Is_Null()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;

        Action act = () => new TypeDefinitionGenerator(typeSettings, factory, typeProvider, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Throws_ArgumentNullException_When_TypeToTypeBaseMap_Is_Empty()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var emptyMap = ImmutableDictionary<Type, Func<TypeBase>>.Empty;

        Action act = () => new TypeDefinitionGenerator(typeSettings, factory, typeProvider, emptyMap);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Valid_Arguments()
    {
        var typeSettings = new TypeSettings("name", "version", true, new Azure.Bicep.Types.CrossFileTypeReference("index.json", 0));
        var factory = new TypeFactory([]);
        var typeProvider = new Mock<ITypeProvider>(MockBehavior.Strict).Object;
        var map = ImmutableDictionary<Type, Func<TypeBase>>.Empty.Add(typeof(string), () => new StringType());

        var generator = new TypeDefinitionGenerator(typeSettings, factory, typeProvider, map);

        generator.Should().NotBeNull();
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

        Action act = () => generator.GenerateBicepResourceTypes();
        act.Should().Throw<NotImplementedException>();
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

        typeSpec.TypesJson.Should().Contain("name");
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

        typeSpec.TypesJson.Should().Contain("flag");
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

        typeSpec.TypesJson.Should().Contain("count");
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

        typeSpec.TypesJson.Should().Contain("nested");
        typeSpec.TypesJson.Should().Contain("name");
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

        typeSpec.TypesJson.Should().Contain("name");
        typeSpec.TypesJson.Should().Contain("description");
    }
}
