// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationManagerTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private const int BaseVersion = 42;

        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        private static readonly ConfigurationManager configurationManager = new(new IOFileSystem());

        private static readonly LinterRulesProvider linterRulesProvider = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DataRow(".arm")]
        [DataRow(".json")]
        [DataRow(".jsonc")]
        public void UpsertCompilation_NotInWorspaceArmTemplateFile_ShouldNotUpsert(string fileExtension)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUri();
            var workspace = new Workspace();
            var manager = BicepCompilationManagerHelper.CreateCompilationManager(uri, string.Empty);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, null, "hello");

            // second get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            workspace.TryGetSourceFile(uri, out var file);

            // The workspace should remain empty.
            file.Should().BeNull();

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(".arm")]
        [DataRow(".json")]
        [DataRow(".jsonc")]
        public void UpsertCompilation_InWorspaceArmTemplateFile_ShouldRefreshWorkspace(string fileExtension)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUri();

            var originalFile = SourceFileFactory.CreateArmTemplateFile(uri, "{}");
            var workspace = new Workspace();
            workspace.UpsertSourceFile(originalFile);

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, null, "hello");

            // second get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            workspace.TryGetSourceFile(uri, out var updatedFile);

            // The workspace should be refreshed.
            updatedFile.Should().NotBeNull();
            updatedFile!.FileUri.Should().Be(originalFile.FileUri);
            updatedFile.Should().NotBeSameAs(originalFile);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void UpsertCompilation_BicepFile_ShouldUpsertSuccessfully(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);
            var upserted = manager.GetCompilation(uri);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var actual = manager.GetCompilation(uri);
            actual.Should().NotBeNull();

            // should be the same object
            actual.Should().BeSameAs(upserted);

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void CloseAfterUpsert_ShouldClearDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var actual = manager.GetCompilation(uri);
            actual.Should().NotBeNull();

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            // 2nd get should be the same
            manager.GetCompilation(uri).Should().BeSameAs(actual);

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            // close compilation
            manager.CloseCompilation(uri);

            // close should have cleared diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // expect zero diagnostics and 0 version
            receivedParams.Should().NotBeNull();
            receivedParams.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();

            // reset call counts
            document.Invocations.Clear();

            // get again
            manager.GetCompilation(uri).Should().BeNull();

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void UpsertCompilation_ShouldUpdateDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);
            var firstUpserted = manager.GetCompilation(uri);

            // should have pushed out diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var firstActual = manager.GetCompilation(uri);
            firstActual.Should().NotBeNull();

            // should be same as first upserted
            firstActual.Should().BeSameAs(firstUpserted);

            // upsert second one
            const int newVersion = BaseVersion + 1;
            manager.UpsertCompilation(uri, newVersion, "hello\r\nthere\r\n");
            var secondUpserted = manager.GetCompilation(uri);

            secondUpserted.Should().NotBeNull();
            secondUpserted.Should().NotBeSameAs(firstUpserted);

            // should have pushed out new diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // reset invocations
            document.Invocations.Clear();

            // there should have been 2 diagnostics
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(newVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(2);

            // get latest
            var secondActual = manager.GetCompilation(uri);
            secondActual.Should().BeSameAs(secondUpserted);
        }

        [TestMethod]
        public void GetNonExistentCompilation_ShouldNotThrow()
        {
            var server = Repository.Create<ILanguageServerFacade>();

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), new Workspace(), BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            var uri = DocumentUri.File(this.TestContext.TestName);

            manager.GetCompilation(uri).Should().BeNull();
        }

        [TestMethod]
        public void CloseNonExistentCompilation_ShouldClearDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), new Workspace(), BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            var uri = DocumentUri.File(this.TestContext.TestName);

            manager.CloseCompilation(uri);

            // close should have cleared diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // expect zero diagnostics and 0 version
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();

            // reset call counts
            document.Invocations.Clear();
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void FatalException_ShouldProduceCorrectDiagnosticsAndClearThemWhenFileIsClosed(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";
            provider.Setup(m => m.Create(It.IsAny<IReadOnlyWorkspace>(), It.IsAny<DocumentUri>(), It.IsAny<ImmutableDictionary<ISourceFile, ISemanticModel>>(), It.IsAny<LinterAnalyzer>())).Throws(new InvalidOperationException(expectedMessage));

            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, provider.Object, workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, BaseVersion, "fake", languageId);
            manager.GetCompilation(uri).Should().BeNull();

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().HaveCount(1);

            var fatalDiagnostic = receivedParams.Diagnostics.Single();

            fatalDiagnostic.Message.Should().Be(expectedMessage);
            fatalDiagnostic.Severity.Should().Be(DiagnosticSeverity.Error);

            // reset counts
            document.Invocations.Clear();

            // close the compilation (even if it wasn't opened successfully)
            manager.CloseCompilation(uri);

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // 0 diagnostics expected
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void NormalUpsertAfterFatalException_ShouldReplaceDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";

            const int version = 74;
            var uri = DocumentUri.File(this.TestContext.TestName);

            // start by failing
            bool failUpsert = true;
            provider
                .Setup(m => m.Create(It.IsAny<IReadOnlyWorkspace>(), It.IsAny<DocumentUri>(), It.IsAny<ImmutableDictionary<ISourceFile, ISemanticModel>>(), It.IsAny<LinterAnalyzer>()))
                .Returns<IReadOnlyWorkspace, DocumentUri, ImmutableDictionary<ISourceFile, ISemanticModel>, IBicepAnalyzer>((grouping, documentUri, modelLookup, linterAnalyzer) => failUpsert
                    ? throw new InvalidOperationException(expectedMessage)
                    : BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager).Create(grouping, documentUri, modelLookup, BicepTestConstants.LinterAnalyzer));

            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, provider.Object, workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, version, "fake", languageId);
            manager.GetCompilation(uri).Should().BeNull();

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
            receivedParams.Diagnostics.Should().HaveCount(1);

            var fatalDiagnostic = receivedParams.Diagnostics.Single();

            fatalDiagnostic.Message.Should().Be(expectedMessage);
            fatalDiagnostic.Severity.Should().Be(DiagnosticSeverity.Error);

            // reset counts
            document.Invocations.Clear();

            // allow success
            failUpsert = false;

            // upsert should succeed because we allowed it
            manager.UpsertCompilation(uri, version, "fake\nfake\nfake\n");
            var upserted = manager.GetCompilation(uri);
            upserted.Should().NotBeNull();

            // new diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
            receivedParams.Diagnostics.Should().HaveCount(3);

            // none of the messages should be our fatal message
            receivedParams.Diagnostics
                .Select(diag => diag.Message)
                .All(message => string.Equals(message, expectedMessage) == false)
                .Should().BeTrue();
        }


        [TestMethod]
        public void SemanticModels_should_only_be_reloaded_on_sourcefile_or_dependent_sourcefile_changes()
        {
            var uris = (
                main: new Uri("file:///main.bicep"),
                moduleA: new Uri("file:///moduleA.bicep"),
                moduleB: new Uri("file:///moduleB.bicep"),
                moduleC: new Uri("file:///moduleC.bicep")
            );

            var fileDict = new Dictionary<Uri, string>
            {
                [uris.main] = @"
module moduleA './moduleA.bicep' = {
  name: 'moduleA'
}
module moduleC './moduleC.bicep' = {
  name: 'moduleC'
}
",
                [uris.moduleA] = @"
module moduleB './moduleB.bicep' = {
  name: 'moduleB'
}
",
                [uris.moduleB] = @"
",
                [uris.moduleC] = @"
",
            };

            var diagsReceieved = new List<PublishDiagnosticsParams>();
            var document = BicepCompilationManagerHelper.CreateMockDocument(p => diagsReceieved.Add(p));
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var fileResolver = new InMemoryFileResolver(fileDict);
            var compilationProvider = new BicepCompilationProvider(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new ModuleDispatcher(new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.BuiltInOnlyConfigurationManager), BicepTestConstants.BuiltInOnlyConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.BuiltInOnlyConfigurationManager);

            var compilationManager = new BicepCompilationManager(server.Object, compilationProvider, new Workspace(), fileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);

            diagsReceieved.Should().BeEmpty();

            IReadOnlyDictionary<Uri, Compilation> GetCompilations() => new Dictionary<Uri, Compilation>
            {
                [uris.main] = compilationManager!.GetCompilation(uris.main)!.Compilation,
                [uris.moduleA] = compilationManager.GetCompilation(uris.moduleA)!.Compilation,
                [uris.moduleB] = compilationManager.GetCompilation(uris.moduleB)!.Compilation,
                [uris.moduleC] = compilationManager.GetCompilation(uris.moduleC)!.Compilation,
            };

            void EnsureSemanticModelsAndSourceFilesDeduplicated()
            {
                var allCompilations = GetCompilations();
                var distinctSourceFiles = allCompilations.Values.SelectMany(x => x.SourceFileGrouping.SourceFiles).Distinct(ReferenceEqualityComparer.Instance);
                var distinctSemanticModels = allCompilations.Values.SelectMany(x => x.SourceFileGrouping.SourceFiles.Select(y => x.GetSemanticModel(y))).Distinct(ReferenceEqualityComparer.Instance);

                distinctSourceFiles.Should().HaveCount(4);
                distinctSemanticModels.Should().HaveCount(4);
            }

            // open the main file
            {
                compilationManager.UpsertCompilation(uris.main, 1, fileDict[uris.main], "bicep");

                diagsReceieved.Should().SatisfyRespectively(
                    x => x.Uri.ToUri().Should().Be(uris.main)
                );
                diagsReceieved.Clear();
            }

            // open all files
            {
                compilationManager.UpsertCompilation(uris.moduleA, 1, fileDict[uris.moduleA], "bicep");
                compilationManager.UpsertCompilation(uris.moduleB, 1, fileDict[uris.moduleB], "bicep");
                compilationManager.UpsertCompilation(uris.moduleC, 1, fileDict[uris.moduleC], "bicep");
                diagsReceieved.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting moduleA should only retrigger necessary recompilations
            {
                compilationManager.UpsertCompilation(uris.moduleA, 2, fileDict[uris.moduleA], "bicep");

                diagsReceieved.Should().SatisfyRespectively(
                    x => x.Uri.ToUri().Should().Be(uris.moduleA),
                    x => x.Uri.ToUri().Should().Be(uris.main)
                );
                diagsReceieved.Clear();

                var compilations = GetCompilations();
                var moduleBSource = compilations[uris.moduleB].SourceFileGrouping.EntryPoint;
                var moduleBModel = compilations[uris.moduleB].GetSemanticModel(moduleBSource);

                compilations[uris.main].GetSemanticModel(moduleBSource).Should().BeSameAs(moduleBModel);
                compilations[uris.moduleA].GetSemanticModel(moduleBSource).Should().BeSameAs(moduleBModel);

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting moduleC should only retrigger necessary recompilations
            {
                compilationManager.UpsertCompilation(uris.moduleC, 2, fileDict[uris.moduleC], "bicep");

                diagsReceieved.Should().SatisfyRespectively(
                    x => x.Uri.ToUri().Should().Be(uris.moduleC),
                    x => x.Uri.ToUri().Should().Be(uris.main)
                );
                diagsReceieved.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting main should only retrigger necessary recompilations
            {
                compilationManager.UpsertCompilation(uris.main, 3, fileDict[uris.main], "bicep");

                diagsReceieved.Should().SatisfyRespectively(
                    x => x.Uri.ToUri().Should().Be(uris.main)
                );
                diagsReceieved.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }
        }

        [TestMethod]
        public void GetLinterStateTelemetryOnBicepFileOpen_ShouldReturnTelemetryEvent()
        {
            var compilationManager = CreateBicepCompilationManager();

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""no-unused-vars"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var configurationManager = new ConfigurationManager(new IOFileSystem());
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var rootConfiguration = GetRootConfiguration(testOutputPath, bicepConfigFileContents, configurationManager);

            var telemetryEvent = compilationManager.GetLinterStateTelemetryOnBicepFileOpen(rootConfiguration);

            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);

            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "enabled", "true" },
                { "simplify-interpolation", "warning" },
                { "no-unused-vars", "info" },
                { "no-hardcoded-env-urls", "warning" },
                { "no-unused-params", "info" },
                { "prefer-interpolation", "warning" },
                { "protect-commandtoexecute-secrets", "warning" },
                { "no-unnecessary-dependson", "warning" },
                { "adminusername-should-not-be-literal", "warning" },
                { "use-stable-vm-image", "warning" },
                { "secure-parameter-default", "warning" },
                { "outputs-should-not-contain-secrets", "warning" },
                { "no-hardcoded-location", "warning" },
                { "explicit-values-for-loc-params", "warning" },
                { "no-loc-expr-outside-params", "warning" },
            };

            telemetryEvent.Properties.Should().Contain(properties);
        }

        [TestMethod]
        public void GetLinterStateTelemetryOnBicepFileOpen_WithOverallLinterStateDisabled_ShouldReturnTelemetryEventWithOneProperty()
        {
            var compilationManager = CreateBicepCompilationManager();

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""no-unused-vars"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var configurationManager = new ConfigurationManager(new IOFileSystem());
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var rootConfiguration = GetRootConfiguration(testOutputPath, bicepConfigFileContents, configurationManager);

            var telemetryEvent = compilationManager.GetLinterStateTelemetryOnBicepFileOpen(rootConfiguration);

            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);

            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "enabled", "false" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetLinterStateTelemetryOnBicepFileOpen_WithNoContents_ShouldUseDefaultSettingsAndReturnTelemetryEvent()
        {
            var compilationManager = CreateBicepCompilationManager();

            var bicepConfigFileContents = @"{}";
            var configurationManager = new ConfigurationManager(new IOFileSystem());
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var rootConfiguration = GetRootConfiguration(testOutputPath, bicepConfigFileContents, configurationManager);

            var telemetryEvent = compilationManager.GetLinterStateTelemetryOnBicepFileOpen(rootConfiguration);

            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen);

            var properties = new Dictionary<string, string>
            {
                { "enabled", "true" },
                { "simplify-interpolation", "warning" },
                { "no-unused-vars", "warning" },
                { "no-hardcoded-env-urls", "warning" },
                { "no-unused-params", "warning" },
                { "prefer-interpolation", "warning" },
                { "protect-commandtoexecute-secrets", "warning" },
                { "no-unnecessary-dependson", "warning" },
                { "adminusername-should-not-be-literal", "warning" },
                { "use-stable-vm-image", "warning" },
                { "secure-parameter-default", "warning" },
                { "outputs-should-not-contain-secrets", "warning" },
                { "no-hardcoded-location", "warning" },
                { "explicit-values-for-loc-params", "warning" },
                { "no-loc-expr-outside-params", "warning" },
            };

            telemetryEvent.Properties.Should().Contain(properties);
        }

        [TestMethod]
        public void GetTelemetryAboutSourceFiles_ShouldReturnTelemetryEvent()
        {
            string bicepFileContents = @"param appInsightsName string = 'testAppInsightsName'

resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightsName
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

param location string = 'testLocation'";
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents, testOutputPath);
            var mainUri = DocumentUri.FromFileSystemPath(bicepFilePath).ToUri();

            var bicepFile = SourceFileFactory.CreateBicepFile(mainUri, bicepFileContents);

            var compilation = Services.BuildCompilation(bicepFileContents);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().ToDiagnostics(bicepFile.LineStarts);

            var compilationManager = CreateBicepCompilationManager();

            var telemetryEvent = compilationManager.GetTelemetryAboutSourceFiles(compilation.GetEntrypointSemanticModel(),
                                                                                 bicepFile.FileUri,
                                                                                 ImmutableHashSet.Create<ISourceFile>(bicepFile),
                                                                                 diagnostics);

            var properties = new Dictionary<string, string>
            {
                { "Modules", "0" },
                { "Parameters", "2" },
                { "Resources", "1" },
                { "Variables", "0" },
                { "FileSizeInBytes", "294" },
                { "LineCount", "12" },
                { "Errors", "0" },
                { "Warnings", "3" },
                { "ModulesInReferencedFiles", "0" },
                { "ParentResourcesInReferencedFiles", "0" },
                { "ParametersInReferencedFiles", "0" },
                { "VariablesInReferencedFiles", "0" }
            };

            telemetryEvent.Should().NotBeNull();
            telemetryEvent!.EventName.Should().Be(TelemetryConstants.EventNames.BicepFileOpen);
            telemetryEvent.Properties.Should().Contain(properties);
        }

        private RootConfiguration GetRootConfiguration(string testOutputPath, string bicepConfigContents, ConfigurationManager configurationManager)
        {
            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

            return configurationManager.GetConfiguration(bicepConfigUri.ToUri());
        }

        private BicepCompilationManager CreateBicepCompilationManager()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            return new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager), workspace, BicepTestConstants.FileResolver, BicepCompilationManagerHelper.CreateMockScheduler().Object, configurationManager, BicepTestConstants.CreateMockTelemetryProvider().Object, linterRulesProvider, BicepTestConstants.LinterAnalyzer);
        }
    }
}
