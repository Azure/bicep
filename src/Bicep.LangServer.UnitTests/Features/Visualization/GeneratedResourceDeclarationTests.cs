// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Features.Custom.Visualization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class GeneratedResourceDeclarationTests
{
    [DataTestMethod]
    [DataRow("Microsoft.Storage/storageAccounts", "storageAccount")]
    [DataRow("Microsoft.Compute/virtualMachines", "virtualMachine")]
    [DataRow("Microsoft.Network/loadBalancers", "loadBalancer")]
    [DataRow("Microsoft.Network/ipAddress", "ipAddress")]
    [DataRow("Test.Rp/basicTests", "basicTest")]
    [DataRow("Test.Rp/9-invalid", "invalid")]
    [DataRow("Test.Rp/123456", "resource")]
    [DataRow("Test.Rp/categories", "category")]
    public void SymbolicName_IsDerivedFromTheTypeName(string fullyQualifiedType, string expected)
    {
        Create(fullyQualifiedType).SymbolicName.Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow(new[] { "other" }, "storageAccount")]
    [DataRow(new[] { "storageAccount", "storageAccount1" }, "storageAccount2")]
    [DataRow(new[] { "StorageAccount" }, "storageAccount1")]
    public void SymbolicName_AvoidsExistingNamesCaseInsensitively(string[] existingNames, string expected)
    {
        Create("Microsoft.Storage/storageAccounts", existingNames: existingNames).SymbolicName.Should().Be(expected);
    }

    [TestMethod]
    public void Declaration_UsesTheSymbolicNameAndFullTypeReference()
    {
        var resource = Create("Test.Rp/widgets", existingNames: ["widget"]);

        resource.Declaration.Name.IdentifierName.Should().Be("widget1");
        resource.Declaration.Type.Should().BeOfType<StringSyntax>().Which.TryGetLiteralValue().Should().Be("Test.Rp/widgets@2020-01-01");
    }

    [TestMethod]
    public void Body_IncludesRequiredSingletonLiteralPropertiesAsLiterals()
    {
        var resource = Create(body: TestTypeHelper.CreateObjectType(
            "Body",
            ("location", TypeFactory.CreateStringLiteralType("global"), TypePropertyFlags.Required),
            ("count", TypeFactory.CreateIntegerLiteralType(3), TypePropertyFlags.Required),
            ("enabled", TypeFactory.CreateBooleanLiteralType(true), TypePropertyFlags.Required)));

        resource.UnresolvedRequiredProperties.Should().BeEmpty();
        var body = resource.Declaration.GetBody();
        ((StringSyntax)body.TryGetPropertyByName("location")!.Value).TryGetLiteralValue().Should().Be("global");
        body.TryGetPropertyByName("count")!.Value.ToString().Should().Be("3");
        body.TryGetPropertyByName("enabled")!.Value.ToString().Should().Be("true");
    }

    [TestMethod]
    public void Body_GeneratesNameAndLeavesRequiredLocationForUserInputWhenIncompatible()
    {
        var resource = Create(body: TestTypeHelper.CreateObjectType(
            "Body",
            ("location", LanguageConstants.String, TypePropertyFlags.Required),
            ("name", LanguageConstants.String, TypePropertyFlags.Required)),
            content: "param location int");

        resource.Declaration.GetBody().Properties.Select(property => property.TryGetKeyText()).Should().Equal("name", "location");
        ((StringSyntax)resource.Declaration.GetBody().TryGetPropertyByName("name")!.Value)
            .TryGetLiteralValue().Should().Be("widget");
        resource.Declaration.GetBody().TryGetPropertyByName("location")!.Value
            .Should().BeOfType<VariableAccessSyntax>().Which.Name.IdentifierName
            .Should().Be("__bicep_visual_resource_creation_required_property__");
        resource.UnresolvedRequiredProperties.Should().Equal("location");
    }

    [TestMethod]
    public void Body_UsesCompatibleLocationParameter()
    {
        var resource = Create(body: TestTypeHelper.CreateObjectType(
            "Body", ("location", LanguageConstants.String, TypePropertyFlags.Required)),
            content: "param location string");

        resource.UnresolvedRequiredProperties.Should().BeEmpty();
        resource.Declaration.GetBody().TryGetPropertyByName("location")!.Value.ToString().Should().Be("location");
    }

    [TestMethod]
    public void Body_UsesResourceGroupLocationOnlyAtResourceGroupScope()
    {
        var body = TestTypeHelper.CreateObjectType(
            "Body", ("location", LanguageConstants.String, TypePropertyFlags.Required));

        var resourceGroup = Create(body: body);
        resourceGroup.UnresolvedRequiredProperties.Should().BeEmpty();
        resourceGroup.Declaration.GetBody().TryGetPropertyByName("location")!.Value.ToString()
            .Should().Be("resourceGroup().location");

        var subscription = Create(body: body, content: "targetScope = 'subscription'");
        subscription.UnresolvedRequiredProperties.Should().Equal("location");
    }

    [TestMethod]
    public void Body_RecursivelyIncludesRequiredNestedProperties()
    {
        var settings = TestTypeHelper.CreateObjectType(
            "Settings",
            ("name", LanguageConstants.String, TypePropertyFlags.Required),
            ("kind", TypeFactory.CreateStringLiteralType("singleton"), TypePropertyFlags.Required),
            ("optional", LanguageConstants.String, TypePropertyFlags.None));
        var resource = Create(body: TestTypeHelper.CreateObjectType(
            "Body", ("settings", settings, TypePropertyFlags.Required)));

        resource.UnresolvedRequiredProperties.Should().Equal("settings");
        var nested = resource.Declaration.GetBody().TryGetPropertyByName("settings")!.Value
            .Should().BeOfType<ObjectSyntax>().Subject;
        nested.Properties.Select(property => property.TryGetKeyText()).Should().Equal("name", "kind");
        nested.TryGetPropertyByName("name")!.Value.Should().BeOfType<VariableAccessSyntax>();
        ((StringSyntax)nested.TryGetPropertyByName("kind")!.Value).TryGetLiteralValue().Should().Be("singleton");
    }

    [TestMethod]
    public void Body_IgnoresOptionalAndNullableProperties()
    {
        var resource = Create(body: TestTypeHelper.CreateObjectType(
            "Body",
            ("description", LanguageConstants.String, TypePropertyFlags.None),
            ("name", TypeHelper.MakeNullable(LanguageConstants.String), TypePropertyFlags.Required)));

        resource.Declaration.GetBody().Properties.Should().BeEmpty();
        resource.UnresolvedRequiredProperties.Should().BeEmpty();
    }

    [TestMethod]
    public void Body_OfDiscriminatedType_ReportsOnlyTheDiscriminatorAsUnresolved()
    {
        var memberA = TestTypeHelper.CreateObjectType(
            "MemberA",
            ("kind", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
            ("settingA", LanguageConstants.String, TypePropertyFlags.Required));
        var memberB = TestTypeHelper.CreateObjectType(
            "MemberB",
            ("kind", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
            ("settingB", LanguageConstants.String, TypePropertyFlags.Required));

        var resource = Create(body: TestTypeHelper.CreateDiscriminatedObjectType("Body", "kind", memberA, memberB));

        resource.Declaration.GetBody().Properties.Should().ContainSingle()
            .Which.TryGetKeyText().Should().Be("kind");
        resource.UnresolvedRequiredProperties.Should().Equal("kind");
    }

    private static GeneratedResourceDeclaration Create(
        string fullyQualifiedType = "Test.Rp/widgets",
        ITypeReference? body = null,
        string[]? existingNames = null,
        string content = "")
    {
        var typeReference = new ResourceTypeReference(fullyQualifiedType, "2020-01-01");
        var resourceType = new ResourceType(
            TestTypeHelper.GetBuiltInNamespaceType("az"),
            typeReference,
            ResourceScope.ResourceGroup,
            ResourceScope.None,
            ResourceFlags.None,
            body ?? TestTypeHelper.CreateObjectType("Body", ("description", LanguageConstants.String, TypePropertyFlags.None)),
            []);

        var model = CompilationHelper.Compile(new ServiceBuilder().WithAzResources(BuiltInTestTypes.Types), content)
            .Compilation.GetEntrypointSemanticModel();
        return GeneratedResourceDeclaration.Create(typeReference, resourceType, existingNames ?? [], model);
    }
}
