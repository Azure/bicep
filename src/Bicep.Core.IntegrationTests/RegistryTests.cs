// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class RegistryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task InvalidRootCachePathShouldProduceReasonableErrors()
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));

            var badCacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(badCacheDirectory);

            var badCachePath = Path.Combine(badCacheDirectory, "file.txt");
            File.Create(badCachePath);
            File.Exists(badCachePath).Should().BeTrue();

            // cache root points to a file
            var features = new Mock<IFeatureProvider>(MockBehavior.Strict);
            features.Setup(m => m.RegistryEnabled).Returns(true);
            features.SetupGet(m => m.CacheRootDirectory).Returns(badCachePath);

            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, features.Object));

            var workspace = new Workspace();
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, fileUri);
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            var compilation = new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), sourceFileGrouping);
            var diagnostics = compilation.GetAllDiagnosticsByBicepFile();
            diagnostics.Should().HaveCount(1);

            diagnostics.Single().Value.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith("Unable to restore the module with reference \"oci:mock-registry-one.invalid/demo/plan:v2\": Unable to create the local module directory \"");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith("Unable to restore the module with reference \"oci:mock-registry-two.invalid/demo/site:v3\": Unable to create the local module directory \"");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith("Unable to restore the module with reference \"ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0\": Unable to create the local module directory \"");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith("Unable to restore the module with reference \"ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2\": Unable to create the local module directory \"");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP062");
                    x.Message.Should().Be("The referenced declaration with name \"siteDeploy\" is not valid.");
                });
        }
    }
}
