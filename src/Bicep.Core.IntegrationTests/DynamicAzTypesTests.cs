// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DynamicAzTypesTests : TestBase
    {
        private async Task<ServiceBuilder> GetServices()
        {
            var indexJson = FileHelper.SaveResultFile(TestContext, "types/index.json", """{"Resources": {}, "Functions": {}}""");

            var cacheRoot = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(cacheRoot);

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true, CacheRootDirectory: cacheRoot))
                .WithContainerRegistryClientFactory(DataSetsExtensions.CreateOciClientForAzProvider());

            await DataSetsExtensions.PublishAzProvider(services.Build(), indexJson);

            return services;
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
            result.Compilation.GetEntrypointSemanticModel().Root.ProviderDeclarations.Should().Contain(x => x.Name.Equals("testAlias"));
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_with_bicepconfig_provider_alias()
        {
            var services = await GetServices();

            services = services.WithConfigurationPatch(c => c.WithProviderAlias($$"""
            {
                "br": {
                    "customAlias": {
                        "registry": "{{LanguageConstants.BicepPublicMcrRegistry}}",
                        "providerPath": "bicep/providers"
                    }
                }
            }
            """));

            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/customAlias:az@{BicepTestConstants.BuiltinAzProviderVersion}'
            ");

            result.Should().GenerateATemplate();
            result.Compilation.GetEntrypointSemanticModel().Root.ProviderDeclarations.Should().Contain(x => x.Name.Equals("az"));
        }

        [TestMethod]
        public async Task Az_namespace_specified_using_legacy_declaration_syntax_yields_diagnostic()
        {
            var services = new ServiceBuilder()
               .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));

            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'az@0.2.661'
            ");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(
                new[] {
                ("BCP304", DiagnosticLevel.Error, "Invalid provider specifier string. Specify a valid provider of format \"<providerName>@<providerVersion>\"."),
            });

        }

        [TestMethod]
        public async Task Bicep_module_artifact_specified_in_provider_declaration_syntax_yields_diagnostic()
        {
            var testArtifact = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "bicep/providers/az", "0.2.661");
            var clientFactory = DataSetsExtensions.CreateMockRegistryClients(
                (new Uri($"https://{testArtifact.RegistryAddress}"), testArtifact.RepositoryPath)).factoryMock;
            await DataSetsExtensions.PublishModuleToRegistryAsync(
                clientFactory,
                moduleName: "az",
                target: testArtifact.ToSpecificationString(':'),
                moduleSource: "",
                publishSource: false,
                documentationUri: "mydocs.org/abc");

            var services = new ServiceBuilder()
           .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true))
           .WithContainerRegistryClientFactory(clientFactory);

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{testArtifact.ToSpecificationString('@')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(
                new[] {
                ("BCP190", DiagnosticLevel.Error, @$"The artifact with reference ""{testArtifact.ToSpecificationString(':')}"" has not been restored."),
                ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
            });
        }

        public record ArtifactRegistryAddress(string RegistryAddress, string RepositoryPath, string ProviderVersion)
        {
            public string ToSpecificationString(char delim) => $"br:{RegistryAddress}/{RepositoryPath}{delim}{ProviderVersion}";
        }

        [TestMethod]
        [DynamicData(nameof(ArtifactRegistryAddressNegativeTestScenarios), DynamicDataSourceType.Method)]
        public async Task Repository_not_found_in_registry(
            ArtifactRegistryAddress artifactRegistryAddress,
            Exception exceptionToThrow,
            IEnumerable<(string, DiagnosticLevel, string)> expectedDiagnostics)
        {
            // ARRANGE
            // mock the blob client to throw the expected exception
            var mockBlobClient = StrictMock.Of<MockRegistryBlobClient>();
            mockBlobClient.Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exceptionToThrow);

            // mock the registry client to return the mock blob client
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();
            containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(
                new Uri($"https://{artifactRegistryAddress.RegistryAddress}"), artifactRegistryAddress.RepositoryPath, mockBlobClient.Object);

            var (clientFactory, _) = containerRegistryFactoryBuilder.Build();

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory);

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{artifactRegistryAddress.ToSpecificationString('@')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(expectedDiagnostics);
        }

        public static IEnumerable<object[]> ArtifactRegistryAddressNegativeTestScenarios()
        {
            // constants
            const string placeholderProviderVersion = "0.0.0-placeholder";

            // unresolvable host registry. For example if DNS is down or unresponsive
            const string unreachableRegistryAddress = "unknown.registry.azurecr.io";
            const string NoSuchHostMessage = $" (No such host is known. ({unreachableRegistryAddress}:443))";
            var AggregateExceptionMessage = $"Retry failed after 4 tries. Retry settings can be adjusted in ClientOptions.Retry or by configuring a custom retry policy in ClientOptions.RetryPolicy.{string.Concat(Enumerable.Repeat(NoSuchHostMessage, 4))}";
            var unreacheable = new ArtifactRegistryAddress(unreachableRegistryAddress, "bicep/providers/az", placeholderProviderVersion);
            yield return new object[] {
                unreacheable,
                new AggregateException(AggregateExceptionMessage),
                new (string, DiagnosticLevel, string)[]{
                    new ("BCP192", DiagnosticLevel.Error, @$"Unable to restore the artifact with reference ""{unreacheable.ToSpecificationString(':')}"": {AggregateExceptionMessage}"),
                    ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
                },
            };

            // manifest not found is thrown when the repository address is not registered and/or the version doesn't exist in the registry
            const string NotFoundMessage = "The artifact does not exist in the registry.";
            var withoutRepo = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "unknown/path/az", placeholderProviderVersion);
            yield return new object[] {
                withoutRepo,
                new RequestFailedException(404, NotFoundMessage),
                new (string, DiagnosticLevel, string)[]{
                    new ("BCP192", DiagnosticLevel.Error, $@"Unable to restore the artifact with reference ""{withoutRepo.ToSpecificationString(':')}"": {NotFoundMessage}"),
                    ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
                },
            };
        }
    }
}
