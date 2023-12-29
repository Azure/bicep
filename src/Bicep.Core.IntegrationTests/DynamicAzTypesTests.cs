// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Bicep.Types.Az;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DynamicAzTypesTests : TestBase
    {
        private async Task<ServiceBuilder> GetServices()
        {
            var indexJson = FileHelper.SaveResultFile(TestContext, "types/index.json", """{"Resources": {}, "Functions": {}}""");

            var clientFactory = DataSetsExtensions.CreateMockRegistryClients(
                false,
                (new Uri($"https://{LanguageConstants.BicepPublicMcrRegistry}"), $"bicep/providers/az")
            ).factoryMock;

            clientFactory = await clientFactory.WithPublishedAzProvider(new System.IO.Abstractions.FileSystem(), indexJson);

            var cacheRoot = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(cacheRoot);

            return new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true, CacheRootDirectory: cacheRoot))
                .WithContainerRegistryClientFactory(clientFactory);
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_without_configuration()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}'
"));

            result.Should().GenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task Az_namespace_errors_with_configuration()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' with {{}}
");

            result.Should().NotGenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Provider namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_with_alias()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as testAlias
            ");

            result.Should().GenerateATemplate();
        }
    }
}
