// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Features.Custom.Visualization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class ResourceTypeCatalogTests
{
    private const ResourceScope ResourceGroup = ResourceScope.ResourceGroup;
    private const ResourceScope Subscription = ResourceScope.Subscription;

    [TestMethod]
    public void GetResourceTypes_ListsEachTypeOnceAtItsDefaultVersionSortedByName()
    {
        var catalog = CreateCatalog(
            "B.Rp/beta@2020-01-01",
            "A.Rp/alpha@2020-01-01",
            "A.Rp/alpha@2023-01-01-preview",
            "A.Rp/previewOnly@2022-01-01-preview");

        catalog.GetResourceTypes(scope: null).Should().Equal(
            new VisualResourceTypeCatalogEntry("A.Rp/alpha", "2020-01-01", false),
            new VisualResourceTypeCatalogEntry("A.Rp/previewOnly", "2022-01-01-preview", true),
            new VisualResourceTypeCatalogEntry("B.Rp/beta", "2020-01-01", false));
    }

    [TestMethod]
    public void ScopeFiltering_KeepsDeployableTypesAndTypesWithUnknownScopes()
    {
        var scopeResolver = new FakeWritableScopeResolver(new()
        {
            ["A.Rp/resourceGroupOnly@2020-01-01"] = ResourceGroup,
            ["A.Rp/subscriptionOnly@2020-01-01"] = Subscription,
            ["A.Rp/both@2020-01-01"] = ResourceGroup | Subscription,
        });
        var catalog = CreateCatalog(scopeResolver, "A.Rp/resourceGroupOnly@2020-01-01", "A.Rp/subscriptionOnly@2020-01-01", "A.Rp/both@2020-01-01", "A.Rp/unknown@2020-01-01");

        catalog.GetResourceTypes(ResourceGroup).Select(type => type.FullyQualifiedType)
            .Should().Equal("A.Rp/both", "A.Rp/resourceGroupOnly", "A.Rp/unknown");
        catalog.GetResourceTypes(Subscription).Select(type => type.FullyQualifiedType)
            .Should().Equal("A.Rp/both", "A.Rp/subscriptionOnly", "A.Rp/unknown");
    }

    [TestMethod]
    public void ScopeFiltering_ReadsScopesOnlyWhenNeededAndOnlyOnce()
    {
        var scopeResolver = new FakeWritableScopeResolver([]);
        var catalog = CreateCatalog(scopeResolver, "A.Rp/alpha@2020-01-01", "A.Rp/alpha@2023-01-01-preview");

        catalog.GetResourceTypes(scope: null);
        scopeResolver.ResolveCount.Should().Be(0);

        catalog.GetResourceTypes(ResourceGroup);
        catalog.GetResourceTypes(Subscription);
        scopeResolver.ResolveCount.Should().Be(1);
    }

    [TestMethod]
    public void NamespacesAndSearch_AreCaseInsensitive()
    {
        var catalog = CreateCatalog("B.Rp/widgets@2020-01-01", "A.Rp/widgets@2020-01-01", "A.Rp/gadgets@2020-01-01");

        catalog.GetNamespaces(scope: null).Should().Equal(
            new VisualResourceTypeNamespace("A.Rp", 2),
            new VisualResourceTypeNamespace("B.Rp", 1));
        catalog.GetResourceTypes("a.rp", scope: null).Select(type => type.FullyQualifiedType)
            .Should().Equal("A.Rp/gadgets", "A.Rp/widgets");
        catalog.Search(" WIDGETS ", scope: null).Select(type => type.FullyQualifiedType)
            .Should().Equal("A.Rp/widgets", "B.Rp/widgets");
    }

    [TestMethod]
    public void GetApiVersions_ListsEveryVersionNewestFirstOrReportsUnknownTypes()
    {
        var catalog = CreateCatalog("A.Rp/alpha@2020-01-01", "A.Rp/alpha@2021-01-01-preview");

        catalog.GetApiVersions("a.rp/ALPHA").Should().Equal("2021-01-01-preview", "2020-01-01");
        catalog.Invoking(catalog => catalog.GetApiVersions("A.Rp/missing"))
            .Should().Throw<VisualResourceCreationException>().WithMessage("*was not found.");
    }

    [TestMethod]
    public void GetId_IsStablePerScopeAndDiffersAcrossScopes()
    {
        var catalog = CreateCatalog("A.Rp/alpha@2020-01-01");

        catalog.GetId(ResourceGroup).Should().Be(catalog.GetId(ResourceGroup));
        catalog.GetId(ResourceGroup).Should().NotBe(catalog.GetId(Subscription));
        catalog.GetId(ResourceGroup).Should().NotBe(CreateCatalog("A.Rp/alpha@2020-01-01").GetId(ResourceGroup));
    }

    private static ResourceTypeCatalog CreateCatalog(params string[] references) => CreateCatalog(new FakeWritableScopeResolver([]), references);

    private static ResourceTypeCatalog CreateCatalog(IResourceWritableScopeResolver scopeResolver, params string[] references) =>
        new(
            references
                .Select(ResourceTypeReference.Parse)
                .GroupBy(reference => reference.Type, StringComparer.OrdinalIgnoreCase)
                .ToImmutableDictionary(group => group.Key, group => group.ToImmutableArray(), StringComparer.OrdinalIgnoreCase),
            scopeResolver);

    private sealed class FakeWritableScopeResolver : IResourceWritableScopeResolver
    {
        private readonly Dictionary<string, ResourceScope> scopes;

        public FakeWritableScopeResolver(Dictionary<string, ResourceScope> scopes)
        {
            this.scopes = scopes;
        }

        public int ResolveCount { get; private set; }

        public ImmutableDictionary<ResourceTypeReference, ResourceScope> Resolve(IEnumerable<ResourceTypeReference> references)
        {
            this.ResolveCount++;
            return references
                .Distinct()
                .Where(reference => this.scopes.ContainsKey(reference.FormatName()))
                .ToImmutableDictionary(reference => reference, reference => this.scopes[reference.FormatName()]);
        }
    }
}
