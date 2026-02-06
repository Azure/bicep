// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.CompilerServices;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Mock.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
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
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ModuleReferenceCompletionProviderTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private readonly IAzureContainerRegistriesProvider azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>().Object;
        private readonly ISettingsProvider settingsProvider = StrictMock.Of<ISettingsProvider>().Object;

        private static async Task<IEnumerable<CompletionItem>> GetAndResolveCompletionItems(BicepSourceFile sourceFile, BicepCompletionContext completionContext, ModuleReferenceCompletionProvider moduleReferenceCompletionProvider)
        {
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(sourceFile, completionContext, CancellationToken.None);
            var resolved = new List<CompletionItem>();
            foreach (var completion in completions)
            {
                var c = await moduleReferenceCompletionProvider.ResolveCompletionItem(completion, CancellationToken.None);
                resolved.Add(c);
            }

            return resolved;
        }

        [DataTestMethod]
        [DataRow("module test |''", 14)]
        [DataRow("module test ''|", 14)]
        [DataRow("module test '|'", 14)]
        [DataRow("module test '|", 13)]
        [DataRow("module test |'", 13)]
        [DataRow("module test |", 12)]
        public async Task GetFilteredCompletions_WithBicepRegistryAndTemplateSpecShemaCompletionContext_ReturnsCompletionItems(string inputWithCursors, int expectedEnd)
        {
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "test": {
                        "registry": "testacr.azurecr.io",
                        "modulePath": "bicep/modules"
                      }
                    },
                    "ts": {
                      "mySpecRG": {
                        "subscription": "00000000-0000-0000-0000-000000000000",
                        "resourceGroup": "test-rg"
                      }
                    }
                  }
                }
                """;

            var (completionContext, sourceFile) = GetBicepCompletionContext("module test '|'", bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Count().Should().Be(5);

            foreach (var c in completions)
            {
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
            var (completionContext, sourceFile) = GetBicepCompletionContext("module test 'br:/|'");
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        // CONSIDER: This doesn't actually test anything useful because the current code takes the entire string
        //   into account, and ignores where the cursor is.
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1|")]
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1|'")]
        [DataRow("module test |'br/public:app/dapr-containerapp:1.0.1'")]
        [DataRow("module test 'br/public:app/dapr-containerapp:1.0.1'|")]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1|'")]
        [DataRow("module test |'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'")]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'|")]
        [DataRow("module test 'br:contoso.com/app/dapr-containerapp:1.0.1|")]
        [DataRow("module test 'br:contoso.com/app/dapr-containerapp:1.0.1|'")]
        [DataRow("module test |'br:contoso.com/app/dapr-containerapp:1.0.1'")]
        [DataRow("module test 'br:contoso.com/app/dapr-containerapp:1.0.1'|")]
        public async Task GetFilteredCompletions_WithInvalidCompletionContext_ReturnsEmptyList(string inputWithCursors)
        {
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    ("bicep/app/dapr-containerapp", null, null, [new("1.0.1", null, null), new("1.0.2", null, null)])
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("module test 'br/|'", 17)]
        [DataRow("module test 'br/|", 16)]
        public async Task GetFilteredCompletions_WithAliasCompletionContext_ReturnsCompletionItems(string inputWithCursors, int expectedEnd)
        {
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "test1": {
                        "registry": "testacr.azurecr.io",
                        "modulePath": "bicep/modules"
                      },
                      "test2": {
                        "registry": "testacr2.azurecr.io"
                      }
                    }
                  }
                }
                """;
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "test1": {
                        "registry": "testacr1.azurecr.io",
                        "modulePath": "bicep/modules"
                      },
                      "test2": {
                        "registry": "testacr2.azurecr.io"
                      },
                      "test3": {
                        "registry": "testacr2.azurecr.io"
                      }
                    }
                  }
                }
                """;
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "test1": {
                        "registry": "testacr1.azurecr.io",
                        "modulePath": "bicep/modules"
                      },
                      "test2": {
                        "registry": "testacr2.azurecr.io"
                      }
                    }
                  }
                }
                """;
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(true);

            var azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>();
            var cloud = sourceFile.Configuration.Cloud;
            azureContainerRegistriesProvider.Setup(x => x.GetContainerRegistriesAccessibleFromAzure(cloud, CancellationToken.None)).Returns(new List<string> { "testacr3.azurecr.io", "testacr4.azurecr.io" }.ToAsyncEnumerable());

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors);

            var settingsProviderMock = StrictMock.Of<ISettingsProvider>();
            settingsProviderMock.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(true);

            var azureContainerRegistriesProvider = StrictMock.Of<IAzureContainerRegistriesProvider>();
            var cloud = sourceFile.Configuration.Cloud;
            azureContainerRegistriesProvider.Setup(x => x.GetContainerRegistriesAccessibleFromAzure(cloud, CancellationToken.None)).Returns(new List<string>().ToAsyncEnumerable());

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|'", "bicep/app/dapr-cntrapp1", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp1:$0'", "bicep/app/dapr-cntrapp2", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp2:$0'", 41)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|", "bicep/app/dapr-cntrapp1", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp1:$0'", "bicep/app/dapr-cntrapp2", "'br:mcr.microsoft.com/bicep/app/dapr-cntrapp2:$0'", 40)]
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
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-cntrapp1", null, null, []),
                    new("bicep/app/dapr-cntrapp2", "description2", "contoso.com/help2", []),
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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

        [TestMethod]
        public async Task GetFilteredCompletions_WithAvmModulePath_UsesSuffixLabelAndPrefixDescription()
        {
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/avm/ptn/ai-ml/ai-foundry", null, null, []),
                    new("bicep/avm/ptn/ai-ml/ai-platform", null, null, []),
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext("module test 'br/public:|'");
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().Contain(
                c => c.Label == "ai-foundry" &&
                c.LabelDetails != null &&
                c.LabelDetails.Description == "avm/ptn/ai-ml/" &&
                c.TextEdit!.TextEdit!.NewText == "'br/public:avm/ptn/ai-ml/ai-foundry:$0'");
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithAvmPathPrefix_ReturnsMatchingCompletion()
        {
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/avm/ptn/ai-ml/ai-foundry", null, null, []),
                    new("bicep/avm/ptn/ai-ml/ai-platform", null, null, []),
                    new("bicep/avm/ptn/ai-platform/baseline", null, null, []),
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext("module test 'br/public:avm/ptn/ai-ml/|'");
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().Contain(
                c => c.Label == "ai-foundry" &&
                c.TextEdit!.TextEdit!.NewText == "'br/public:avm/ptn/ai-ml/ai-foundry:$0'");
            completions.Should().NotContain(
                c => c.TextEdit!.TextEdit!.NewText == "'br/public:avm/ptn/ai-platform/baseline:$0'");
        }

        [DataTestMethod]
        [DataRow("module test 'br:registry.contoso.io/bicep/|'", "bar", "bicep/whatever/abc/foo/", "'br:registry.contoso.io/bicep/whatever/abc/foo/bar:$0'")]
        [DataRow("module test 'br:registry.contoso.io/bicep/|", "bar", "bicep/whatever/abc/foo/", "'br:registry.contoso.io/bicep/whatever/abc/foo/bar:$0'")]
        [DataRow("module test 'br/myRegistry:|'", "bar", "abc/foo/", "'br/myRegistry:abc/foo/bar:$0'")]
        [DataRow("module test 'br/myRegistry_noPath:|'", "bar", "bicep/whatever/abc/foo/", "'br/myRegistry_noPath:bicep/whatever/abc/foo/bar:$0'")]
        public async Task GetFilteredCompletions_WithPrivateModulePathCompletions_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedLabelDescription,
            string expectedCompletionText)
        {
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                null,
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "registry.contoso.io",
                    [
                        ("bicep/whatever/abc/foo/bar", "d1", "contoso.com/help1", []),
                    ])
                );

            var (completionContext, sourceFile) = GetBicepCompletionContext(
                inputWithCursors,
                """
                {
                    "moduleAliases": {
                        "br": {
                            "myRegistry": {
                                "registry": "registry.contoso.io",
                                "modulePath": "bicep/whatever"
                            },
                            "myRegistry_noPath": {
                                "registry": "registry.contoso.io"
                            }
                        }
                    }
                }
                """);

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be(expectedLabel);
                    c.LabelDetails.Should().NotBeNull();
                    c.LabelDetails!.Description.Should().Be(expectedLabelDescription);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.Detail.Should().Be("d1");
                    c.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help1)");
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expectedCompletionText);
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:testacr1.azurecr.io/|'", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 37)]
        [DataRow("module test 'br:testacr1.azurecr.io/|", "bicep/modules", "'br:testacr1.azurecr.io/bicep/modules:$0'", 0, 12, 0, 36)]
        public async Task GetFilteredCompletions_IfAliasesInBicepConfig_AndRegistriesNotAvailable_GetPartialCompletionsBasedOnConfigOnly(
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

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(null);

            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be(expectedLabel);
                    x.Kind.Should().Be(CompletionItemKind.Reference);
                    x.InsertText.Should().BeNull();
                    x.TextEdit!.TextEdit!.NewText.Should().Be(expectedCompletionText);
                    x.TextEdit!.TextEdit!.Range.Start.Line.Should().Be(startLine);
                    x.TextEdit!.TextEdit!.Range.Start.Character.Should().Be(startCharacter);
                    x.TextEdit!.TextEdit!.Range.End.Line.Should().Be(endLine);
                    x.TextEdit!.TextEdit!.Range.End.Character.Should().Be(endCharacter);
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br/public:app/dapr-containerapp:|'", "1.0.2", "'br/public:app/dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", "0001", 46)]
        [DataRow("module test 'br/public:app/dapr-containerapp:|", "1.0.2", "'br/public:app/dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", "0001", 45)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|'", "1.0.2", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'$0", "0001", 63)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|", "1.0.2", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1'$0", "0001", 62)]
        [DataRow("module test 'br/test1:dapr-containerapp:|'", "1.0.2", "'br/test1:dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br/test1:dapr-containerapp:1.0.1'$0", "0001", 41)]
        [DataRow("module test 'br/test1:dapr-containerapp:|", "1.0.2", "'br/test1:dapr-containerapp:1.0.2'$0", "0000", "1.0.1", "'br/test1:dapr-containerapp:1.0.1'$0", "0001", 40)]
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
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "test1": {
                        "registry": "mcr.microsoft.com",
                        "modulePath": "bicep/app"
                      },
                      "test2": {
                        "registry": "mcr.microsoft.com"
                      }
                    }
                  }
                }
                """;
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-containerapp", null, null, [new("1.0.1", "d2", "contoso.com/help%20page.html"), new("1.0.2", null, null)]),
                    new("bicep/app/dapr-containerappapp", null, null, [new("1.0.1", "d2", "contoso.com/help%20page.html"), new("1.0.2", null, null)])
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var catalog = new RegistryModuleCatalog(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-containerappapp", null, null, []),
                    new("bicep/app/app/dapr-cntrapp2", "description2", "contoso.com/help2", []),
                ]).Object,
                StrictMock.Of<IPrivateAcrModuleMetadataProviderFactory>().Object,
                StrictMock.Of<IContainerRegistryClientFactory>().Object,
                BicepTestConstants.BuiltInOnlyConfigurationManager
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext("module test 'br/public:app/dapr-containerappapp:|'");
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
            var bicepConfigFileContents = """
                {
                  "moduleAliases": {
                    "br": {
                      "public": {
                        "registry": "testacr1.azurecr.io",
                        "modulePath": "bicep/modules"
                      },
                      "test2": {
                        "registry": "testacr2.azurecr.io"
                      }
                    }
                  }
                }
                """;
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            var completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

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
        [DataRow("module test 'br/test1:|'", "dapr-containerapp", "'br/test1:dapr-containerapp:$0'", 0, 12, 0, 23)]
        [DataRow("module test 'br/test1:|", "dapr-containerapp", "'br/test1:dapr-containerapp:$0'", 0, 12, 0, 22)]
        [DataRow("module test 'br/test2:|'", "bicep/app/dapr-containerapp", "'br/test2:bicep/app/dapr-containerapp:$0'", 0, 12, 0, 23)]
        [DataRow("module test 'br/test2:|", "bicep/app/dapr-containerapp", "'br/test2:bicep/app/dapr-containerapp:$0'", 0, 12, 0, 22)]
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

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-containerapp", "dapr description", "contoso.com/help", []),
                ])
            );

            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
            IEnumerable<CompletionItem> completions = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be(expectedLabel);
                    x.Kind.Should().Be(CompletionItemKind.Snippet);
                    x.InsertText.Should().BeNull();
                    x.Detail.Should().Be("dapr description");
                    x.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help)");

                    var actualTextEdit = x.TextEdit!.TextEdit;
                    actualTextEdit.Should().NotBeNull();
                    actualTextEdit!.NewText.Should().Be(expectedCompletionText);
                    actualTextEdit!.Range.Start.Line.Should().Be(startLine);
                    actualTextEdit!.Range.Start.Character.Should().Be(startCharacter);
                    actualTextEdit!.Range.End.Line.Should().Be(endLine);
                    actualTextEdit!.Range.End.Character.Should().Be(endCharacter);
                });
        }

        [DataTestMethod]
        [DataRow("module foo 'br:mcr.microsoft.com/bicep/|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br:mytest.contoso.io/|", ModuleRegistryType.ACR, ModuleRegistryType.AcrBasePathFromAlias)]
        [DataRow("module foo 'br/public:|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br/test1acr:|", ModuleRegistryType.ACR)]
        [DataRow("module foo 'br/test2acr:|", ModuleRegistryType.ACR)]
        [DataRow("module foo 'br/test3mcr:|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br/test4mcr:|", ModuleRegistryType.MCR)]
        [DataRow("module foo 'br:yourtest.contoso.com/|", ModuleRegistryType.AcrBasePathFromAlias)]
        public async Task VerifyTelemetryEventIsPostedOnModuleRegistryPathCompletion(string inputWithCursors, params string[] moduleRegistryTypes)
        {
            var bicepConfigFileContents = @"{
              ""moduleAliases"": {
                ""br"": {
                  ""test1acr"": {
                    ""registry"": ""mytest.contoso.io"",
                    ""modulePath"": ""bicep/modules""
                  },
                  ""test2acr"": {
                    ""registry"": ""mytest.contoso.io""
                  },
                  ""test3mcr"": {
                    ""registry"": ""mcr.microsoft.com"",
                    ""modulePath"": ""bicep/app""
                  },
                  ""test4mcr"": {
                    ""registry"": ""mcr.microsoft.com""
                  },
                  ""test5unknownAcr"": {
                    ""registry"": ""yourtest.contoso.com"",
                    ""modulePath"": ""bicep/your/apps""
                  }
                }
              }
            }";
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-cntrapp1", "description1", null, []),
                    new("bicep/app/dapr-cntrapp2", null, "contoso.com/help2", [])
                ]),
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "mytest.contoso.io",
                    [
                        new("bicep/modules/app1", null, null, [])
                    ]
                )
            );

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                telemetryProvider.Object);
            var items = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);
            items.Should().HaveCountGreaterThanOrEqualTo(1);

            foreach (var moduleRegistryType in moduleRegistryTypes)
            {
                telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                    p => p.EventName == TelemetryConstants.EventNames.ModuleRegistryPathCompletion &&
                    p.Properties != null &&
                    p.Properties["moduleRegistryType"] == moduleRegistryType)), Times.Once(),
                    $"Should have fired telemetry event for module registry type {moduleRegistryType}");
            }
            telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                p => p.EventName == TelemetryConstants.EventNames.ModuleRegistryPathCompletion &&
                p.Properties != null &&
                !moduleRegistryTypes.Contains(p.Properties["moduleRegistryType"]))), Times.Never,
                $"Telemetry event fired for unexpected module registry type");
        }

        [DataTestMethod]
        [DataRow("module foo 'br:mcr.microsoft.com/bicep/|", null)]
        [DataRow("module foo 'br:mytest.contoso.io/|", ModuleRegistryResolutionType.AcrModulePath)]
        [DataRow("module foo 'br/public:|", null)]
        [DataRow("module foo 'br/public:app/dapr-cntrapp1|", null)]
        [DataRow("module foo 'br/public:app/dapr-cntrapp1:|", null)]
        [DataRow("module foo 'br/test1acr:|", ModuleRegistryResolutionType.AcrModulePath)]
        [DataRow("module foo 'br:mytest.contoso.io/bicep/modules/app1:|", ModuleRegistryResolutionType.AcrVersion)]
        [DataRow("module foo 'br/test1acr:app1:|", ModuleRegistryResolutionType.AcrVersion)]
        public async Task VerifyTelemetryEventIsPostedOnModuleRegistryCompletionItemResolution(string inputWithCursors, string? moduleResolutionType)
        {
            var bicepConfigFileContents = @"{
              ""moduleAliases"": {
                ""br"": {
                  ""test1acr"": {
                    ""registry"": ""mytest.contoso.io"",
                    ""modulePath"": ""bicep/modules""
                  }
                }
              }
            }";
            var (completionContext, sourceFile) = GetBicepCompletionContext(inputWithCursors, bicepConfigFileContents);

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                RegistryCatalogMocks.MockPublicMetadataProvider([
                    new("bicep/app/dapr-cntrapp1", "description1", null, [new("v1.1", null, null)]),
                ]),
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "mytest.contoso.io",
                    [
                        new("bicep/modules/app1", null, null, [new("v1.1", null, null)])
                    ]
                )
            );

            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider,
                catalog,
                settingsProvider,
                telemetryProvider.Object);
            var items = await GetAndResolveCompletionItems(sourceFile, completionContext, moduleReferenceCompletionProvider);
            items.Should().HaveCountGreaterThanOrEqualTo(1);

            if (moduleResolutionType is not null)
            {
                telemetryProvider.Verify(
                m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                    p => p.EventName == TelemetryConstants.EventNames.ModuleRegistryResolution &&
                    p.Properties != null &&
                    p.Properties["type"] == moduleResolutionType)),
                Times.Once,
                $"Telemetry event should have fired for resolution type {moduleResolutionType}");
            }
            else
            {
                telemetryProvider.Verify(
                m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                    p => p.EventName == TelemetryConstants.EventNames.ModuleRegistryResolution)),
                Times.Never,
                $"Telemetry event should not have fired");
            }
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithACRCompletionsSettingSetToTrue_AndIsCanceled_EnumerationShouldBeCanceled()
        {
            var (completionContext, sourceFile) = GetBicepCompletionContext("module test 'br:|'");

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
                await cts.CancelAsync();

                await Task.Delay(1);
                ct.ThrowIfCancellationRequested();
                secondItemReturned = true;
                yield return "testacr4.azurecr.io";
            }
            azureContainerRegistriesProvider.Setup(x => x.GetContainerRegistriesAccessibleFromAzure(It.IsAny<CloudConfiguration>(), It.IsAny<CancellationToken>()))
                .Returns((CloudConfiguration _, CancellationToken ct) => GetUris(ct));

            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(
                azureContainerRegistriesProvider.Object,
                RegistryCatalogMocks.CreateCatalogWithMocks(),
                settingsProviderMock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);

            var func = () => moduleReferenceCompletionProvider.GetFilteredCompletions(sourceFile, completionContext, cts.Token);
            await func.Should().ThrowAsync<OperationCanceledException>();

            firstItemReturned.Should().BeTrue();
            secondItemReturned.Should().BeFalse();
        }

        private (BicepCompletionContext, BicepSourceFile) GetBicepCompletionContext(
            string inputWithCursors,
            string? bicepConfigFileContents = null)
        {
            var documentUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');

            var files = new Dictionary<string, MockFileData>
            {
                ["/path/to/main.bicep"] = bicepFileContents,
            };

            if (bicepConfigFileContents is not null)
            {
                files["/path/to/bicepconfig.json"] = bicepConfigFileContents;
            }

            var configurationManager = bicepConfigFileContents is null
                ? BicepTestConstants.BuiltInOnlyConfigurationManager
                : IConfigurationManager.WithStaticConfiguration(BicepTestConstants.GetConfiguration(bicepConfigFileContents));

            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: true, configurationManager: configurationManager);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;

            return (BicepCompletionContext.Create(compilation, cursors[0]), compilation.SourceFileGrouping.EntryPoint);
        }
    }
}
