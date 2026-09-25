// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Features.Custom.Visualization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class ResourceWritableScopeResolverTests
{
    private static readonly ResourceTypeReference StorageAccounts = ResourceTypeReference.Parse("Microsoft.Storage/storageAccounts@2023-01-01");

    // A separate instance of the built-in type implementation, so it goes through the generic per-type path.
    private static ImportedExtensionResourceWritableScopeResolver ImportedExtensionResolver =>
        new(BicepTestConstants.AzResourceTypeProvider, TestTypeHelper.GetBuiltInNamespaceType("az"));

    [TestMethod]
    public void BuiltInResolver_MatchesPerTypeResolution()
    {
        var sample = AzResourceTypeProvider.Instance.GetAvailableTypes()
            .OrderBy(reference => reference.FormatName(), StringComparer.OrdinalIgnoreCase)
            .Where((_, index) => index % 500 == 0)
            .ToArray();

        var scopes = BuiltInAzureResourceWritableScopeResolver.Instance.Value.Resolve(sample);

        scopes.Keys.Should().BeEquivalentTo(sample);
        scopes.Should().Equal(ImportedExtensionResolver.Resolve(sample));
    }

    [DataTestMethod]
    [DataRow("Microsoft.Storage/storageAccounts@2023-01-01", ResourceScope.ResourceGroup)]
    [DataRow("Microsoft.Resources/resourceGroups@2022-09-01", ResourceScope.Subscription)]
    [DataRow("Microsoft.Management/managementGroups@2021-04-01", ResourceScope.Tenant)]
    public void BuiltInResolver_ReportsWellKnownDeploymentScopes(string reference, ResourceScope expected)
    {
        var typeReference = ResourceTypeReference.Parse(reference);

        BuiltInAzureResourceWritableScopeResolver.Instance.Value.Resolve([typeReference])
            .Should().ContainKey(typeReference).WhoseValue.Should().Be(expected);
    }

    [TestMethod]
    public void Resolvers_OmitUnknownTypesAndDuplicates()
    {
        var unknown = ResourceTypeReference.Parse("Test.Rp/missing@2020-01-01");

        BuiltInAzureResourceWritableScopeResolver.Instance.Value.Resolve([StorageAccounts, StorageAccounts, unknown]).Keys.Should().BeEquivalentTo([StorageAccounts]);
        ImportedExtensionResolver.Resolve([StorageAccounts, StorageAccounts, unknown]).Keys.Should().BeEquivalentTo([StorageAccounts]);
    }
}
