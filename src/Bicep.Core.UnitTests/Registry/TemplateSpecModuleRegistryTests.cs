// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class TemplateSpecModuleRegistryTests
{
    [TestMethod]
    public async Task RestoreArtifacts_WithUntrustedCloud_DoesNotCreateRepository()
    {
        var configuration = BicepTestConstants.CreateMockConfiguration(new()
        {
            ["cloud.currentProfile"] = "Custom",
            ["cloud.profiles.Custom.resourceManagerEndpoint"] = "https://management.example.invalid",
            ["cloud.profiles.Custom.activeDirectoryAuthority"] = "https://login.example.invalid/adfs",
        });
        var referencingFile = BicepTestConstants.CreateDummyBicepFile(configuration);
        TemplateSpecModuleReference.TryParse(
            referencingFile.LoadFeatures(),
            referencingFile.LoadConfiguration(),
            null,
            "00000000-0000-0000-0000-000000000000/test-rg/test-spec:v1")
            .IsSuccess(out var reference, out _).Should().BeTrue();
        var repositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();
        var registry = new TemplateSpecModuleRegistry(repositoryFactory.Object, new CloudConfigurationTrustPolicy());

        var failures = await registry.RestoreArtifacts([reference!]);

        failures.Should().ContainSingle();
        failures[reference!].Should().HaveCode("BCP457");
        repositoryFactory.VerifyNoOtherCalls();
    }
}
