// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepEditLinterRuleCommandHandlerTests
    {
        // TODO: Refactor to use new LanguageServerMock under Mocks namespace

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

        private LanguageServerMock SetUpOnTriggerCompletion(LanguageServerMock server, Action<ShowDocumentParams> onShowDocument, Action<string?> onTriggerCompletion, bool enableShowDocumentCapability)
        {
            ShowDocumentParams? showDocumentParams = null;
            server.WindowMock.OnShowDocument(
                p =>
                {
                    showDocumentParams = p;
                    onShowDocument(p);
                },
                enableClientCapability: enableShowDocumentCapability);

            server.Mock
                .Setup(m => m.SendNotification("bicep/triggerEditorCompletion"))
                .Callback((string notification) =>
                {
                    if (showDocumentParams == null)
                    {
                        throw new Exception("Completion was triggered but no show document call was made");
                    }
                    else if (showDocumentParams.Selection == null)
                    {
                        throw new Exception("No selection was given in the show document call");
                    }
                    else if (showDocumentParams.Selection.Start != showDocumentParams.Selection.End)
                    {
                        throw new Exception("Completion was triggered on a non-empty selection");
                    }

                    string? stringTriggeredForCompletion = GetStringContentsAtDocumentPosition(showDocumentParams.Uri, showDocumentParams.Selection.Start);
                    onTriggerCompletion(stringTriggeredForCompletion);
                });

            return server;
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

            var server = new LanguageServerMock();

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: true);

            // act
            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            showDocumentParams.Should().NotBeNull();
            showDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            stringTriggeredForCompletion.Should().Be("no-unused-params-current-level", "rule's current level value should be selected and completion triggered when the config file is opened");
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "no-unused-params" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "false" },
                    { "error", string.Empty },
                    { "result", EventResult.Succeeded },
                });
        }

        [TestMethod]
        public async Task WhenClientDoesNotSupportShowDocumentRequestOrWorkspaceFolders_ShowDocumentParamsAndStringTriggeredForCompletionsShouldBeNull()
        {
            string expectedConfig = @"{
  // See https://aka.ms/bicep/config for more information on Bicep configuration options
  // Press CTRL+SPACE at any location to see Intellisense suggestions
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

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: false);

            server.WorkspaceMock.OnRequestWorkspaceFoldersThrow(enableClientCapability: false);

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", "", CancellationToken.None);

            showDocumentParams.Should().BeNull();
            stringTriggeredForCompletion.Should().BeNull();
            File.ReadAllText(expectedConfigPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "true" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", EventResult.Succeeded },
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

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: true);

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            showDocumentParams.Should().NotBeNull();
            showDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            stringTriggeredForCompletion.Should().Be("warning", "rule's current level value should be selected when the config file is opened");
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

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: true);

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None);

            showDocumentParams.Should().NotBeNull();
            showDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
            stringTriggeredForCompletion.Should().Be("warning", "new rule's level value should be selected and completion triggered when the config file is opened");
            File.ReadAllText(configPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", EventResult.Succeeded },
                });
        }

        [TestMethod]
        public async Task IfConfigExists_AndIsInvalid_ThenShowError_AndSendFailureTelemetry()
        {
            string bicepConfig = @"invalid json";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            server.WindowMock.OnShowDocument(p => showDocumentParams = p);

            string? message = null;
            server.WindowMock.OnShowMessage(p => message = p.Message);

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None);

            showDocumentParams.Should().BeNull("ShowTextDocument Shouldn't get called");
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "false" },
                    { "newRuleAdded", "false" },
                    { "error", "JsonReaderException" },
                    { "result", EventResult.Failed },
                });
            message.Should().Be("Unexpected character encountered while parsing value: i. Path '', line 0, position 0.");
        }

        [TestMethod]
        public async Task IfConfigDoesNotExist_ThenCreateAndAddRuleAndSelect()
        {
            string expectedConfig = @"{
  // See https://aka.ms/bicep/config for more information on Bicep configuration options
  // Press CTRL+SPACE at any location to see Intellisense suggestions
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

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: true);

            server.WorkspaceMock.OnRequestWorkspaceFolders(
                new Container<WorkspaceFolder>(
                    [new() { Name = "my workspace", Uri = DocumentUri.File(rootFolder) }]));

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            BicepTelemetryEvent? ev = null;
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                .Callback((BicepTelemetryEvent e) =>
                {
                    ev = e;
                });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", "", CancellationToken.None);

            showDocumentParams.Should().NotBeNull();
            showDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(expectedConfigPath.ToLowerInvariant());
            stringTriggeredForCompletion.Should().Be("warning", "new rule's level value should be selected and completion triggered when the config file is opened");
            File.ReadAllText(expectedConfigPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
            ev.Should().NotBeNull();
            ev!.EventName.Should().Be(TelemetryConstants.EventNames.EditLinterRule);
            ev.Properties.Should().Contain(new Dictionary<string, string> {
                    { "code", "whatever" },
                    { "newConfigFile", "true" },
                    { "newRuleAdded", "true" },
                    { "error", string.Empty },
                    { "result", EventResult.Succeeded },
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

            var server = new LanguageServerMock();

            ShowDocumentParams? showDocumentParams = null;
            string? stringTriggeredForCompletion = null;
            SetUpOnTriggerCompletion(server, p => showDocumentParams = p, s => stringTriggeredForCompletion = s, enableShowDocumentCapability: true);

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Mock.Object, server.ClientCapabilitiesProvider, telemetryProvider.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            showDocumentParams.Should().NotBeNull();
            showDocumentParams!.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());

            stringTriggeredForCompletion.Should().Be("warning", "rule's current level value should be selected and completion triggered when the config file is opened");
        }

    }
}
