// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ConfigurationManager = Bicep.Core.Configuration.ConfigurationManager;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ModuleReferenceCompletionProviderTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private IAzureContainerRegistriesProvider azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>().Object;
        private static IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>().Object;
        private ISettingsProvider settingsProvider = StrictMock.Of<ISettingsProvider>().Object;

        // TODO: We need improved assertions for all the completion item tests

        [DataTestMethod]
        [DataRow("module test |''", 14)]
        [DataRow("module test ''|", 14)]
        [DataRow("module test '|'", 14)]
        [DataRow("module test '|", 13)]
        [DataRow("module test |'", 13)]
        [DataRow("module test |", 12)]
        public async Task GetFilteredCompletions_WithBicepRegistryAndTemplateSpecShemaCompletionContext_ReturnsCompletionItems(string inputWithCursors, int expectedEnd)
        {
            var completionContext = GetBicepCompletionContext(inputWithCursors, null, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                BicepTestConstants.BuiltInOnlyConfigurationManager,
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Count().Should().Be(4);

            completions.Should().Contain(
                c => c.Label == "br/public:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Public Bicep registry" &&
                c.TextEdit!.TextEdit!.NewText == "'br/public:$0'" &&
                c.TextEdit.TextEdit.Range.Start.Line == 0 &&
                c.TextEdit.TextEdit.Range.Start.Character == 12 &&
                c.TextEdit.TextEdit.Range.End.Line == 0 &&
                c.TextEdit.TextEdit.Range.End.Character == expectedEnd);

            completions.Should().Contain(
                c => c.Label == "br:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Bicep registry" &&
                c.TextEdit!.TextEdit!.NewText == "'br:$0'" &&
                c.TextEdit.TextEdit.Range.Start.Line == 0 &&
                c.TextEdit.TextEdit.Range.Start.Character == 12 &&
                c.TextEdit.TextEdit.Range.End.Line == 0 &&
                c.TextEdit.TextEdit.Range.End.Character == expectedEnd);

            completions.Should().Contain(
                c => c.Label == "ts/" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Template spec (alias)" &&
                c.TextEdit!.TextEdit!.NewText == "'ts/$0'" &&
                c.TextEdit.TextEdit.Range.Start.Line == 0 &&
                c.TextEdit.TextEdit.Range.Start.Character == 12 &&
                c.TextEdit.TextEdit.Range.End.Line == 0 &&
                c.TextEdit.TextEdit.Range.End.Character == expectedEnd);

            completions.Should().Contain(
                c => c.Label == "ts:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Template spec" &&
                c.TextEdit!.TextEdit!.NewText == "'ts:$0'" &&
                c.TextEdit.TextEdit.Range.Start.Line == 0 &&
                c.TextEdit.TextEdit.Range.Start.Character == 12 &&
                c.TextEdit.TextEdit.Range.End.Line == 0 &&
                c.TextEdit.TextEdit.Range.End.Character == expectedEnd);
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithBicepRegistryAndTemplateSpecShemaCompletionContext_AndTemplateSpecAliasInBicepConfigFile_ReturnsCompletionItems()
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test"": {
        ""registry"": ""testacr.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      }
    },
    ""ts"": {
      ""mySpecRG"": {
        ""subscription"": ""00000000-0000-0000-0000-000000000000"",
        ""resourceGroup"": ""test-rg""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext("module test '|'", bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Count().Should().Be(5);

            foreach ( var c in completions) {
                c.Label.Should().MatchRegex("^(.*/)|(.*:)$");
                c.Kind.Should().Be(CompletionItemKind.Reference);
                c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                c.InsertText.Should().BeNull();
                c.TextEdit!.TextEdit!.NewText.Should().MatchRegex("^'.*\\$0'$");
                c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                c.TextEdit.TextEdit.Range.End.Character.Should().Be(14);
            }

            completions.Should().Contain(
                c => c.Label == "br:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Bicep registry" &&
                c.TextEdit!.TextEdit!.NewText == "'br:$0'");

            completions.Should().Contain(
                c => c.Label == "br/test:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Alias for br:testacr.azurecr.io/bicep/modules/" &&
                c.TextEdit!.TextEdit!.NewText == "'br/test:$0'");

            completions.Should().Contain(
                c => c.Label == "br/public:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Public Bicep registry" &&
                c.TextEdit!.TextEdit!.NewText == "'br/public:$0'");

            completions.Should().Contain(
                c => c.Label == "ts:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Template spec" &&
                c.TextEdit!.TextEdit!.NewText == "'ts:$0'");

            completions.Should().Contain(
                c => c.Label == "ts/mySpecRG:" &&
                c.Kind == CompletionItemKind.Reference &&
                c.InsertTextFormat == InsertTextFormat.Snippet &&
                c.InsertText == null &&
                c.Detail == "Template spec" &&
                c.TextEdit!.TextEdit!.NewText == "'ts/mySpecRG:$0'");
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithInvalidTextInCompletionContext_ReturnsEmptyListOfCompletionItems()
        {
            var completionContext = GetBicepCompletionContext("module test 'br:/|'", null, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                BicepTestConstants.BuiltInOnlyConfigurationManager,
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1|")]
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1|'")]
        [DataRow("module test |'br/public:app/dapr-containerapp:1.0.1'")]
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1'|")]
        public async Task GetFilteredCompletions_WithInvalidCompletionContext_ReturnsEmptyList(string inputWithCursors)
        {
            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetVersions("app/dapr-containerapp")).ReturnsAsync(new List<PublicRegistryModuleVersion> { new("1.0.1", null, null), new("1.0.2", null, null) });

            var completionContext = GetBicepCompletionContext(inputWithCursors, null, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("module test 'br/|'", 17)]
        [DataRow("module test 'br/|", 16)]
        public async Task GetFilteredCompletions_WithAliasCompletionContext_ReturnsCompletionItems(string inputWithCursors, int expectedEnd)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""testacr.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""testacr2.azurecr.io""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("public");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br/public:$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be("test1");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br/test1:$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be("test2");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br/test2:$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:|'")]
        [DataRow("module test 'br:|")]
        public async Task GetFilteredCompletions_WithACRCompletionSettingSetToFalse_ReturnsACRCompletionItemsUsingBicepConfig(string inputWithCursors)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""testacr1.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""testacr2.azurecr.io""
      },
      ""test3"": {
        ""registry"": ""testacr2.azurecr.io""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("mcr.microsoft.com/bicep");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:mcr.microsoft.com/bicep/$0'"); ;
                },
                c =>
                {
                    c.Label.Should().Be("testacr1.azurecr.io");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:testacr1.azurecr.io/$0'");
                },
                c =>
                {
                    c.Label.Should().Be("testacr2.azurecr.io");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:testacr2.azurecr.io/$0'");
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:|'")]
        [DataRow("module test 'br:|")]
        public async Task GetFilteredCompletions_WithACRCompletionsSettingSetToTrue_ReturnsACRCompletionItemsUsingResourceGraphClient(string inputWithCursors)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');

            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);

            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""testacr1.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""testacr2.azurecr.io""
      }
    }
  }
}";
            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(true);

            var azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>();
            azureContainerRegistriesProvider.Setup(x => x.GetRegistryUris(documentUri.ToUriEscaped(), CancellationToken.None)).Returns(new List<string> { "testacr3.azurecr.io", "testacr4.azurecr.io" }.ToAsyncEnumerable());

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("mcr.microsoft.com/bicep");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:mcr.microsoft.com/bicep/$0'");
                },
                c =>
                {
                    c.Label.Should().Be("testacr3.azurecr.io");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:testacr3.azurecr.io/$0'");
                },
                c =>
                {
                    c.Label.Should().Be("testacr4.azurecr.io");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:testacr4.azurecr.io/$0'");
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:|'")]
        [DataRow("module test 'br:|")]
        public async Task GetFilteredCompletions_WithACRCompletionsSettingSetToTrue_AndNoAccessibleRegistries_ReturnsNoACRCompletions(
            string inputWithCursors)
        {
            var completionContext = GetBicepCompletionContext(inputWithCursors, null, out DocumentUri documentUri);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(true);

            var azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>();
            azureContainerRegistriesProvider.Setup(x => x.GetRegistryUris(documentUri.ToUriEscaped(), CancellationToken.None)).Returns(new List<string>().ToAsyncEnumerable());

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("mcr.microsoft.com/bicep");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:mcr.microsoft.com/bicep/$0'");
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|'", "app/dapr-cntrapp1", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp1:$0'", "app/dapr-cntrapp2", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp2:$0'", 41)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|", "app/dapr-cntrapp1", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp1:$0'", "app/dapr-cntrapp2", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp2:$0'", 40)]
        [DataRow("module test 'br/public:|'", "app/dapr-cntrapp1", "'br/public:app/dapr-cntrapp1:$0'", "app/dapr-cntrapp2", "'br/public:app/dapr-cntrapp2:$0'", 24)]
        [DataRow("module test 'br/public:|", "app/dapr-cntrapp1", "'br/public:app/dapr-cntrapp1:$0'", "app/dapr-cntrapp2", "'br/public:app/dapr-cntrapp2:$0'", 23)]
        public async Task GetFilteredCompletions_WithPublicMcrModuleRegistryCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel1,
            string expectedCompletionText1,
            string expectedLabel2,
            string expectedCompletionText2,
            int expectedEnd)
        {
            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetModules()).ReturnsAsync(new List<PublicRegistryModule> { new("app/dapr-cntrapp1", null, null), new("app/dapr-cntrapp2", "description2", "contoso.com/help2") });

            var completionContext = GetBicepCompletionContext(inputWithCursors, null, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                BicepTestConstants.BuiltInOnlyConfigurationManager,
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be(expectedLabel1);
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.Documentation.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expectedCompletionText1);
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be(expectedLabel2);
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("description2");
                    c.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help2)");
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expectedCompletionText2);
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:testacr1.azurecr.io/|'", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 37)]
        [DataRow("module test 'br:testacr1.azurecr.io/|", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 36)]
        public async Task GetFilteredCompletions_WithPathCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedCompletionText,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""testacr1.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""testacr2.azurecr.io""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().Contain(
                x => x.Label == expectedLabel &&
                x.Kind == CompletionItemKind.Reference &&
                x.InsertText == null &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText &&
                x.TextEdit!.TextEdit!.Range.Start.Line == startLine &&
                x.TextEdit!.TextEdit!.Range.Start.Character == startCharacter &&
                x.TextEdit!.TextEdit!.Range.End.Line == endLine &&
                x.TextEdit!.TextEdit!.Range.End.Character == endCharacter);
        }

        [DataTestMethod]
        [DataRow("module test 'br/public:app/dapr-containerapp:|'", "1.0.2", "'br/public:app/dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", "1_1.0.1", 46)]
        [DataRow("module test 'br/public:app/dapr-containerapp:|", "1.0.2", "'br/public:app/dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", "1_1.0.1", 45)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|'", "1.0.2", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'$0", "1_1.0.1", 63)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|", "1.0.2", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'$0", "1_1.0.1", 62)]
        [DataRow("module test 'br/test1:dapr-containerapp:|'", "1.0.2", "'br/test1:dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br/test1:dapr-containerapp:1.0.1'$0", "1_1.0.1", 41)]
        [DataRow("module test 'br/test1:dapr-containerapp:|", "1.0.2", "'br/test1:dapr-containerapp:1.0.2'$0", "0_1.0.2", "1.0.1", "'br/test1:dapr-containerapp:1.0.1'$0", "1_1.0.1", 40)]
        public async Task GetFilteredCompletions_WithMcrVersionCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel1,
            string expectedCompletionText1,
            string expectedSortText1,
            string expectedLabel2,
            string expectedCompletionText2,
            string expectedSortText2,
            int expectedEnd)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep/app""
      },
      ""test2"": {
        ""registry"": ""mcr.microsoft.com""
      }
    }
  }
}";
            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetVersions("app/dapr-containerapp")).ReturnsAsync(new List<PublicRegistryModuleVersion> { new("1.0.2", null, null), new("1.0.1", "d2", "contoso.com/help%20page.html") });

            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().Contain(c => c.Label == expectedLabel1)
                .Which.Should().Match<CompletionItem>(x =>
                x.Kind == CompletionItemKind.Snippet &&
                x.InsertText == null &&
                x.SortText == expectedSortText1 &&
                x.Detail == null &&
                x.Documentation == null &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText1 &&
                x.TextEdit!.TextEdit!.Range.Start.Line == 0 &&
                x.TextEdit!.TextEdit!.Range.Start.Character == 12 &&
                x.TextEdit!.TextEdit!.Range.End.Line == 0 &&
                x.TextEdit!.TextEdit!.Range.End.Character == expectedEnd);

            completions.Should().Contain(c => c.Label == expectedLabel2)
                .Which.Should().Match<CompletionItem>(x =>
                x.Kind == CompletionItemKind.Snippet &&
                x.InsertText == null &&
                x.SortText == expectedSortText2 &&
                x.Detail == "d2" &&
                x.Documentation!.MarkupContent!.Value == "[View Documentation](contoso.com/help%20page.html)" &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText2 &&
                x.TextEdit!.TextEdit!.Range.Start.Line == 0 &&
                x.TextEdit!.TextEdit!.Range.Start.Character == 12 &&
                x.TextEdit!.TextEdit!.Range.End.Line == 0 &&
                x.TextEdit!.TextEdit!.Range.End.Character == expectedEnd);
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithMcrVersionCompletionContext_AndNoMatchingModuleName_ReturnsEmptyListOfCompletionItems()
        {
            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetVersions("app/dapr-containerappapp")).ReturnsAsync(new List<PublicRegistryModuleVersion>());

            var completionContext = GetBicepCompletionContext("module test 'br/public:app/dapr-containerappapp:|'", null, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("module test 'br:testacr1.azurecr.io/|'", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 37)]
        [DataRow("module test 'br:testacr1.azurecr.io/|", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 36)]
        public async Task GetFilteredCompletions_WithPublicAliasOverriddenInBicepConfigAndPathCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedCompletionText,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""public"": {
        ""registry"": ""testacr1.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""testacr2.azurecr.io""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            completions.Should().Contain(
                x => x.Label == expectedLabel &&
                x.Kind == CompletionItemKind.Reference &&
                x.InsertText == null &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText &&
                x.TextEdit!.TextEdit!.Range.Start.Line == startLine &&
                x.TextEdit!.TextEdit!.Range.Start.Character == startCharacter &&
                x.TextEdit!.TextEdit!.Range.End.Line == endLine &&
                x.TextEdit!.TextEdit!.Range.End.Character == endCharacter);
        }

        [DataTestMethod]
        [DataRow("module test 'br/test1:|'", "dapr-containerappapp", "'br/test1:dapr-containerappapp:$0'", 0, 12, 0, 23)]
        [DataRow("module test 'br/test1:|", "dapr-containerappapp", "'br/test1:dapr-containerappapp:$0'", 0, 12, 0, 22)]
        [DataRow("module test 'br/test2:|'", "bicep/app/dapr-containerappapp", "'br/test2:bicep/app/dapr-containerappapp:$0'", 0, 12, 0, 23)]
        [DataRow("module test 'br/test2:|", "bicep/app/dapr-containerappapp", "'br/test2:bicep/app/dapr-containerappapp:$0'", 0, 12, 0, 22)]
        public async Task GetFilteredCompletions_WithAliasForMCRInBicepConfigAndModulePath_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedCompletionText,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep/app""
      },
      ""test2"": {
        ""registry"": ""mcr.microsoft.com""
      }
    }
  }
}";
            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetModules()).ReturnsAsync(new List<PublicRegistryModule> { new("app/dapr-containerappapp", "dapr description", "contoso.com/help") });

            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            IEnumerable<CompletionItem> completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            CompletionItem actualCompletionItem = completions.First(x => x.Label == expectedLabel);
            actualCompletionItem.Kind.Should().Be(CompletionItemKind.Snippet);
            actualCompletionItem.InsertText.Should().BeNull();
            actualCompletionItem.Detail.Should().Be("dapr description");
            actualCompletionItem.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help)");

            var actualTextEdit = actualCompletionItem.TextEdit!.TextEdit;
            actualTextEdit.Should().NotBeNull();
            actualTextEdit!.NewText.Should().Be(expectedCompletionText);
            actualTextEdit!.Range.Start.Line.Should().Be(startLine);
            actualTextEdit!.Range.Start.Character.Should().Be(startCharacter);
            actualTextEdit!.Range.End.Line.Should().Be(endLine);
            actualTextEdit!.Range.End.Character.Should().Be(endCharacter);
        }

        [DataTestMethod]
        [DataRow("module foo 'br:mcr.microsoft.com/bicep/|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br:test.azurecr.io/|", ModuleRegistryType.ACR)]
        [DataRow("module foo 'br/public:|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br/test1:|", ModuleRegistryType.ACR)]
        [DataRow("module foo 'br/test2:|", ModuleRegistryType.ACR)]
        [DataRow("module foo 'br/test3:|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br/test4:|", ModuleRegistryType.MCR)]
        public async Task VerifyTelemetryEventIsPostedOnModuleRegistryPathCompletion(string inputWithCursors, string moduleRegistryType)
        {
            var bicepConfigFileContents = @"{
  ""moduleAliases"": {
    ""br"": {
      ""test1"": {
        ""registry"": ""myacr.azurecr.io"",
        ""modulePath"": ""bicep/modules""
      },
      ""test2"": {
        ""registry"": ""mytest.azurecr.io""
      },
      ""test3"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep/app""
      },
      ""test4"": {
        ""registry"": ""mcr.microsoft.com""
      }
    }
  }
}";
            var completionContext = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents, out DocumentUri documentUri);

            var publicRegistryModuleMetadataProvider = StrictMock.Of<IPublicRegistryModuleMetadataProvider>();
            publicRegistryModuleMetadataProvider.Setup(x => x.GetModules()).ReturnsAsync(new List<PublicRegistryModule> { new("app/dapr-cntrapp1", "description1", null), new("app/dapr-cntrapp2", null, "contoso.com/help2") });

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider.Object,
                settingsProvider,
                telemetryProvider.Object);
            await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, CancellationToken.None);

            telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                p => p.EventName == TelemetryConstants.EventNames.ModuleRegistryPathCompletion &&
                p.Properties != null &&
                p.Properties["moduleRegistryType"] == moduleRegistryType)), Times.Exactly(1));
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithACRCompletionsSettingSetToTrue_AndIsCanceled_EnumerationShouldBeCanceled()
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors("module test 'br:|'", '|');

            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(true);

            var cts = new CancellationTokenSource();
            var azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>();
            var firstItemReturned = false;
            var secondItemReturned = false;
            async IAsyncEnumerable<string> GetUris([EnumeratorCancellation] CancellationToken ct)
            {
                await Task.Delay(1);
                ct.ThrowIfCancellationRequested();
                firstItemReturned = true;
                yield return "testacr3.azurecr.io";

                // Cancel at source
                cts.Cancel();

                await Task.Delay(1);
                ct.ThrowIfCancellationRequested();
                secondItemReturned = true;
                yield return "testacr4.azurecr.io";
            }
            azureContainerRegistriesProvider.Setup(x => x.GetRegistryUris(documentUri.ToUriEscaped(), It.IsAny<CancellationToken>()))
                .Returns((Uri uri, CancellationToken ct) => GetUris(ct));

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                new ConfigurationManager(new IOFileSystem()),
                publicRegistryModuleMetadataProvider,
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);

            var func = () => moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUriEscaped(), completionContext, cts.Token);
            await func.Should().ThrowAsync<OperationCanceledException>();

            firstItemReturned.Should().BeTrue();
            secondItemReturned.Should().BeFalse();
        }

        private BicepCompletionContext GetBicepCompletionContext(
            string inputWithCursors,
            string? bicepConfigFileContents,
            out DocumentUri documentUri)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;

            if (bicepConfigFileContents is not null)
            {
                FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
            }

            return BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
        }
    }
}
