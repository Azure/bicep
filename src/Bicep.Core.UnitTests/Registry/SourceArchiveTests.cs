// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Formats.Tar;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceArchiveTests
{
    public TestContext? TestContext { get; set; }

    private const string MainDotBicepSource = @"
        targetScope = 'subscription'
        // Module description
        metadata description = 'fake main bicep file'";

    private const string MainDotJsonSource = @"{
        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
        ""contentVersion"": ""1.0.0.0"",
        ""resources"": {
        // Some people like this formatting
        },
        ""parameters"": {
        ""objectParameter"": {
            ""type"": ""object""
        }
        }
    }";

    private const string SecondaryDotBicepSource = @"
        // Module description
        metadata description = 'fake secondary bicep file'
    ";

    private const string StandaloneJsonSource = @"{
        // This file is a module that was referenced directly via JSON
        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
        ""contentVersion"": ""1.0.0.0"",
        ""resources"": [],
        ""parameters"": {
            ""secureStringParam"": {
                ""type"": ""securestring""
            }
        }
    }";

    private const string TemplateSpecJsonSource = @"{
            // Template spec
            ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
            ""contentVersion"": ""1.0.0.0"",
            ""metadata"": {
                ""_generator"": {
                    ""name"": ""bicep"",
                    ""version"": ""0.17.1.54307"",
                    ""templateHash"": ""3268788020119860428""
                },
                ""description"": ""my template spec description storagespec v2""
            },
            ""parameters"": {
                ""storageAccountType"": {
                    ""type"": ""string"",
                    ""defaultValue"": ""Standard_LRS"",
                    ""allowedValues"": [
                        ""Standard_LRS"",
                        ""Standard_GRS"",
                        ""Standard_ZRS"",
                        ""Premium_LRS""
                    ]
                },
                ""loc"": {
                    ""type"": ""string"",
                    ""defaultValue"": ""[resourceGroup().location]""
                }
            },
            ""variables"": {
                ""prefix"": ""mytest""
            },
            ""resources"": [
                {
                    ""type"": ""Microsoft.Storage/storageAccounts"",
                    ""apiVersion"": ""2021-04-01"",
                    ""name"": ""[format('{0}{1}', variables('prefix'), uniqueString(resourceGroup().id))]"",
                    ""location"": ""[parameters('loc')]"",
                    ""sku"": {
                        ""name"": ""[parameters('storageAccountType')]""
                    },
                    ""kind"": ""StorageV2""
                }
            ]
        }";

    private const string LocalModuleDotJsonSource = @"{
        // localModule.json
        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
        ""contentVersion"": ""1.0.0.0"",
        ""resources"": [],
        ""parameters"": {
            ""stringParam"": {
                ""type"": ""string""
            }
        }
    }";

    private ISourceFile CreateSourceFile(MockFileSystem fs, Uri projectFolderUri, string relativePath, string sourceKind, string content)
    {
        projectFolderUri.AbsolutePath.Should().EndWith("/");
        Uri uri = new Uri(projectFolderUri, relativePath);
        fs.AddFile(uri.LocalPath, content);
        string actualContents = fs.File.ReadAllText(uri.LocalPath);
        return sourceKind switch
        {
            SourceArchive.SourceKind_ArmTemplate => SourceFileFactory.CreateArmTemplateFile(uri, actualContents),
            SourceArchive.SourceKind_Bicep => SourceFileFactory.CreateSourceFile(uri, actualContents),
            SourceArchive.SourceKind_TemplateSpec => SourceFileFactory.CreateTemplateSpecFile(uri, actualContents),
            _ => throw new Exception($"Unrecognized source kind: {sourceKind}")
        }; ;
    }

    [TestMethod]
    public void CanPackAndUnpackSourceFiles()
    {
        Uri projectFolder = new Uri("file:///my project/my sources/", UriKind.Absolute);
        var fs = new MockFileSystem();
        fs.AddDirectory(projectFolder.LocalPath);

        var mainBicep = CreateSourceFile(fs, projectFolder, "main.bicep", SourceArchive.SourceKind_Bicep, MainDotBicepSource);
        var mainJson = CreateSourceFile(fs, projectFolder, "main.json", SourceArchive.SourceKind_ArmTemplate, MainDotJsonSource);
        var standaloneJson = CreateSourceFile(fs, projectFolder, "standalone.json", SourceArchive.SourceKind_ArmTemplate, StandaloneJsonSource);
        var templateSpecMainJson = CreateSourceFile(fs, projectFolder, "Main template.json", SourceArchive.SourceKind_TemplateSpec, TemplateSpecJsonSource);
        var localModuleJson = CreateSourceFile(fs, projectFolder, "localModule.json", SourceArchive.SourceKind_ArmTemplate, LocalModuleDotJsonSource);

        using var stream = SourceArchive.PackSourcesIntoStream(mainBicep.FileUri, mainBicep, mainJson, standaloneJson, templateSpecMainJson, localModuleJson);
        stream.Length.Should().BeGreaterThan(0);

        SourceArchive sourceArchive = SourceArchive.FromStream(stream);
        sourceArchive.EntrypointPath.Should().Be("main.bicep");


        var archivedFiles = sourceArchive.SourceFiles.ToArray();
        archivedFiles.Should().BeEquivalentTo(
            new SourceArchive.SourceFileInfo[] {
                new ("main.bicep", "main.bicep", SourceArchive.SourceKind_Bicep, MainDotBicepSource),
                new ("main.json", "main.json", SourceArchive.SourceKind_ArmTemplate, MainDotJsonSource ),
                new ("standalone.json", "standalone.json", SourceArchive.SourceKind_ArmTemplate, StandaloneJsonSource),
                new ("Main template.json", "Main template.json", SourceArchive.SourceKind_TemplateSpec,  TemplateSpecJsonSource),
                new ("localModule.json", "localModule.json", SourceArchive.SourceKind_ArmTemplate,  LocalModuleDotJsonSource),
            });
    }

    [DataTestMethod]
    [DataRow(
        "my other.bicep",
        "my other.bicep",
        DisplayName = "HandlesPathsCorrectly: spaces")]
    [DataRow(
        "/my root/my project/sub folder/my other bicep.bicep",
        "sub folder/my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: subfolder")]
    [DataRow(
        "/my root/my project/sub folder/sub folder 2/my other bicep.bicep",
        "sub folder/sub folder 2/my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: sub-subfolder")]
    [DataRow(
        "/my root/my other bicep.bicep",
        "../my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: ..")]
    [DataRow(
        "/my other bicep.bicep",
        "../../my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: ../..")]
    [DataRow(
        "/folder/my other bicep.bicep",
        "../../folder/my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: ../../folder")]
    [DataRow(
        "/my root/my project/my other bicep.bicep",
        "my other bicep.bicep",
        DisplayName = "HandlesPathsCorrectly: separate drives")]
    public void HandlesPathsCorrectly(
        string pathRelativeToMainBicepLocation,
        string expecteArchivedUri,
        string? expecteArchivePath = null)
    {
        string mainBicepPath = MockUnixSupport.Path("c:/my root/my project/my main.bicep");
        expecteArchivePath ??= expecteArchivedUri;

        Uri entrypointUri = DocumentUri.FromFileSystemPath(mainBicepPath).ToUriEncoded();
        var fs = new MockFileSystem();

        var mainBicepFolder = new Uri(Path.GetDirectoryName(mainBicepPath)! + "/", UriKind.Absolute);
        fs.AddDirectory(mainBicepFolder.LocalPath);

        var mainBicep = CreateSourceFile(fs, mainBicepFolder, Path.GetFileName(mainBicepPath), SourceArchive.SourceKind_Bicep, MainDotBicepSource);
        var testFile = CreateSourceFile(fs, mainBicepFolder, pathRelativeToMainBicepLocation, SourceArchive.SourceKind_Bicep, SecondaryDotBicepSource);

        using var stream = SourceArchive.PackSourcesIntoStream(mainBicep.FileUri, mainBicep, testFile);

        SourceArchive sourceArchive = SourceArchive.FromStream(stream);

        sourceArchive.EntrypointPath.Should().Be("my main.bicep");

        var archivedTestFile = sourceArchive.SourceFiles.Single(f => f.Path != "my main.bicep");
        archivedTestFile.Path.Should().Be(expecteArchivedUri);
        archivedTestFile.ArchivePath.Should().Be(expecteArchivePath);
        archivedTestFile.Contents.Should().Be(SecondaryDotBicepSource);
    }

    [TestMethod]
    public void GetSourceFiles_ForwardsCompat_ShouldIgnoreUnrecognizedPropertiesInMetadata()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""file:///main.bicep"",
                  ""I am an unrecognized property name"": {},
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""main.bicep"",
                      ""kind"": ""bicep"",
                      ""I am also recognition challenged"": ""Hi, Mom!""
                    }
                  ]
                }"
            ),
            (
                "main.bicep",
                @"bicep contents"
            )
        );

        var sut = SourceArchive.FromStream(zip);
        var file = sut.SourceFiles.Single();

        file.Kind.Should().Be("bicep");
        file.Contents.Should().Be("bicep contents");
        file.Path.Should().Contain("main.bicep");
    }

    [TestMethod]
    public void GetSourceFiles_BackwardsCompat_ShouldBeAbleToReadOldFormats()
    {
        // DO NOT ADD TO THIS DATA - IT IS MEANT TO TEST READING
        // OLD FILE VERSIONS WITH MINIMAL DATA
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""file:///main.bicep"",
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""main.bicep"",
                      ""kind"": ""bicep""
                    }
                  ]
                }"
            ),
            (
                "main.bicep",
                @"bicep contents"
            )
        );

        var sut = SourceArchive.FromStream(zip);
        var file = sut.SourceFiles.Single();

        file.Kind.Should().Be("bicep");
        file.Contents.Should().Be("bicep contents");
        file.Path.Should().Contain("main.bicep");
    }

    [TestMethod]
    public void GetSourceFiles_ForwardsCompat_ShouldIgnoreFileEntriesNotInMetadata()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""file:///main.bicep"",
                  ""I am an unrecognized property name"": {},
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""main.bicep"",
                      ""kind"": ""bicep"",
                      ""I am also recognition challenged"": ""Hi, Mom!""
                    }
                  ]
                }"
            ),
            (
                "I'm not mentioned in metadata.bicep",
                @"unmentioned contents"
            ),
            (
                "main.bicep",
                @"bicep contents"
            )
        );

        var sut = SourceArchive.FromStream(zip);
        var file = sut.SourceFiles.Single();

        file.Kind.Should().Be("bicep");
        file.Contents.Should().Be("bicep contents");
        file.Path.Should().Contain("main.bicep");
    }

    private Stream CreateGzippedTarredFileStream(params (string relativePath, string contents)[] files)
    {
        var outFolder = FileHelper.GetUniqueTestOutputPath(TestContext!);
        var ms = new MemoryStream();
        using (var gz = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true))
        {
            using (var tarWriter = new TarWriter(gz, leaveOpen: true))
            {
                foreach (var (relativePath, contents) in files)
                {
                    // Intentionally creating the archive differently than SourceArchive does it.
                    Directory.CreateDirectory(outFolder);
                    var fileName = Path.Join(outFolder, new Guid().ToString());
                    File.WriteAllText(fileName, contents, Encoding.UTF8);
                    tarWriter.WriteEntry(fileName, relativePath);
                }
            }
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}
