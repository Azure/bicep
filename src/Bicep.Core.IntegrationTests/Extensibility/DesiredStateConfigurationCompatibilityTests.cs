// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.IntegrationTests.Extensibility;

[TestClass]
public class DesiredStateConfigurationCompatibilityTests
{
    private static readonly string ExtensionReference = "example.azurecr.io/test/dsc:0.1.0";

    private static async Task<ServiceBuilder> GetServicesWithPrepublishedTypes()
    {
        var tgzData = ExtensionResourceTypeHelper.GetMockDesiredStateConfigurationTypesTgz();
        var features = new FeatureProviderOverrides(LocalDeployEnabled: true);
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
            tgzData, features, null, ExtensionReference);

        return services
            .WithConfigurationPatch(c => c
            .WithExtensions($$"""
            {
              "dsc": "br:{{ExtensionReference}}"
            }
            """)
            .WithImplicitExtensions("[]")
            .WithExperimentalFeaturesEnabled(
                ExperimentalFeaturesEnabled.AllDisabled with { LocalDeploy = true }));
    }

    [TestMethod]
    public async Task DesiredStateConfiguration_echo_resource_works()
    {
        var services = await GetServicesWithPrepublishedTypes();

        var result = await CompilationHelper.RestoreAndCompile(services, $$"""
extension 'br:{{ExtensionReference}}'
targetScope = 'local'

resource myEcho 'Microsoft.DSC.Debug/Echo@1.0.0' = {
    output: 'Hello world!'
    showSecrets: false
}
""");

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        // TODO: Add more extensive tests.
    }

    [TestMethod]
    public async Task DesiredStateConfiguration_unknown_resource_works()
    {
        var services = await GetServicesWithPrepublishedTypes();

        var result = await CompilationHelper.RestoreAndCompile(services, $$"""
extension 'br:{{ExtensionReference}}'
targetScope = 'local'

resource myResource 'Foo/Bar@1.0.0' = {
    field: 'value'
    okay: true
}
""");

        // With the fallback resource type, unknown DSC resources using SemVer are now supported.
        // NOTE: BCP081 warning is expected when using fallback types
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]{
            ("BCP081", DiagnosticLevel.Warning, "Resource type \"Foo/Bar@1.0.0\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
        });
        result.Template.Should().NotBeNull();
    }
}
