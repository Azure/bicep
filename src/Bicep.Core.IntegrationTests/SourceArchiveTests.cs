// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;
using Bicep.LanguageServer.Handlers;
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.IO;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class SourceArchiveTests : TestBase
    {
        private readonly TestExternalArtifactManager artifactManager;
        private readonly TestCompiler compiler;

        public SourceArchiveTests()
        {
            this.artifactManager = new TestExternalArtifactManager();
            this.compiler = TestCompiler
                .ForMockFileSystemCompilation()
                .ConfigureServices(services => services.AddExternalArtifactManager(this.artifactManager));
        }

        [TestMethod]
        public void TryUnpackFrom_SourceTgzFileHandle_ReturnsExpectedSourceArchive()
        {
            // Assert.
            var tgzFileHandle = CreateMockTgzFileHandle(
                ("__metadata.json", """
                    {
                      "entryPoint": "main.bicep",
                      "bicepVersion": "0.1.2",
                      "metadataVersion": 1,
                      "sourceFiles": [
                        {
                          "path": "main.bicep",
                          "archivePath": "files/main.bicep",
                          "kind": "bicep"
                        }
                      ],
                      "documentLinks": {}
                    }
                    """
                ),
                ("files/main.bicep", "bicep content"));

            // Act.
            var result = SourceArchive.TryUnpackFromFile(tgzFileHandle);

            // Assert.
            result.Should().BeSuccess();
            result.Unwrap().Should().HaveMetadataAndFiles("""
                {
                  "metadataVersion": 1,
                  "entryPoint": "main.bicep",
                  "bicepVersion": "0.1.2",
                  "sourceFiles": [
                    {
                      "path": "main.bicep",
                      "archivePath": "files/main.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    }
                  ],
                  "documentLinks": {}
                }
                """,
                ("files/main.bicep", "bicep content"));
        }

        [TestMethod]
        public async Task PackIntoBinaryData_WhenDataUnpacked_ProducesEquivelantSourceArchive()
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();
            var original = SourceArchive.CreateFrom(sourceFileGrouping);

            // Act.
            var packadData = original.PackIntoBinaryData();

            // Assert.
            var fileHandleMock = StrictMock.Of<IFileHandle>();
            fileHandleMock.Setup(x => x.Exists()).Returns(true);
            fileHandleMock.Setup(x => x.OpenRead()).Returns(packadData.ToStream());

            var unpackedArchiveResult = SourceArchive.TryUnpackFromFile(fileHandleMock.Object);
            unpackedArchiveResult.Should().BeSuccess();
            unpackedArchiveResult.Unwrap().Should().BeEquivalentTo(original);
        }

        [TestMethod]
        public async Task CreateFrom_WithAllModuleKinds_ReturnsExpectedSourceArchive()
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();

            // Act.
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Assert.
            var bicepRegistryModule1Template = (await this.compiler.CompileInline(SampleData.BicepRegistryModule1Text)).Template!;
            var bicepRegistryModule2Template = (await this.compiler.CompileInline(SampleData.BicepRegistryModule2Text)).Template!;

            sourceArchive.Should().HaveMetadataAndFiles($$"""
                {
                  "metadataVersion": 1,
                  "entryPoint": "main.bicep",
                  "bicepVersion": "{{SourceArchiveConstants.CurrentBicepVersion}}",
                  "sourceFiles": [
                    {
                      "path": "main.bicep",
                      "archivePath": "files/main.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "local1.bicep",
                      "archivePath": "files/local1.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "nested-local.bicep",
                      "archivePath": "files/nested-local.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "modules/local2.bicep",
                      "archivePath": "files/modules/local2.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "modules/arm-templates/arm-template.json",
                      "archivePath": "files/modules/arm-templates/arm-template.json",
                      "kind": "ArmTemplate",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "<cache>/br/mockregistry.io/test$module1/v1$/main.json",
                      "archivePath": "files/_cache_/br/mockregistry.io/test$module1/v1$/main.json",
                      "kind": "ArmTemplate",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "<cache>/br/mockregistry.io/test$module2/v2$/main.json",
                      "archivePath": "files/_cache_/br/mockregistry.io/test$module2/v2$/main.json",
                      "kind": "ArmTemplate",
                      "sourceArtifactId": "br:mockregistry.io/test/module2:v2"
                    }
                  ],
                  "documentLinks": {
                    "main.bicep": [
                      {
                        "range": "[0:14]-[0:28]",
                        "target": "local1.bicep"
                      },
                      {
                        "range": "[6:14]-[6:36]",
                        "target": "modules/local2.bicep"
                      },
                      {
                        "range": "[12:19]-[12:60]",
                        "target": "modules/arm-templates/arm-template.json"
                      },
                      { "range": "[18:12]-[18:48]",
                        "target": "<cache>/br/mockregistry.io/test$module1/v1$/main.json"
                      },
                      {
                        "range": "[24:12]-[24:48]",
                        "target": "<cache>/br/mockregistry.io/test$module2/v2$/main.json"
                      }
                    ],
                    "local1.bicep": [
                      {
                        "range": "[2:12]-[2:32]",
                        "target": "nested-local.bicep"
                      }
                    ]
                  }
                }
                """,
                ("files/main.bicep", SampleData.BicepMainModuleText),
                ("files/local1.bicep", SampleData.BicepLocalModule1Text),
                ("files/nested-local.bicep", SampleData.BicepNestedLocalModuleText),
                ("files/modules/local2.bicep", SampleData.BicepLocalModule2Text),
                ("files/modules/arm-templates/arm-template.json", SampleData.ArmTemplateModuleText),
                ("files/_cache_/br/mockregistry.io/test$module1/v1$/main.json", bicepRegistryModule1Template.ToString(Newtonsoft.Json.Formatting.Indented)),
                ("files/_cache_/br/mockregistry.io/test$module2/v2$/main.json", bicepRegistryModule2Template.ToString(Newtonsoft.Json.Formatting.Indented)));
        }

        [TestMethod]
        public async Task CreateFrom_WithReferencesToSameModules_HandlesSameReferencesProperly()
        {
            // Arrange.
            await this.artifactManager.PublishRegistryModules(
                new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                new("br:mockregistry.io/test/module2:v1", "param p2 string", WithSource: true),
                new("br:mockregistry.io/test/module1:v2", "param p12 string", WithSource: true));

            var sourceFileGrouping = await this.CreateSourceFileGrouping(
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }

                    module m1b 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: false
                      }
                    }

                    module local 'local.bicep' = {
                        params: {
                            p2: 'there'
                      }
                    }

                    module local2 'local.bicep' = {
                        params: {
                            p2: 'there'
                      }
                    }

                    module m12 'br:mockregistry.io/test/module1:v2' = {
                        params: {
                            p12: 'p12'
                      }
                    }
                    """),
                ("local.bicep", """
                    param p2 string

                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    module m2 'br:mockregistry.io/test/module2:v1' = {
                        params: {
                            p2: 'p2'
                      }
                    }
                    module m12 'br:mockregistry.io/test/module1:v2' = {
                        params: {
                            p12: 'p12'
                      }
                    }
                    """));

            // Act.
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Assert.
            using (new AssertionScope())
            {
                sourceArchive.FindSourceFile("main.bicep").Metadata.SourceArtifactId.Should().BeNull();
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v1$/main.json").Metadata.SourceArtifactId.Should().Be("br:mockregistry.io/test/module1:v1");
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module1/v2$/main.json").Metadata.SourceArtifactId.Should().Be("br:mockregistry.io/test/module1:v2");
                sourceArchive.FindSourceFile("<cache>/br/mockregistry.io/test$module2/v1$/main.json").Metadata.SourceArtifactId.Should().Be("br:mockregistry.io/test/module2:v1");
                sourceArchive.FindSourceFile("local.bicep").Metadata.SourceArtifactId.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task CreateFrom_WithEdgeCasePaths_CreatesExpectedArchivePaths()
        {
            // Arrange.
            var longPath1 = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/a.bicep";
            var longPath2 = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/b.bicep";
            var longPath3 = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/c.bicep";

            // /path/to/bicep
            // /path/foo/foo.bicep
            // /path/bar/bar.bicep
            var sourceFileGrouping = await this.CreateSourceFileGrouping(
                ("main.bicep", $$"""
                    module weirdo 'weirdo&+[]#.bicep' = {}
                    module foo '../foo/foo.bicep' = {}
                    module bar '../bar/bar.bicep' = {}

                    module longPath1 '{{longPath1}}' = {}
                    module longPath2 '{{longPath2}}' = {}
                    module longPath3 '{{longPath3}}' = {}
                    """),
                ("../foo/foo.bicep", ""),
                ("../bar/bar.bicep", """
                    module baz 'baz.bicep' = {}
                    """),
                ("../bar/baz.bicep", ""),
                ("weirdo&+[]#.bicep", ""),
                (longPath1, ""),
                (longPath2, ""),
                (longPath3, ""));

            // Act.
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Assert.
            using (new AssertionScope())
            {
                sourceArchive.FindSourceFile("weirdo&+[]#.bicep").Metadata.ArchivePath.Should().Be("files/weirdo_____.bicep");
                sourceArchive.FindSourceFile("<root2>/foo.bicep").Metadata.ArchivePath.Should().Be("files/_root2_/foo.bicep");
                sourceArchive.FindSourceFile("<root1>/bar.bicep").Metadata.ArchivePath.Should().Be("files/_root1_/bar.bicep");
                sourceArchive.FindSourceFile("<root1>/baz.bicep").Metadata.ArchivePath.Should().Be("files/_root1_/baz.bicep");
                sourceArchive.FindSourceFile(longPath1).Metadata.ArchivePath.Should().Be("files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 1__path_too_long__.bicep");
                sourceArchive.FindSourceFile(longPath2).Metadata.ArchivePath.Should().Be("files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 1__path_too_long__.bicep(1)");
                sourceArchive.FindSourceFile(longPath3).Metadata.ArchivePath.Should().Be("files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 1__path_too_long__.bicep(2)");
            }
        }

        [TestMethod]
        public async Task CreateFrom_WithNotResolvedModules_IgnoresThoseModules()
        {
            // Arrange.
            var moduleText = "param p1 bool";
            var moduleTemplate = (await this.compiler.CompileInline(moduleText)).Template!;
            var moduleTemplateText = moduleTemplate.ToString(Newtonsoft.Json.Formatting.Indented);
            await this.artifactManager.PublishRegistryModule("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true);

            var mainBicepText = """
                module m1 'br:mockregistry.io/test/module1:v1' = {
                    params: {
                        p1: true
                  }
                }

                module m2 'br:mockregistry.io/test/module2:v1' = { // not found
                    params: {
                        p1: true
                  }
                }

                module m3 'foobar.bicep' = {} // not found
                """;
            var sourceFileGrouping = await this.CreateSourceFileGrouping(("main.bicep", mainBicepText));

            // Act.
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Assert.
            sourceArchive.Should().HaveMetadataAndFiles($$"""
                {
                  "metadataVersion": 1,
                  "entryPoint": "main.bicep",
                  "bicepVersion": "{{SourceArchiveConstants.CurrentBicepVersion}}",
                  "sourceFiles": [
                    {
                      "path": "main.bicep",
                      "archivePath": "files/main.bicep",
                      "kind": "Bicep",
                      "sourceArtifactId": null
                    },
                    {
                      "path": "<cache>/br/mockregistry.io/test$module1/v1$/main.json",
                      "archivePath": "files/_cache_/br/mockregistry.io/test$module1/v1$/main.json",
                      "kind": "ArmTemplate",
                      "sourceArtifactId": "br:mockregistry.io/test/module1:v1"
                    }
                  ],
                  "documentLinks": {
                    "main.bicep": [
                      {
                        "range": "[0:10]-[0:46]",
                        "target": "<cache>/br/mockregistry.io/test$module1/v1$/main.json"
                      }
                    ]
                  }
                }
                """,
                ("files/main.bicep", mainBicepText),
                ("files/_cache_/br/mockregistry.io/test$module1/v1$/main.json", moduleTemplateText));
        }

        [DataTestMethod]
        [DataRow("main.bicep", "files/main.bicep", ArchivedSourceFileKind.Bicep, null)]
        [DataRow("local1.bicep", "files/local1.bicep", ArchivedSourceFileKind.Bicep, null)]
        [DataRow("modules/local2.bicep", "files/modules/local2.bicep", ArchivedSourceFileKind.Bicep, null)]
        [DataRow("modules/arm-templates/arm-template.json", "files/modules/arm-templates/arm-template.json", ArchivedSourceFileKind.ArmTemplate, null)]
        public async Task FindSourceFile_ExistingPath_ReturnsExpectedFile(string path, string expectedArchivePath, ArchivedSourceFileKind expectedKind, string? expectedSourceArtifactId)
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Act.
            var file = sourceArchive.FindSourceFile(path);

            // Assert.
            file.Metadata.ArchivePath.Should().Be(expectedArchivePath);
            file.Metadata.Kind.Should().Be(expectedKind);
            file.Metadata.SourceArtifactId.Should().Be(expectedSourceArtifactId);
        }

        [DataTestMethod]
        [DataRow("foo.bicep")]
        [DataRow("bar.bicep")]
        public async Task FindSourceFile_NonexistentPath_Throws(string path)
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Act & Assert.
            FluentActions.Invoking(() => sourceArchive.FindSourceFile(path))
                .Should().Throw<Exception>();
        }

        [TestMethod]
        public void FindSourceFile_WithOriginalV1MetadataFormat_SucceedsForBackwardCompatibility()
        {
            // DO NOT ADD TO THIS DATA - IT IS MEANT TO TEST READING
            // OLD FILE VERSIONS WITH MINIMAL DATA
            var sourceArchive = SourceArchive.TryUnpackFromFile(CreateMockTgzFileHandle(
                ("__metadata.json", """
                    {
                      "entryPoint": "main.bicep",
                      "bicepVersion": "0.1.2",
                      "metadataVersion": 1,
                      "sourceFiles": [
                        {
                          "path": "main.bicep",
                          "archivePath": "files/main.bicep",
                          "kind": "bicep"
                        }
                      ]
                    }
                    """
                ),
                ("files/main.bicep", "bicep content")
            )).UnwrapOrThrow();

            var file = sourceArchive.FindSourceFile("main.bicep");

            file.Metadata.Kind.Should().Be(ArchivedSourceFileKind.Bicep);
            file.Contents.Should().Be("bicep content");
        }

        [TestMethod]
        public void FindSourceFile_WithExtraPropertiesInMetadata_IgnoresForForwardCompatibility()
        {
            // Arrange.
            var sourceArchive = SourceArchive.TryUnpackFromFile(CreateMockTgzFileHandle(
                ("__metadata.json", """
                    {
                      "entryPoint": "main.bicep",
                      "I am an unrecognized property name": {},
                      "sourceFiles": [
                        {
                          "path": "main.bicep",
                          "archivePath": "files/main.bicep",
                          "kind": "bicep",
                          "I am also recognition challenged": "Hi, Mom!"
                        }
                      ],
                      "bicepVersion": "0.1.2",
                      "metadataVersion": 1
                    }
                    """
                ),
                ("I'm not mentioned in metadata.bicep", "unmentioned contents"),
                ("files/Nor am I.bicep", "unmentioned contents 2"),
                ("files/main.bicep", "bicep contents")
            )).UnwrapOrThrow();

            var file = sourceArchive.FindSourceFile("main.bicep");

            file.Metadata.Kind.Should().Be(ArchivedSourceFileKind.Bicep);
            file.Contents.Should().Be("bicep contents");
        }

        [TestMethod]
        public void FindSourceFile_WithIncompatibleOlderMetadataVersion_ReturnsErrorResult()
        {
            // Arrange.
            var sourceArchive = SourceArchive.TryUnpackFromFile(CreateMockTgzFileHandle(
                ("__metadata.json", """
                    {
                      "entryPoint": "file:///main.bicep",
                      "metadataVersion": <version>,
                      "bicepVersion": "0.whatever.0",
                      "sourceFiles": [
                        {
                          "path": "file:///main.bicep",
                          "archivePath": "main.bicep",
                          "kind": "bicep"
                        }
                      ]
                    }
                    """.Replace("<version>", (SourceArchiveConstants.CurrentMetadataVersion - 1).ToString())
                ),
                ("main.bicep", "bicep contents")));

            // Act.
            var success = sourceArchive.IsSuccess(out _, out var ex);

            // Assert.
            success.Should().BeFalse();
            ex.Should().NotBeNull();
            ex!.Message.Should().StartWith("This source code was published with an older, incompatible version of Bicep (0.whatever.0). You are using version ");
        }

        [TestMethod]
        public void FindSourceFile_WithIncompatibleNewerMetadataVersion_ReturnsErrorResult()
        {
            // Arrange.
            var result = SourceArchive.TryUnpackFromFile(CreateMockTgzFileHandle(
                ("__metadata.json", """
                    {
                      "entryPoint": "file:///main.bicep",
                      "metadataVersion": <version>,
                      "bicepVersion": "0.whatever.0",
                      "sourceFiles": [
                        {
                          "path": "file:///main.bicep",
                          "archivePath": "main.bicep",
                          "kind": "bicep"
                        }
                      ]
                    }
                    """.Replace("<version>", (SourceArchiveConstants.CurrentMetadataVersion + 1).ToString())
                ),
                ("main.bicep", "bicep contents")
            ));

            // Act.
            var success = result.IsSuccess(out _, out var ex);

            // Assert.
            success.Should().BeFalse();
            ex.Should().NotBeNull();
            ex!.Message.Should().StartWith("This source code was published with a newer, incompatible version of Bicep (0.whatever.0). You are using version ");
        }


        [TestMethod]
        public async Task FindDocumentLinks_ExistingPath_ReturnsExpectedLinks()
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Act.
            var mainLinks = sourceArchive.FindDocumentLinks("main.bicep");
            var localModule1Links = sourceArchive.FindDocumentLinks("local1.bicep");

            // Assert.
            using (new AssertionScope())
            {
                mainLinks.Should().BeEquivalentTo(new ArchivedSourceFileLink[]
                {
                    new(new TextRange(0, 14, 0, 28), "local1.bicep"),
                    new(new TextRange(6, 14, 6, 36), "modules/local2.bicep"),
                    new(new TextRange(12, 19, 12, 60), "modules/arm-templates/arm-template.json"),
                    new(new TextRange(18, 12, 18, 48), "<cache>/br/mockregistry.io/test$module1/v1$/main.json"),
                    new(new TextRange(24, 12, 24, 48), "<cache>/br/mockregistry.io/test$module2/v2$/main.json"),
                });

                localModule1Links.Should().BeEquivalentTo(new ArchivedSourceFileLink[]
                {
                    new(new TextRange(2, 12, 2, 32), "nested-local.bicep"),
                });
            }
        }

        [TestMethod]
        public async Task FindDocumentLinks_NonexistentPath_ReturnsEmptyLinks()
        {
            // Arange.
            var sourceFileGrouping = await this.CreateSourceFileGroupingWithAllModuleKinds();
            var sourceArchive = SourceArchive.CreateFrom(sourceFileGrouping);

            // Act.
            var links = sourceArchive.FindDocumentLinks("foobar.bicep");

            // Assert.
            links.Should().BeEmpty();
        }

        private static IFileHandle CreateMockTgzFileHandle(params (string name, string content)[] entries)
        {
            var stream = new MemoryStream();
            using (var writer = new TgzWriter(stream, leaveOpen: true))
            {
                foreach (var (name, content) in entries)
                {
                    writer.WriteEntry(name, content);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            var tgzFileHandleMock = StrictMock.Of<IFileHandle>();
            tgzFileHandleMock.Setup(x => x.Exists()).Returns(true);
            tgzFileHandleMock.Setup(x => x.OpenRead()).Returns(stream);

            return tgzFileHandleMock.Object;
        }

        private async Task<SourceFileGrouping> CreateSourceFileGrouping(params (string FilePath, TestFileData FileData)[] files)
        {
            var compilationResult = await this.compiler.Compile(files);

            return compilationResult.Compilation.SourceFileGrouping;
        }

        private async Task<SourceFileGrouping> CreateSourceFileGroupingWithAllModuleKinds()
        {
            await this.artifactManager.PublishRegistryModule(SampleData.BicepRegistryModule1ArtifactId, SampleData.BicepRegistryModule1Text, withSource: false);
            await this.artifactManager.PublishRegistryModule(SampleData.BicepRegistryModule2ArtifactId, SampleData.BicepRegistryModule2Text, withSource: true);
            this.artifactManager.UpsertTemplateSpec(SampleData.TemplateSpecId, SampleData.TemplateSpecModuleText);

            return await this.CreateSourceFileGrouping(
                ("main.bicep", SampleData.BicepMainModuleText),
                ("local1.bicep", SampleData.BicepLocalModule1Text),
                ("nested-local.bicep", SampleData.BicepNestedLocalModuleText),
                ("modules/local2.bicep", SampleData.BicepLocalModule2Text),
                ("modules/arm-templates/arm-template.json", SampleData.ArmTemplateModuleText));
        }

        private static class SampleData
        {
            public const string BicepMainModuleText = $$"""
                module local1 'local1.bicep' = {
                    params: {
                        blm1: 'hello'
                  }
                }

                module local2 'modules/local2.bicep' = {
                    params: {
                        blm2: 'there'
                  }
                }

                module armTemplate 'modules/arm-templates/arm-template.json' = {
                    params: {
                        atm: 'arm template'
                  }
                }

                module brm1 '{{BicepRegistryModule1ArtifactId}}' = {
                    params: {
                        brm1: 'registry module 1'
                  }
                }

                module brm2 '{{BicepRegistryModule2ArtifactId}}' = {
                    params: {
                        brm2: 'registry module 2'
                  }
                }

                module tsm '{{TemplateSpecArtifactId}}' = {
                    params: {
                        tsm: 'template spec module'
                  }
                }
                """;

            public const string BicepLocalModule1Text = """
                param blm1 string

                module bnlm 'nested-local.bicep' = {
                    params: {
                        bnlm: 'nested local module'
                  }
                }
                """;

            public const string BicepLocalModule2Text = """
                param blm2 string
                """;

            public const string BicepLocalModule3Text = """
                param blm3 string
                """;

            public const string BicepNestedLocalModuleText = """
                param bnlm string
                """;

            public const string ArmTemplateModuleText = """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                  "contentVersion": "1.0.0.0",
                  "resources": [],
                  "parameters": {
                    "atm": {
                      "type": "string"
                    }
                  }
                }
                """;

            public const string BicepRegistryModule1ArtifactId = "br:mockregistry.io/test/module1:v1";
            public const string BicepRegistryModule1Text = """
                param brm1 string
                """;

            public const string BicepRegistryModule2ArtifactId = "br:mockregistry.io/test/module2:v2";
            public const string BicepRegistryModule2Text = """
                param brm2 string
                """;

            public const string TemplateSpecId = "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/test-ts/versions/v1";
            public const string TemplateSpecArtifactId = "ts:00000000-0000-0000-0000-000000000000/test-rg/test-ts:v1";
            public const string TemplateSpecModuleText = $$"""
                {BicepLocalModule2Text
                  "id": "{{TemplateSpecId}}",
                  "properties": {
                    "mainTemplate": {
                      "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                      "contentVersion": "1.0.0.0",
                      "resources": [],
                      "parameters": {
                        "tsm": {
                          "type": "string"
                        }
                      }
                    }
                  }
                }
                """;
        }
    }
}

