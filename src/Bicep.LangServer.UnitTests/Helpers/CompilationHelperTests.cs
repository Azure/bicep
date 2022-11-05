// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using IOFileSystem = System.IO.Abstractions.FileSystem;
using CompilationHelper = Bicep.LanguageServer.Utils.CompilationHelper;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Helpers
{
    [TestClass]
    public class CompilationHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly FileResolver FileResolver = BicepTestConstants.FileResolver;
        private static readonly IConfigurationManager configurationManager = new ConfigurationManager(new IOFileSystem());
        private readonly ModuleDispatcher ModuleDispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider, configurationManager);

        [TestMethod]
        public async Task GetCompilationAsync_WithNullCompilationContext_ShouldCreateCompilation()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            // Do not upsert compilation. This will cause CompilationContext to be null
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: false);
            var compilationContext = bicepCompilationManager.GetCompilation(documentUri);

            compilationContext.Should().BeNull();

            var compilation = await CompilationHelper.GetCompilationAsync(
                documentUri,
                documentUri.ToUri(),
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer,
                bicepCompilationManager,
                configurationManager,
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider);

            compilation.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetCompilationAsync_WithNonNullCompilationContext_ShouldReuseCompilation()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            // Upsert compilation. This will cause CompilationContext to be non null
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: true);

            var compilation = await CompilationHelper.GetCompilationAsync(
                documentUri,
                documentUri.ToUri(),
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer,
                bicepCompilationManager,
                configurationManager,
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider);

            compilation.Should().NotBeNull();
            compilation.Should().BeSameAs(bicepCompilationManager.GetCompilation(documentUri)!.Compilation);
        }
    }
}
