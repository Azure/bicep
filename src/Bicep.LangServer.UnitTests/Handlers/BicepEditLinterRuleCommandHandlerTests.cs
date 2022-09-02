// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepEditLinterRuleCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        #region Support

        private (string bicepPath, string configPath) CreateFiles(
            string? bicepConfig)
        {
            var tempFolder = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempFolder);

            var bicepPath = Path.Combine(tempFolder, "main.bicep");
            var configPath = Path.Combine(tempFolder, "bicepconfig.json");

            File.WriteAllText(bicepPath, "// bicep code");
            if (bicepConfig is not null)
            {
                File.WriteAllText(configPath, bicepConfig);
            }

            return (bicepPath, configPath);
        }

        private static string? GetStringContentsAtDocumentPosition(DocumentUri uri, Position? position)
        {
            if (position is null)
            {
                return null;
            }

            var contents = File.ReadAllText(uri.GetFileSystemPath());
            var lineStarts = TextCoordinateConverter.GetLineStarts(contents);
            var offset = TextCoordinateConverter.GetOffset(lineStarts, position.Line, position.Character);
            if (offset > 0 && contents[offset - 1] == '"')
            {
                var stringLength = contents.Substring(offset).IndexOf('"');
                if (stringLength >= 0)
                {
                    return GetTextFromFile(uri, new Range(position, new Position(position.Line, position.Character + stringLength)));
                }
            }

            return null;
        }

        private static string? GetTextFromFile(DocumentUri uri, Range? range)
        {
            if (range is null)
            {
                return null;
            }

            range.End.Character.Should().BeGreaterThanOrEqualTo(0);
            var contents = File.ReadAllText(uri.GetFileSystemPath());
            var lineStarts = TextCoordinateConverter.GetLineStarts(contents);
            var start = TextCoordinateConverter.GetOffset(lineStarts, range.Start.Line, range.Start.Character);
            var end = TextCoordinateConverter.GetOffset(lineStarts, range.End.Line, range.End.Character);

            var selectedText = contents.Substring(start, end - start);
            return selectedText;
        }

        private class MockClientCapabilitiesProvider
        {
            public Mock<IClientCapabilitiesProvider> Mock;

            public MockClientCapabilitiesProvider()
            {
                var clientCapabilitiesProvider = StrictMock.Of<IClientCapabilitiesProvider>();
                clientCapabilitiesProvider.Setup(m => m.DoesClientSupportShowDocumentRequest()).Returns(true);
                clientCapabilitiesProvider.Setup(m => m.DoesClientSupportWorkspaceFolders()).Returns(true);

                Mock = clientCapabilitiesProvider;
            }
        }

        private class LanguageServerMock
        {
            public Mock<ILanguageServerFacade> Mock;
            public string? StringTriggeredForCompletion { get; private set; }
            public ShowDocumentParams? ShowDocumentParams { get; private set; }

            public LanguageServerMock(
                ShowDocumentResult result,
                Action<string>? messageCallback = null,
                Container<WorkspaceFolder>? workspaceFolders = null)
            {
                var window = StrictMock.Of<IWindowLanguageServer>();

                window
                    .Setup(m => m.SendNotification(It.IsAny<LogMessageParams>()));
                window
                    .Setup(m => m.SendRequest<ShowDocumentResult>(It.IsAny<ShowDocumentParams>(), It.IsAny<CancellationToken>()))
                    .Callback((IRequest<ShowDocumentResult> request, CancellationToken token) =>
                    {
                        var @params = (ShowDocumentParams)request;
                        ShowDocumentParams = @params;
                    })
                    .ReturnsAsync(() => result);
                window
                    .Setup(m => m.SendNotification(It.IsAny<ShowMessageParams>()))
                    .Callback((IRequest request) =>
                    {
                        var @params = (ShowMessageParams)request;
                        if (messageCallback != null)
                        {
                            messageCallback(@params.Message);
                        }
                        else
                        {
                            Assert.Fail($"Message was displayed when no message expected: {@params.Message}");
                        }
                    });

                var workspace = StrictMock.Of<IWorkspaceLanguageServer>();
                workspace
                    .Setup(m => m.SendRequest<Container<WorkspaceFolder>?>(It.IsAny<WorkspaceFolderParams>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => workspaceFolders);

                var server = StrictMock.Of<ILanguageServerFacade>();
                server
                    .Setup(m => m.Window)
                    .Returns(window.Object);
                server
                    .Setup(m => m.Workspace)
                    .Returns(workspace.Object);
                server
                 .Setup(m => m.SendNotification("bicep/triggerEditorCompletion"))
                 .Callback((string notification) =>
                 {
                     if (this.ShowDocumentParams == null)
                     {
                         throw new Exception("Completion was triggered but no show document call was made");
                     }
                     if (this.ShowDocumentParams.Selection == null)
                     {
                         throw new Exception("No selection was given in the show document call");
                     }
                     if (this.ShowDocumentParams.Selection.Start != this.ShowDocumentParams.Selection.End)
                     {
                         throw new Exception("Completion was triggered on a non-empty selection");
                     }

                     StringTriggeredForCompletion = GetStringContentsAtDocumentPosition(this.ShowDocumentParams.Uri, this.ShowDocumentParams.Selection.Start);
                 });

                Mock = server;
            }
        }

        #endregion Support

        [TestMethod]
        public async Task IfConfigExists_AndContainsRuleAlready_ThenJustShowAndSelect()
        {
            string bicepConfig = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""whatever"": {
                      ""level"": ""error""
                    },
                    ""no-unused-params"": {
                      ""level"": ""no-unused-params-current-level""
                    }
                  }
                }
              }
            }";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = new LanguageServerMock(new ShowDocumentResult() { Success = true });

            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            server.ShowDocumentParams.Should().NotBeNull();
            server.ShowDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            server.StringTriggeredForCompletion.Should().Be("no-unused-params-current-level", "rule's current level value should be selected and completion triggered when the config file is opened");
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "no-unused-params" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "false" },
                    { "error", string.Empty },
                    { "result", Result.Succeeded },
                });
        }

        [TestMethod]
        public async Task WhenClientDoesNotSupportShowDocumentRequestAndWorkspaceFolders_ShowDocumentParamsAndStringTriggeredForCompletionsShouldBeNull()
        {
            string expectedConfig = @"{
  // See https://aka.ms/bicep/config for more information on Bicep configuration options
  // Press CTRL+SPACE/CMD+SPACE at any location to see Intellisense suggestions
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""whatever"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";

            var rootFolder = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(rootFolder);

            var bicepFolder = Path.Combine(rootFolder, "subfolder");
            Directory.CreateDirectory(bicepFolder);
            string bicepPath = Path.Combine(bicepFolder, "main.bicep");
            File.WriteAllText(bicepPath, "var a = 'hello'");

            string expectedConfigPath = Path.Join(bicepFolder, "bicepconfig.json");

            var server = new LanguageServerMock(
             new ShowDocumentResult() { Success = true },
              workspaceFolders: new Container<WorkspaceFolder>(new WorkspaceFolder[] { new() { Name = "my workspace", Uri = DocumentUri.File(rootFolder) } }));

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            var clientCapabilitiesProvider = StrictMock.Of<IClientCapabilitiesProvider>();
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportShowDocumentRequest()).Returns(false);
            clientCapabilitiesProvider.Setup(m => m.DoesClientSupportWorkspaceFolders()).Returns(false);

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Object, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", "", CancellationToken.None);

            server.ShowDocumentParams.Should().BeNull();
            server.StringTriggeredForCompletion.Should().BeNull();
            File.ReadAllText(expectedConfigPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "true" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", Result.Succeeded },
                });
        }

        [TestMethod]
        public async Task IfConfigExists_AndContainsRuleButNotLevel_ThenJustAddLevelAndSelect()
        {
            string bicepConfig = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                    }
                  }
                }
              }
            }";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = new LanguageServerMock(new ShowDocumentResult() { Success = true });

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();

            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object,telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            server.ShowDocumentParams.Should().NotBeNull();
            server.ShowDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            server.StringTriggeredForCompletion.Should().Be("warning", "rule's current level value should be selected when the config file is opened");
        }

        [TestMethod]
        public async Task IfConfigExists_AndDoesNotContainRule_ThenAddRuleAndSelect()
        {
            string bicepConfig = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""no-unused-params-current-level""
                    }
                  }
                }
              }
            }";
            string expectedConfig = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""whatever"": {
                      ""level"": ""warning""
                    },
                    ""no-unused-params"": {
                      ""level"": ""no-unused-params-current-level""
                    }
                  }
                }
              }
            }";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = new LanguageServerMock(new ShowDocumentResult() { Success = true });

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None);

            server.ShowDocumentParams.Should().NotBeNull();
            server.ShowDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            server.StringTriggeredForCompletion.Should().Be("warning", "new rule's level value should be selected and completion triggered when the config file is opened");
            File.ReadAllText(configPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", Result.Succeeded },
                });
        }

        [TestMethod]
        public async Task IfConfigExists_AndIsInvalid_ThenShowError_AndSendFailureTelemetry()
        {
            string bicepConfig = @"invalid json";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);
            string? message = null;

            var server = new LanguageServerMock(
                   new ShowDocumentResult() { Success = true },
                messageCallback: (msg) =>
                {
                    message = msg;
                });

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });
            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object,telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None);

            server.ShowDocumentParams.Should().BeNull("ShowTextDocument Shouldn't get called");
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "false" },
                    { "error", "JsonReaderException" },
                    { "result", Result.Failed },
                });
            message.Should().Be("Unexpected character encountered while parsing value: i. Path '', line 0, position 0.");
        }

        [TestMethod]
        public async Task IfConfigDoesNotExist_ThenCreateAndAddRuleAndSelect()
        {
            string expectedConfig = @"{
  // See https://aka.ms/bicep/config for more information on Bicep configuration options
  // Press CTRL+SPACE/CMD+SPACE at any location to see Intellisense suggestions
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""whatever"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";

            var rootFolder = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(rootFolder);

            var bicepFolder = Path.Combine(rootFolder, "subfolder");
            Directory.CreateDirectory(bicepFolder);
            string bicepPath = Path.Combine(bicepFolder, "main.bicep");
            File.WriteAllText(bicepPath, "var a = 'hello'");

            string expectedConfigPath = Path.Join(rootFolder, "bicepconfig.json");

            var server = new LanguageServerMock(
             new ShowDocumentResult() { Success = true },
              workspaceFolders: new Container<WorkspaceFolder>(new WorkspaceFolder[] { new() { Name = "my workspace", Uri = DocumentUri.File(rootFolder) } }));

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object,telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", "", CancellationToken.None);

            server.ShowDocumentParams.Should().NotBeNull();
            server.ShowDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(expectedConfigPath.ToLowerInvariant());
            server.StringTriggeredForCompletion.Should().Be("warning", "new rule's level value should be selected and completion triggered when the config file is opened");
            File.ReadAllText(expectedConfigPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "true" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", Result.Succeeded },
                });
        }

        [TestMethod]
        public async Task IfConfigExists_AndHasComments_AndContainsRuleButNotLevel_ThenJustAddLevelAndSelect()
        {
            string bicepConfig = @"{
              // Look, Mom!
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                    }
                  }
                }
              }
            }";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = new LanguageServerMock(new ShowDocumentResult() { Success = true });

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();

            var clientCapabilitiesProvider = new MockClientCapabilitiesProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, clientCapabilitiesProvider.Mock.Object,telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            server.ShowDocumentParams.Should().NotBeNull();
            server.ShowDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());

            server.StringTriggeredForCompletion.Should().Be("warning", "rule's current level value should be selected and completion triggered when the config file is opened");
        }

    }
}
