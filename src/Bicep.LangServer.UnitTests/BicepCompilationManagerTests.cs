// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;
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
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationManagerTests
    {
        private const int BaseVersion = 42;

        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        private static readonly LinterRulesProvider linterRulesProvider = new();

        private static BicepCompilationManager GetTestBicepCompilationManager(Mock<ITextDocumentLanguageServer> document, Workspace? workspace = null)
        {
            workspace ??= new Workspace();
            return new BicepCompilationManager(
                BicepCompilationManagerHelper.CreateMockServer(document).Object,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(),
                workspace,
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);
        }

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
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUriEncoded();
            var workspace = new Workspace();
            var manager = BicepCompilationManagerHelper.CreateCompilationManager(uri, string.Empty);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpdateCompilation(uri, null, "hello");

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
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUriEncoded();

            var originalFile = BicepTestConstants.SourceFileFactory.CreateArmTemplateFile(uri.ToIOUri(), "{}");
            var workspace = new Workspace();
            workspace.UpsertSourceFile(originalFile);

            var manager = GetTestBicepCompilationManager(document, workspace);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpdateCompilation(uri, null, "hello");

            // second get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            workspace.TryGetSourceFile(uri, out var updatedFile);

            // The workspace should be refreshed.
            updatedFile.Should().NotBeNull();
            updatedFile!.FileHandle.Uri.Should().Be(originalFile.FileHandle.Uri);
            updatedFile.Should().NotBeSameAs(originalFile);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(LanguageConstants.ParamsLanguageId)]
        public void UpsertCompilation_BicepFile_ShouldUpsertSuccessfully(string languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var manager = GetTestBicepCompilationManager(document);
            var uri = CreateUri(languageId);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.OpenCompilation(uri, BaseVersion, "hello", languageId);
            var upserted = manager.GetCompilation(uri);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(string.Equals(languageId, LanguageConstants.ParamsLanguageId, StringComparison.Ordinal) ? 2 : 1);

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
        [DataRow(LanguageConstants.ParamsLanguageId)]
        public void CloseAfterUpsert_ShouldClearDiagnostics(string languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var uri = CreateUri(languageId);
            var manager = GetTestBicepCompilationManager(document);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.OpenCompilation(uri, BaseVersion, "hello", languageId);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(string.Equals(languageId, LanguageConstants.ParamsLanguageId, StringComparison.Ordinal) ? 2 : 1);

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
        [DataRow(LanguageConstants.ParamsLanguageId)]
        public void UpsertCompilation_ShouldUpdateDiagnostics(string languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var uri = CreateUri(languageId);
            var manager = GetTestBicepCompilationManager(document);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.OpenCompilation(uri, BaseVersion, "hello", languageId);
            var firstUpserted = manager.GetCompilation(uri);

            // should have pushed out diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(string.Equals(languageId, LanguageConstants.ParamsLanguageId, StringComparison.Ordinal) ? 2 : 1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var firstActual = manager.GetCompilation(uri);
            firstActual.Should().NotBeNull();

            // should be same as first upserted
            firstActual.Should().BeSameAs(firstUpserted);

            // upsert second one
            const int newVersion = BaseVersion + 1;
            manager.UpdateCompilation(uri, newVersion, "hello\r\nthere\r\n");
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
            receivedParams.Diagnostics.Count().Should().Be(string.Equals(languageId, LanguageConstants.ParamsLanguageId, StringComparison.Ordinal) ? 3 : 2);

            // get latest
            var secondActual = manager.GetCompilation(uri);
            secondActual.Should().BeSameAs(secondUpserted);
        }

        [TestMethod]
        public void GetNonExistentCompilation_ShouldNotThrow()
        {
            var server = Repository.Create<ILanguageServerFacade>();

            var manager = new BicepCompilationManager(
                server.Object,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(),
                new Workspace(),
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);

            var uri = DocumentUri.File($"{TestContext.TestName}.bicep");

            manager.GetCompilation(uri).Should().BeNull();
        }

        [TestMethod]
        public void CloseNonExistentCompilation_ShouldClearDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var manager = GetTestBicepCompilationManager(document);
            var uri = DocumentUri.File($"{TestContext.TestName}.bicep");

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
        [DataRow(LanguageConstants.ParamsLanguageId)]
        public void FatalException_ShouldProduceCorrectDiagnosticsAndClearThemWhenFileIsClosed(string languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";
            provider.Setup(
                m => m.Create(
                    It.IsAny<IReadOnlyWorkspace>(),
                    It.IsAny<DocumentUri>(),
                    It.IsAny<ImmutableDictionary<ISourceFile, ISemanticModel>>()))
                    .Throws(new InvalidOperationException(expectedMessage));

            var uri = CreateUri(languageId);

            var manager = new BicepCompilationManager(
                server.Object,
                provider.Object,
                new Workspace(),
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);

            // upsert should fail because of the mock fatal exception
            manager.OpenCompilation(uri, BaseVersion, "fake", languageId);
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
        [DataRow(LanguageConstants.ParamsLanguageId)]
        public void NormalUpsertAfterFatalException_ShouldReplaceDiagnostics(string languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";

            const int version = 74;
            var uri = CreateUri(languageId);

            // start by failing
            bool failUpsert = true;
            provider
                .Setup(m => m.Create(
                    It.IsAny<IReadOnlyWorkspace>(),
                    It.IsAny<DocumentUri>(),
                    It.IsAny<ImmutableDictionary<ISourceFile, ISemanticModel>>()))
                .Returns<IReadOnlyWorkspace, DocumentUri, ImmutableDictionary<ISourceFile, ISemanticModel>>((grouping, documentUri, modelLookup) => failUpsert
                    ? throw new InvalidOperationException(expectedMessage)
                    : BicepCompilationManagerHelper.CreateEmptyCompilationProvider().Create(grouping, documentUri, modelLookup));

            var workspace = new Workspace();

            var manager = new BicepCompilationManager(
                server.Object,
                provider.Object,
                workspace,
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);

            // upsert should fail because of the mock fatal exception
            manager.OpenCompilation(uri, version, "fake", languageId);
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
            manager.UpdateCompilation(uri, version, "fake\nfake\nfake\n");
            var upserted = manager.GetCompilation(uri);
            upserted.Should().NotBeNull();

            // new diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
            receivedParams.Diagnostics.Should().HaveCount(string.Equals(languageId, LanguageConstants.ParamsLanguageId, StringComparison.Ordinal) ? 4 : 3);

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

            var fileExplorer = new InMemoryFileExplorer();

            foreach (var (uri, content) in fileDict)
            {
                fileExplorer.GetFile(uri.ToIOUri()).Write(content);
            }

            var diagsReceived = new List<PublishDiagnosticsParams>();
            var document = BicepCompilationManagerHelper.CreateMockDocument(p => diagsReceived.Add(p));
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var services = new ServiceBuilder()
                .WithFileExplorer(fileExplorer)
                .Build();

            var compilationProvider = new BicepCompilationProvider(
                BicepTestConstants.EmptyEnvironment,
                TestTypeHelper.CreateEmptyNamespaceProvider(),
                fileExplorer,
                services.Construct<IModuleDispatcher>(),
                BicepTestConstants.LinterAnalyzer,
                BicepTestConstants.SourceFileFactory);

            var compilationManager = new BicepCompilationManager(
                server.Object,
                compilationProvider,
                new Workspace(),
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);

            diagsReceived.Should().BeEmpty();

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
                compilationManager.OpenCompilation(uris.main, 1, fileDict[uris.main], "bicep");

                diagsReceived.Should().SatisfyRespectively(
                    x => x.Uri.ToUriEncoded().Should().Be(uris.main)
                );
                diagsReceived.Clear();
            }

            // open all files
            {
                compilationManager.OpenCompilation(uris.moduleA, 1, fileDict[uris.moduleA], "bicep");
                compilationManager.OpenCompilation(uris.moduleB, 1, fileDict[uris.moduleB], "bicep");
                compilationManager.OpenCompilation(uris.moduleC, 1, fileDict[uris.moduleC], "bicep");
                diagsReceived.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting moduleA should only retrigger necessary recompilations
            {
                compilationManager.OpenCompilation(uris.moduleA, 2, fileDict[uris.moduleA], "bicep");

                diagsReceived.Should().SatisfyRespectively(
                    x => x.Uri.ToUriEncoded().Should().Be(uris.moduleA),
                    x => x.Uri.ToUriEncoded().Should().Be(uris.main)
                );
                diagsReceived.Clear();

                var compilations = GetCompilations();
                var moduleBSource = compilations[uris.moduleB].SourceFileGrouping.EntryPoint;
                var moduleBModel = compilations[uris.moduleB].GetSemanticModel(moduleBSource);

                compilations[uris.main].GetSemanticModel(moduleBSource).Should().BeSameAs(moduleBModel);
                compilations[uris.moduleA].GetSemanticModel(moduleBSource).Should().BeSameAs(moduleBModel);

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting moduleC should only retrigger necessary recompilations
            {
                compilationManager.OpenCompilation(uris.moduleC, 2, fileDict[uris.moduleC], "bicep");

                diagsReceived.Should().SatisfyRespectively(
                    x => x.Uri.ToUriEncoded().Should().Be(uris.moduleC),
                    x => x.Uri.ToUriEncoded().Should().Be(uris.main)
                );
                diagsReceived.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }

            // upserting main should only retrigger necessary recompilations
            {
                compilationManager.OpenCompilation(uris.main, 3, fileDict[uris.main], "bicep");

                diagsReceived.Should().SatisfyRespectively(
                    x => x.Uri.ToUriEncoded().Should().Be(uris.main)
                );
                diagsReceived.Clear();

                EnsureSemanticModelsAndSourceFilesDeduplicated();
            }
        }

        [TestMethod]
        public void Updates_to_untitled_file_should_match_selected_language()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var bicepFileUri = DocumentUri.File("untitled-1");
            var paramsFileUri = DocumentUri.File("untitled-2");
            var manager = GetTestBicepCompilationManager(document);

            // first gets should not return anything
            manager.GetCompilation(bicepFileUri).Should().BeNull();

            manager.GetCompilation(paramsFileUri).Should().BeNull();

            // open the untitled bicep file
            manager.OpenCompilation(bicepFileUri, BaseVersion, string.Empty, LanguageConstants.LanguageId);

            // should have received diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(bicepFileUri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().BeEmpty();
            document.Invocations.Clear();

            // info should now be available for the file
            manager.GetCompilation(bicepFileUri).Should().NotBeNull();

            // open the untitled params file
            manager.OpenCompilation(paramsFileUri, BaseVersion, string.Empty, LanguageConstants.ParamsLanguageId);

            // info should now be available for the file
            manager.GetCompilation(paramsFileUri).Should().NotBeNull();

            // should have received diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(paramsFileUri);
            receivedParams.Version.Should().Be(BaseVersion);

            // just a "using" declaration diagnostic
            receivedParams.Diagnostics.Should().HaveCount(1);
            document.Invocations.Clear();

            // update the params file
            manager.UpdateCompilation(paramsFileUri, BaseVersion + 1, "param foo = 42");

            // should have received diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(paramsFileUri);
            receivedParams.Version.Should().Be(BaseVersion + 1);

            // just a "using" declaration diagnostic
            receivedParams.Diagnostics.Should().HaveCount(1);
            document.Invocations.Clear();

            // update the params file
            manager.UpdateCompilation(bicepFileUri, BaseVersion + 2, "param foo int\nparam bar string\n");

            // should have received diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(bicepFileUri);
            receivedParams.Version.Should().Be(BaseVersion + 2);

            // just two "unused param" diagnostics
            receivedParams.Diagnostics.Should().HaveCount(2);
            document.Invocations.Clear();

            // close first file
            manager.CloseCompilation(bicepFileUri);

            manager.GetCompilation(bicepFileUri).Should().BeNull();

            // close second file
            manager.CloseCompilation(paramsFileUri);

            manager.GetCompilation(paramsFileUri).Should().BeNull();
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

            var rootConfiguration = BicepTestConstants.GetConfiguration(bicepConfigFileContents);

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
            var rootConfiguration = BicepTestConstants.GetConfiguration(bicepConfigFileContents);

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
            var rootConfiguration = BicepTestConstants.GetConfiguration(bicepConfigFileContents);

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
        public void GetBicepOpenTelemetryEvent_ShouldReturnTelemetryEvent()
        {
            var result = CompilationHelper.Compile(@"param appInsightsName string = 'testAppInsightsName'

resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightsName
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

param location string = 'testLocation'");

            var model = result.Compilation.GetEntrypointSemanticModel();
            var sourceFile = (model.SourceFile as BicepFile)!;

            var compilationManager = CreateBicepCompilationManager();
            var telemetryEvent = compilationManager.GetBicepOpenTelemetryEvent(
                model,
                sourceFile,
                model.GetAllDiagnostics().ToDiagnostics(sourceFile.LineStarts));

            var properties = new Dictionary<string, string>
            {
                ["Modules"] = "0",
                ["Parameters"] = "2",
                ["Resources"] = "1",
                ["Variables"] = "0",
                ["CharCount"] = "294",
                ["LineCount"] = "12",
                ["Errors"] = "0",
                ["Warnings"] = "1",
                ["ModulesInReferencedFiles"] = "0",
                ["ParentResourcesInReferencedFiles"] = "0",
                ["ParametersInReferencedFiles"] = "0",
                ["VariablesInReferencedFiles"] = "0",
            };

            telemetryEvent.Should().NotBeNull();
            telemetryEvent!.EventName.Should().Be(TelemetryConstants.EventNames.BicepFileOpen);
            telemetryEvent.Properties.Should().Contain(properties);
        }

        [TestMethod]
        public void GetBicepParamOpenTelemetryEvent_ShouldReturnTelemetryEvent()
        {
            var result = CompilationHelper.CompileParams(
                ("main.bicep", """
                    param intParam int
                    """),
                ("parameters.bicepparam", """
                    using 'main.bicep'

                    param intParam = 123
                    """));

            var model = result.Compilation.GetEntrypointSemanticModel();
            var sourceFile = (model.SourceFile as BicepParamFile)!;

            var compilationManager = CreateBicepCompilationManager();
            var telemetryEvent = compilationManager.GetBicepParamOpenTelemetryEvent(
                model,
                sourceFile,
                model.GetAllDiagnostics().ToDiagnostics(sourceFile.LineStarts));

            var properties = new Dictionary<string, string>
            {
                ["CharCount"] = "40",
                ["LineCount"] = "3",
                ["Errors"] = "0",
                ["Warnings"] = "0",
            };

            telemetryEvent.Should().NotBeNull();
            telemetryEvent!.EventName.Should().Be(TelemetryConstants.EventNames.BicepParamFileOpen);
            telemetryEvent.Properties.Should().Contain(properties);
        }

        private BicepCompilationManager CreateBicepCompilationManager()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File($"{TestContext.TestName}.bicep");
            var workspace = new Workspace();

            return new BicepCompilationManager(
                server.Object,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(),
                workspace,
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                linterRulesProvider,
                BicepTestConstants.SourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);
        }

        private DocumentUri CreateUri(string languageId) => DocumentUri.File(this.TestContext.TestName + (languageId switch
        {
            LanguageConstants.LanguageId => LanguageConstants.LanguageFileExtension,
            LanguageConstants.ParamsLanguageId => LanguageConstants.ParamsFileExtension,
            _ => LanguageConstants.LanguageFileExtension
        }));
    }
}
