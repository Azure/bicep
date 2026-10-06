// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualResourceTypeCatalogServiceTests
{
    private static readonly ImmutableArray<ResourceTypeComponents> CatalogFixture =
    [
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2020-01-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01-preview", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/beta", "2020-06-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/gamma", "2019-01-01", TypeSymbolValidationFlags.Default),
    ];

    #region Catalog

    [TestMethod]
    public void GetResourceTypes_ReturnsLatestStableApiVersionForEachTypeWithStableCatalogId()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceTypeCatalogService();

        var result = service.GetResourceTypes(model, knownCatalogId: null);

        result.ResourceTypes!.Select(entry => (entry.FullyQualifiedType, entry.ApiVersion)).Should().Equal(
            ("Test.Rp/alpha", "2020-01-01"),
            ("Test.Rp/beta", "2020-06-01"),
            ("Test.Rp/gamma", "2019-01-01"));
        result.ResourceTypes.Should().OnlyContain(entry => !entry.IsPreview);
        service.GetResourceTypes(model, knownCatalogId: null).CatalogId.Should().Be(result.CatalogId);
    }

    [TestMethod]
    public void GetResourceTypes_WithTheCurrentCatalogId_OmitsTheTypes()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceTypeCatalogService();
        var catalogId = service.GetResourceTypes(model, knownCatalogId: null).CatalogId;

        var result = service.GetResourceTypes(model, catalogId);

        result.CatalogId.Should().Be(catalogId);
        result.ResourceTypes.Should().BeNull();
        service.GetResourceTypes(model, "stale-catalog").ResourceTypes.Should().HaveCount(3);
    }

    [TestMethod]
    public void GetResourceTypes_PrefersStableVersionWithSameDate()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01-preview", TypeSymbolValidationFlags.Default))
            .Add(TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);

        new VisualResourceTypeCatalogService().GetResourceTypes(model, knownCatalogId: null).ResourceTypes!
            .Should().ContainSingle(entry => entry.FullyQualifiedType == "Other.Rp/delta")
            .Which.ApiVersion.Should().Be("2022-01-01");
    }

    [TestMethod]
    public void GetResourceTypes_PreviewOnlyType_ReturnsNewestPreview()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/previewOnly", "2022-01-01-preview", TypeSymbolValidationFlags.Default))
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/previewOnly", "2023-01-01-preview", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);

        new VisualResourceTypeCatalogService().GetResourceTypes(model, knownCatalogId: null).ResourceTypes!
            .Should().ContainSingle(entry => entry.FullyQualifiedType == "Test.Rp/previewOnly")
            .Which.Should().Be(new VisualResourceTypeCatalogEntry("Test.Rp/previewOnly", "2023-01-01-preview", true));
    }

    [TestMethod]
    public void GetResourceTypeVersions_ReturnsAllVersionsNewestFirstWithMatchingCatalogId()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceTypeCatalogService();

        var versions = service.GetResourceTypeVersions(model, "test.rp/ALPHA");

        versions.ApiVersions.Should().Equal("2021-01-01", "2021-01-01-preview", "2020-01-01");
        versions.CatalogId.Should().Be(service.GetResourceTypes(model, knownCatalogId: null).CatalogId);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("Test.Rp/missing")]
    public void GetResourceTypeVersions_UnknownType_ReportsFailure(string resourceType)
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceTypeCatalogService();

        Action act = () => service.GetResourceTypeVersions(model, resourceType);

        act.Should().Throw<VisualResourceCreationException>().WithMessage("*was not found.");
    }
    private static readonly ImmutableArray<ResourceTypeComponents> ScopedFixture =
    [
        CreateScopedType("Scope.Rp/resourceGroupOnly", ResourceScope.ResourceGroup),
        CreateScopedType("Scope.Rp/subscriptionOnly", ResourceScope.Subscription),
        CreateScopedType("Scope.Rp/managementGroupOnly", ResourceScope.ManagementGroup),
        CreateScopedType("Scope.Rp/tenantOnly", ResourceScope.Tenant),
        CreateScopedType("Scope.Rp/extensionOnly", ResourceScope.Resource),
        CreateScopedType("Scope.Rp/everywhere", ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource),
        // Readable at the resource group (usable with `existing`) but only deployable at the subscription.
        CreateScopedType("Scope.Rp/readOnlyAtResourceGroup", ResourceScope.ResourceGroup | ResourceScope.Subscription, readOnlyScopes: ResourceScope.ResourceGroup),
        CreateScopedType("Other.Rp/tenantOnly", ResourceScope.Tenant),
    ];

    [DataTestMethod]
    [DataRow("resourceGroup", new[] { "Scope.Rp/everywhere", "Scope.Rp/resourceGroupOnly" })]
    [DataRow("subscription", new[] { "Scope.Rp/everywhere", "Scope.Rp/readOnlyAtResourceGroup", "Scope.Rp/subscriptionOnly" })]
    [DataRow("managementGroup", new[] { "Scope.Rp/everywhere", "Scope.Rp/managementGroupOnly" })]
    [DataRow("tenant", new[] { "Other.Rp/tenantOnly", "Scope.Rp/everywhere", "Scope.Rp/tenantOnly" })]
    public void ResourceCatalog_OffersOnlyTypesDeployableAtTheDocumentScope(string targetScope, string[] expectedTypes)
    {
        var model = CreateModel(ScopedFixture, $"targetScope = '{targetScope}'");

        new VisualResourceTypeCatalogService().GetResourceTypes(model, knownCatalogId: null).ResourceTypes!
            .Select(entry => entry.FullyQualifiedType).Should().Equal(expectedTypes);
    }

    [TestMethod]
    public void ResourceCatalog_ExcludesExtensionOnlyTypesThatRequireAScopeProperty()
    {
        var model = CreateModel(ScopedFixture, string.Empty);

        new VisualResourceTypeCatalogService().GetResourceTypes(model, knownCatalogId: null).ResourceTypes!
            .Should().NotContain(entry => entry.FullyQualifiedType == "Scope.Rp/extensionOnly");
    }

    [TestMethod]
    public void CatalogId_IsStableWithinAScopeAndChangesWithTheScope()
    {
        var service = new VisualResourceTypeCatalogService();
        var resourceGroupModel = CreateModel(ScopedFixture, "targetScope = 'resourceGroup'");
        var subscriptionModel = CreateModel(ScopedFixture, "targetScope = 'subscription'");

        var resourceGroupId = service.GetResourceTypes(resourceGroupModel, knownCatalogId: null).CatalogId;

        service.GetResourceTypes(resourceGroupModel, knownCatalogId: null).CatalogId.Should().Be(resourceGroupId);
        service.GetResourceTypeVersions(resourceGroupModel, "Scope.Rp/everywhere").CatalogId.Should().Be(resourceGroupId);
        service.GetResourceTypes(subscriptionModel, knownCatalogId: null).CatalogId.Should().NotBe(resourceGroupId);
    }
    [TestMethod]
    public void ResourceCatalog_VersionsAreNotFilteredByScope()
    {
        var fixture = ImmutableArray.Create(
            CreateScopedType("Scope.Rp/widgets", ResourceScope.ResourceGroup, "2024-01-01"),
            CreateScopedType("Scope.Rp/widgets", ResourceScope.Subscription, "2020-01-01"));
        var model = CreateModel(fixture, string.Empty);

        new VisualResourceTypeCatalogService().GetResourceTypeVersions(model, "Scope.Rp/widgets").ApiVersions
            .Should().Equal("2024-01-01", "2020-01-01");
    }

    #endregion

    private static ResourceTypeComponents CreateScopedType(
        string fullyQualifiedType,
        ResourceScope scopes,
        string apiVersion = "2024-01-01",
        ResourceScope readOnlyScopes = ResourceScope.None) =>
        TestTypeHelper.CreateCustomResourceType(
            fullyQualifiedType, apiVersion, TypeSymbolValidationFlags.Default, scopes, readOnlyScopes, ResourceFlags.None);

    private static SemanticModel CreateModel(IEnumerable<ResourceTypeComponents> resourceTypes, string content) =>
        CompilationHelper.Compile(new ServiceBuilder().WithAzResources(resourceTypes), content).Compilation.GetEntrypointSemanticModel();
}
