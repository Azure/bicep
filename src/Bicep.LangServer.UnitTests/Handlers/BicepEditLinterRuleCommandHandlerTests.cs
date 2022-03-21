// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
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

        private string? GetSelectedTextFromFile(DocumentUri uri, Range? range)
        {
            if (range is null)
            {
                return null;
            }

            var contents = File.ReadAllText(uri.GetFileSystemPath());
            var lineStarts = TextCoordinateConverter.GetLineStarts(contents);
            var start = TextCoordinateConverter.GetOffset(lineStarts, range.Start.Line, range.Start.Character);
            var end = TextCoordinateConverter.GetOffset(lineStarts, range.End.Line, range.End.Character);

            var selectedText = contents.Substring(start, end - start);
            return selectedText;
        }

        private static Mock<ILanguageServerFacade> CreateMockLanguageServer(Action<ShowDocumentParams, CancellationToken> callback, ShowDocumentResult result, Container<WorkspaceFolder>? workspaceFolders = null)
        {
            var window = StrictMock.Of<IWindowLanguageServer>();

            window
                .Setup(m => m.SendNotification(It.IsAny<LogMessageParams>()));
            window
                .Setup(m => m.SendRequest<ShowDocumentResult>(It.IsAny<ShowDocumentParams>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<ShowDocumentResult> request, CancellationToken token) =>
                {
                    var @params = (ShowDocumentParams)request;
                    callback(@params, token);
                })
                .ReturnsAsync(() => result);

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

            string? selectedText = null;
            var server = CreateMockLanguageServer(
                (ShowDocumentParams @params, CancellationToken token) =>
                {
                    @params.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
                    selectedText = GetSelectedTextFromFile(@params.Uri, @params.Selection);
                },
                new ShowDocumentResult() { Success = true });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "no-unused-params", configPath, CancellationToken.None);

            selectedText.Should().Be("no-unused-params-current-level", "rule's current level value should be selected when the config file is opened");
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

            string? selectedText = null;
            var server = CreateMockLanguageServer(
                (ShowDocumentParams @params, CancellationToken token) =>
                {
                    @params.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(configPath.ToLowerInvariant());
                    selectedText = GetSelectedTextFromFile(@params.Uri, @params.Selection);
                },
                new ShowDocumentResult() { Success = true });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None);

            selectedText.Should().Be("warning", "new rule's level value should be selected when the config file is opened");
            File.ReadAllText(configPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
        }

        [TestMethod]
        public async Task IfConfigExists_AndIsInvalid_ThenThrow()
        {
            string bicepConfig = @"invalid json";

            var (bicepPath, configPath) = CreateFiles(bicepConfig);

            var server = CreateMockLanguageServer(
                (ShowDocumentParams @params, CancellationToken token) =>
                {
                },
                new ShowDocumentResult() { Success = true });

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Object);
            await FluentActions
                .Awaiting(() => bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", configPath, CancellationToken.None))
                .Should()
                .ThrowAsync<Newtonsoft.Json.JsonException>();
        }

        [TestMethod]
        public async Task IfConfigDoesNotExist_ThenCreateAndAddRuleAndSelect()
        {
            string expectedConfig = @"{
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

            string? selectedText = null;
            var server = CreateMockLanguageServer(
                (ShowDocumentParams @params, CancellationToken token) =>
                {
                    @params.Uri.GetFileSystemPath().ToLowerInvariant().Should().Be(expectedConfigPath.ToLowerInvariant());
                    selectedText = GetSelectedTextFromFile(@params.Uri, @params.Selection);
                },
                new ShowDocumentResult() { Success = true },
                new Container<WorkspaceFolder>(new WorkspaceFolder[] { new() { Name = "my workspace", Uri = DocumentUri.File(rootFolder) } }));

            BicepEditLinterRuleCommandHandler bicepEditLinterRuleHandler = new(StrictMock.Of<ISerializer>().Object, server.Object);
            await bicepEditLinterRuleHandler.Handle(new Uri(bicepPath), "whatever", "", CancellationToken.None);

            selectedText.Should().Be("warning", "new rule's level value should be selected when the config file is opened");
            File.ReadAllText(expectedConfigPath).Should().BeEquivalentToIgnoringNewlines(expectedConfig);
        }
    }
}
