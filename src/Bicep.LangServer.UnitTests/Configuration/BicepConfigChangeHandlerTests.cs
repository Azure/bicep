// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer;
using Bicep.LanguageServer.BicepConfig;
using Bicep.LanguageServer.Compilation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LocalFileSystem = System.IO.Abstractions.FileSystem;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Configuration
{
    [TestClass]
    public class BicepConfigLifecycleManagerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void HandleFileChanges_WithValidBicepConfigFile_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""info""
                    }
                  }
                }
              }
            }";

            HandleBicepConfigFileChange(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Information);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void HandleFileChanges_WithInvalidBicepConfigFile_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""warning""
            }";

            HandleBicepConfigFileChange(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out var configFilePath,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@$"Failed to parse the contents of the Bicep configuration file ""{configFilePath}"" as valid JSON: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 13.");
                    x.Severity.Should().Be(DiagnosticSeverity.Error);
                    x.Code?.String.Should().Be("BCP271");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(0, 0)
                    });
                },
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void HandleFileChanges_WithBicepConfigFileThatDoesntAdhereToSchema_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = """
                {
                  "analyzers": {
                    "core": {
                      "verbose": false,
                      "enabled": true,
                      "rules": {
                        "no-unused-params": {
                          "level": "abcdef"
                        }
                      }
                    }
                  }
                }
                """;

            HandleBicepConfigFileChange(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void HandleFileChanges_WithEmptySourceFile_ShouldNotRefreshCompilation()
        {
            var bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""warning""
                    }
                  }
                }
              }
            }";

            HandleBicepConfigFileChange(string.Empty,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out _,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void HandleBicepConfigOpenEvent_WithCyclicExtends_PublishesBCP454ToConfigFileUri()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile("/a/bicepconfig.json", """{ "extends": "./b/bicepconfig.json" }""");
            mockFileSystem.AddFile("/a/b/bicepconfig.json", """{ "extends": "../bicepconfig.json" }""");

            var published = OpenConfigFile(mockFileSystem, "/a/bicepconfig.json");

            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/a/bicepconfig.json"))].Should().ContainSingle(d => d.Code!.Value.String == "BCP454");
            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/a/b/bicepconfig.json"))].Should().BeEmpty();
        }

        [TestMethod]
        public void HandleBicepConfigOpenEvent_WithInvalidValueInBase_PublishesToBase()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile("/bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");
            mockFileSystem.AddFile("/base/bicepconfig.base.json", """{ "cloud": { "currentProfile": "Missing" } }""");

            var published = OpenConfigFile(mockFileSystem, "/bicepconfig.json");

            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/bicepconfig.json"))].Should().BeEmpty();
            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/base/bicepconfig.base.json"))].Should().ContainSingle(d => d.Code!.Value.String == "BCP273");
        }

        [TestMethod]
        public void HandleBicepConfigOpenEvent_WithDiagnosticWithoutFile_DoesNotPublishItToConfigFiles()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile("/base/bicepconfig.base.json", """{ "cloud": { "currentProfile": "Custom", "profiles": { "Custom": { "resourceManagerEndpoint": "https://management.example.invalid", "activeDirectoryAuthority": "https://login.example.invalid" } } } }""");
            mockFileSystem.AddFile("/bicepconfig.json", """{ "extends": "./base/bicepconfig.base.json" }""");

            var published = OpenConfigFile(mockFileSystem, "/bicepconfig.json", CloudConfigurationTrustPolicy.FromEnvironmentValue("{ not json"));

            // An invalid environment variable isn't caused by either file; Bicep files show it instead.
            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/bicepconfig.json"))].Should().BeEmpty();
            published[DocumentUri.From(InMemoryFileResolver.GetFileUri("/base/bicepconfig.base.json"))].Should().BeEmpty();
        }

        private static Dictionary<DocumentUri, Container<Diagnostic>> OpenConfigFile(MockFileSystem mockFileSystem, string configFilePath, CloudConfigurationTrustPolicy? trustPolicy = null)
        {
            var published = new Dictionary<DocumentUri, Container<Diagnostic>>();
            var document = BicepCompilationManagerHelper.CreateMockDocument(p => published[p.Uri] = p.Diagnostics);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            var fileExplorer = new FileSystemFileExplorer(mockFileSystem);
            var bicepConfigManager = new BicepConfigurationManager(fileExplorer, trustPolicy ?? BicepTestConstants.TestCloudConfigurationTrustPolicy);
            var lifecycleManager = new BicepConfigLifecycleManager(
                bicepConfigManager,
                new FileChangeEpoch(),
                server);

            lifecycleManager.HandleBicepConfigOpenEvent(DocumentUri.From(InMemoryFileResolver.GetFileUri(configFilePath)));

            return published;
        }

        [TestMethod]
        public void HandleFileChanges_WithoutBicepConfigFile_ShouldUseDefaultConfigAndRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            HandleBicepConfigFileChange(bicepFileContents,
                                                       string.Empty,
                                                       saveBicepConfigFile: false,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        private void HandleBicepConfigFileChange(string bicepFileContents, string bicepConfigFileContents, bool saveBicepConfigFile, out Mock<ITextDocumentLanguageServer> document, out string? bicepConfigFilePath, out Container<Diagnostic>? diagnostics)
        {
            PublishDiagnosticsParams? receivedParams = null;
            document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            var mockFileSystem = new MockFileSystem();

            var bicepFilePath = "/input.bicep";
            mockFileSystem.AddFile(bicepFilePath, bicepFileContents);

            var workspace = new ActiveSourceFileSet();
            var fileExplorer = new FileSystemFileExplorer(mockFileSystem);
            var fileChangeEpoch = new FileChangeEpoch();
            var bicepConfigManager = new BicepConfigurationManager(fileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: fileChangeEpoch);
            var configurationManager = bicepConfigManager;
            var sourceFileFactory = new SourceFileFactory(configurationManager, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.AuxiliaryFileCache, BicepTestConstants.FileExplorer);
            var bicepCompilationManager = new BicepCompilationManager(
                server,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager),
                workspace,
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                sourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache,
                configurationManager);
            bicepCompilationManager.OpenCompilation(DocumentUri.From(InMemoryFileResolver.GetFileUri(bicepFilePath)), null, bicepFileContents, LanguageConstants.LanguageId);

            // The configuration file is created after the compilation, so handling the file event is what makes the compilation use it.
            if (saveBicepConfigFile)
            {
                bicepConfigFilePath = mockFileSystem.Path.GetFullPath("/bicepconfig.json");
                mockFileSystem.AddFile(bicepConfigFilePath, bicepConfigFileContents);
            }
            else
            {
                bicepConfigFilePath = null;
            }

            fileChangeEpoch.StartNew();
            bicepCompilationManager.HandleFileChanges([new FileEvent
            {
                Uri = DocumentUri.FromFileSystemPath(mockFileSystem.Path.GetFullPath("/bicepconfig.json")),
                Type = saveBicepConfigFile ? FileChangeType.Created : FileChangeType.Deleted,
            }]);

            diagnostics = receivedParams?.Diagnostics;
        }

        [TestMethod]
        public void HandleFileChanges_WithUnchangedConfiguration_DoesNotRefreshCompilation()
        {
            var publishCount = 0;
            var document = BicepCompilationManagerHelper.CreateMockDocument(_ => publishCount++);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile("/input.bicep", "param p string");
            mockFileSystem.AddFile("/bicepconfig.json", "{}");

            var fileChangeEpoch = new FileChangeEpoch();
            var configurationManager = new BicepConfigurationManager(new FileSystemFileExplorer(mockFileSystem), BicepTestConstants.TestCloudConfigurationTrustPolicy, fileChangeEpoch: fileChangeEpoch);
            var bicepCompilationManager = new BicepCompilationManager(
                server,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager),
                new ActiveSourceFileSet(),
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                new SourceFileFactory(configurationManager, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.AuxiliaryFileCache, BicepTestConstants.FileExplorer),
                BicepTestConstants.AuxiliaryFileCache,
                configurationManager);
            bicepCompilationManager.OpenCompilation(DocumentUri.From(InMemoryFileResolver.GetFileUri("/input.bicep")), null, "param p string", LanguageConstants.LanguageId);
            var publishCountAfterOpen = publishCount;

            // For example, a .bicep file was saved: nothing the configuration was loaded from changed.
            mockFileSystem.AddFile("/other.bicep", "");
            fileChangeEpoch.StartNew();
            bicepCompilationManager.HandleFileChanges([new FileEvent
            {
                Uri = DocumentUri.FromFileSystemPath(mockFileSystem.Path.GetFullPath("/other.bicep")),
                Type = FileChangeType.Created,
            }]);

            publishCount.Should().Be(publishCountAfterOpen);
        }
    }
}
