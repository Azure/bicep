// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.SourceCode.SourceArchive;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceArchiveTests
{
    public TestContext? TestContext { get; set; }

    string CacheRoot = $"{ROOT}Users/username/.bicep";

#if WINDOWS_BUILD
    private const string ROOT = "c:\\";
#else
    private const string ROOT = "/";
#endif

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

    private const string ExternalModuleDotJsonSource = @"{
        // external module json
        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
        ""contentVersion"": ""1.0.0.0"",
        ""resources"": [],
        ""parameters"": {
            ""stringParam"": {
                ""type"": ""string""
            }
        }
    }";

    private SourceFileWithArtifactReference CreateSourceFile(MockFileSystem fs, string path, string sourceKind, string content, string? artifactReference = null)
    {
        return CreateSourceFile(fs, null, path, sourceKind, content, artifactReference);
    }

    private SourceFileWithArtifactReference CreateSourceFile(MockFileSystem fs, Uri? projectFolderUri, string relativePath, string sourceKind, string content, string? artifactReferenceId = null)
    {
        var artifactReference = artifactReferenceId is null ? null : OciRegistryHelper.ParseModuleReference(artifactReferenceId);
        projectFolderUri?.AbsolutePath.Should().EndWith("/");
        Uri uri = projectFolderUri is null ? PathHelper.FilePathToFileUrl(relativePath) : PathHelper.FilePathToFileUrl(Path.Combine(projectFolderUri.LocalPath, relativePath));
        fs.AddFile(uri.LocalPath, content);
        string actualContents = fs.File.ReadAllText(uri.LocalPath);

        return new SourceFileWithArtifactReference(
            sourceKind switch
            {
                SourceArchive.SourceKind.ArmTemplate => SourceFileFactory.CreateArmTemplateFile(uri, actualContents),
                SourceArchive.SourceKind.Bicep => SourceFileFactory.CreateSourceFile(uri, actualContents),
                SourceArchive.SourceKind.TemplateSpec => SourceFileFactory.CreateTemplateSpecFile(uri, actualContents),
                _ => throw new Exception($"Unrecognized source kind: {sourceKind}")
            },
            artifactReference);
    }

    [TestMethod]
    public void CanPackAndUnpackSourceFiles()
    {
        Uri projectFolder = PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/");
        var fs = new MockFileSystem();
        fs.AddDirectory(projectFolder.LocalPath);

        var mainBicep = CreateSourceFile(fs, projectFolder, "main.bicep", SourceArchive.SourceKind.Bicep, MainDotBicepSource);
        var mainJson = CreateSourceFile(fs, projectFolder, "main.json", SourceArchive.SourceKind.ArmTemplate, MainDotJsonSource);
        var standaloneJson = CreateSourceFile(fs, projectFolder, "standalone.json", SourceArchive.SourceKind.ArmTemplate, StandaloneJsonSource);
        var templateSpecMainJson = CreateSourceFile(fs, projectFolder, "Template spec 1.json", SourceArchive.SourceKind.TemplateSpec, TemplateSpecJsonSource);
        var localModuleJson = CreateSourceFile(fs, projectFolder, "localModule.json", SourceArchive.SourceKind.ArmTemplate, LocalModuleDotJsonSource);
        var templateSpecMainJson2 = CreateSourceFile(fs, projectFolder, "folder/template spec 2.json", SourceArchive.SourceKind.TemplateSpec, TemplateSpecJsonSource);
        var externalModuleJson = CreateSourceFile(fs, $"{CacheRoot}/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
                SourceArchive.SourceKind.ArmTemplate /* the actual source archived is the compiled JSON */, ExternalModuleDotJsonSource, "mcr.microsoft.com/bicep/storage/storage-account:1.0.1");


        using var stream = SourceArchive.PackSourcesIntoStream(
            mainBicep.SourceFile.FileUri,
            CacheRoot, mainBicep, mainJson, standaloneJson, templateSpecMainJson, localModuleJson, templateSpecMainJson2, externalModuleJson);
        stream.Length.Should().BeGreaterThan(0);

        SourceArchive? sourceArchive = SourceArchive.UnpackFromStream(stream).UnwrapOrThrow();
        sourceArchive!.EntrypointRelativePath.Should().Be("main.bicep");


        var archivedFiles = sourceArchive.SourceFiles.ToArray();
        archivedFiles.Should().BeEquivalentTo(
            new SourceArchive.SourceFileInfo[] {
                // Note: the template spec files will be filtered out
                new ("main.bicep", "files/main.bicep", SourceArchive.SourceKind.Bicep, MainDotBicepSource, null),
                new ("main.json", "files/main.json", SourceArchive.SourceKind.ArmTemplate, MainDotJsonSource, null),
                new ("standalone.json", "files/standalone.json", SourceArchive.SourceKind.ArmTemplate, StandaloneJsonSource, null),
                new ("localModule.json", "files/localModule.json", SourceArchive.SourceKind.ArmTemplate,  LocalModuleDotJsonSource, null),
                new ("<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json", "files/_cache_/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
                    SourceArchive.SourceKind.ArmTemplate, ExternalModuleDotJsonSource, OciRegistryHelper.ParseModuleReference("br:mcr.microsoft.com/bicep/storage/storage-account:1.0.1")),
            });
    }

    [TestMethod]
    public void CanPackAndUnpackDocumentLinks()
    {
        Uri projectFolder = PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/");
        var fs = new MockFileSystem();
        fs.AddDirectory(projectFolder.LocalPath);
        var mainBicep = CreateSourceFile(fs, projectFolder, "main&.bicep", SourceArchive.SourceKind.Bicep, MainDotBicepSource);
        var mainJson = CreateSourceFile(fs, projectFolder, "main.json", SourceArchive.SourceKind.ArmTemplate, MainDotJsonSource);
        var standaloneJson = CreateSourceFile(fs, projectFolder, "standalone.json", SourceArchive.SourceKind.ArmTemplate, StandaloneJsonSource);
        var templateSpecMainJson = CreateSourceFile(fs, projectFolder, "cache/wherever/template spec 1.json", SourceArchive.SourceKind.TemplateSpec, TemplateSpecJsonSource);
        var localModuleJson = CreateSourceFile(fs, projectFolder, "modules/localJsonModule.json", SourceArchive.SourceKind.ArmTemplate, LocalModuleDotJsonSource);
        var localModuleBicep = CreateSourceFile(fs, projectFolder, "modules/localBicepModule.bicep", SourceArchive.SourceKind.ArmTemplate, LocalModuleDotJsonSource);
        var externalModuleJson = CreateSourceFile(fs, $"{CacheRoot}/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
            SourceArchive.SourceKind.ArmTemplate, ExternalModuleDotJsonSource, "mcr.microsoft.com/bicep/storage/storage-account:1.0.1");

        var linksInput = new Dictionary<Uri, SourceCodeDocumentUriLink[]>()
        {
            {
                PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main&.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(1, 2, 1, 3), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/modules/localJsonModule.json")),
                    new(new SourceCodeRange(11, 2, 11, 3), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/modules/localBicepModule.bicep")),
                    new(new SourceCodeRange(12, 45, 23, 56), PathHelper.FilePathToFileUrl($"{CacheRoot}/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json")),
                }
            },
            {
                PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/modules/localBicepModule.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(123, 124, 234, 235), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main&.bicep")),
                    new(new SourceCodeRange(234, 235, 345, 346), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main.json")),
                    new(new SourceCodeRange(123, 456, 234, 567), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main&.bicep")),
                    new(new SourceCodeRange(1234, 4567, 2345, 5678), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/wherever/template spec 1.json")),
                    new(new SourceCodeRange(345, 2, 345, 3), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/modules/localJsonModule.json")),
                    new(new SourceCodeRange(12, 45, 23, 56), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/wherever/template spec 1.json")),
                    new(new SourceCodeRange(12, 45, 23, 56), PathHelper.FilePathToFileUrl($"{CacheRoot}/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json")),
                }
            },
            {
                // Shouldn't be possible to have a template spec file as the source, but still test that it's filtered out
                PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/wherever/template spec 1.json"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(123, 124, 234, 235), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main&.bicep")),
                    new(new SourceCodeRange(234, 235, 345, 346), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main.json")),
                    new(new SourceCodeRange(123, 456, 234, 567), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/main&.bicep")),
                    new(new SourceCodeRange(1234, 4567, 2345, 5678), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/cache/wherever/template spec 1.json")),
                    new(new SourceCodeRange(345, 2, 345, 3), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/modules/localJsonModule.json")),
                    new(new SourceCodeRange(12, 45, 23, 56), PathHelper.FilePathToFileUrl($"{ROOT}my project/my sources/cache/wherever/template spec 1.json")),
                    new(new SourceCodeRange(12, 45, 23, 56), PathHelper.FilePathToFileUrl($"{CacheRoot}/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json")),
                }
            },
        };
        using var stream = SourceArchive.PackSourcesIntoStream(mainBicep.SourceFile.FileUri, CacheRoot, linksInput, mainBicep, mainJson, standaloneJson, templateSpecMainJson, localModuleJson, localModuleBicep, externalModuleJson);
        stream.Length.Should().BeGreaterThan(0);

        SourceArchive? sourceArchive = SourceArchive.UnpackFromStream(stream).TryUnwrap();
        sourceArchive.Should().NotBeNull();

        var archivedLinks = sourceArchive!.DocumentLinks;

        var expected = new Dictionary<string, SourceCodeDocumentPathLink[]>()
        {
            // Note: the template spec files will be filtered out
            {
                "main&.bicep",
                new SourceCodeDocumentPathLink[]
                {
                    new(new SourceCodeRange(1, 2, 1, 3), "modules/localJsonModule.json"),
                    new(new SourceCodeRange(11, 2, 11, 3), "modules/localBicepModule.bicep"),
                    new(new SourceCodeRange(12, 45, 23, 56), "<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json"),
                }
            },
            {
                "modules/localBicepModule.bicep",
                new SourceCodeDocumentPathLink[]
                {
                    new(new SourceCodeRange(123, 124, 234, 235), "main&.bicep"),
                    new(new SourceCodeRange(234, 235, 345, 346), "main.json"),
                    new(new SourceCodeRange(123, 456, 234, 567), "main&.bicep"),
                    new(new SourceCodeRange(345, 2, 345, 3), "modules/localJsonModule.json"),
                    new(new SourceCodeRange(12, 45, 23, 56), "<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json"),
                }
            },
        };

        archivedLinks.Should().BeEquivalentTo(expected);
    }

    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main.bicep", $"{ROOT}my other.bicep" },
        new string[] { "my root/my project/my main.bicep", "my other.bicep" },
        new string[] { "files/my root/my project/my main.bicep", "files/my other.bicep" },
        DisplayName = "HandlesPathsCorrectly: spaces")]
#if WINDOWS_BUILD
    [DataRow(
        new string[] { $"{ROOT}my root\\my project\\my main.bicep", $"{ROOT}my other.bicep" },
        new string[] { "my root/my project/my main.bicep", "my other.bicep" },
        new string[] { "files/my root/my project/my main.bicep", "files/my other.bicep" },
        DisplayName = "HandlesPathsCorrectly: backslashes")]
#endif
    [DataRow(
        new string[] { $"{ROOT}my#project/sub#folder/my#main.bicep", $"{ROOT}my#project/my#main.bicep" },
        new string[] { "sub#folder/my#main.bicep", "my#main.bicep" },
        new string[] { "files/sub_folder/my_main.bicep", "files/my_main.bicep" },
        DisplayName = "HandlesPathsCorrectly: #")]
    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main.bicep", $"{ROOT}my root/my project/sub folder/my other bicep.bicep" },
        new string[] { "my main.bicep", "sub folder/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/sub folder/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: subfolder")]
    [DataRow(
        new string[] { $"{ROOT}my main.bicep", $"{ROOT}sub folder/my other bicep.bicep" },
        new string[] { "my main.bicep", "sub folder/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/sub folder/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: in root")]
    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main.bicep", $"{ROOT}my root/my project/sub folder/sub folder 2/my other bicep.bicep" },
        new string[] { "my main.bicep", "sub folder/sub folder 2/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/sub folder/sub folder 2/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: sub-subfolder")]
    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main.bicep", $"{ROOT}my root/my other bicep.bicep" },
        new string[] { "my project/my main.bicep", "my other bicep.bicep" },
        new string[] { "files/my project/my main.bicep", "files/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: main.bicep in subfolder")]
    [DataRow(
        new string[] { $"{ROOT}my root/folder1/folder2/my main.bicep", $"{ROOT}my root/my other bicep.bicep" },
        new string[] { "folder1/folder2/my main.bicep", "my other bicep.bicep" },
        new string[] { "files/folder1/folder2/my main.bicep", "files/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: main.bicep in sub-subfolder")]
    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main.bicep", $"{ROOT}my root/my other project/my other bicep.bicep" },
        new string[] { "my main.bicep", "<root2>/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/_root2_/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: main.bicep in different folder")]
    [DataRow(
        new string[] { $"{ROOT}folder1/my project/my main.bicep", $"{ROOT}folder2/my other project/my other bicep.bicep", $"{ROOT}folder2/my other project 2/my other bicep.bicep" },
        new string[] { "my main.bicep", "<root2>/my other bicep.bicep", "<root3>/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/_root2_/my other bicep.bicep", "files/_root3_/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: 3 roots")]
    [DataRow(
        new string[] { $"{ROOT}repos/bicep/deployment/my main.bicep", $"{ROOT}repos/bicep/deployment/subfolder1/module1.bicep", $"{ROOT}repos/bicep/deployment/subfolder2/module2.bicep" },
        new string[] { "my main.bicep", "subfolder1/module1.bicep", "subfolder2/module2.bicep" },
        new string[] { "files/my main.bicep", "files/subfolder1/module1.bicep", "files/subfolder2/module2.bicep" },
        DisplayName = "HandlesPathsCorrectly: Two subfolders")]
    [DataRow(
        new string[] { $"{ROOT}repos/bicep/my main.bicep", $"{ROOT}repos/bicep/..deployment../subfolder1/..module1..bicep" },
        new string[] { "my main.bicep", "..deployment../subfolder1/..module1..bicep" },
        new string[] { "files/my main.bicep", "files/..deployment../subfolder1/..module1..bicep" },
        DisplayName = "HandlesPathsCorrectly: .. in names")]
    [DataRow(
        new string[] { $"{ROOT}my root/my project/my main bicep.bicep", $"{ROOT}my other root/my project/my other bicep.bicep" },
        new string[] { "my main bicep.bicep", "<root2>/my other bicep.bicep" },
        new string[] { "files/my main bicep.bicep", "files/_root2_/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: no folders in common")]
#if WINDOWS_BUILD
    [DataRow(
        // This shouldn't ever happen, with the exception of when the cache root path is on another drive, because local module
        // files must be relative to the referencing file.
        new string[] { "c:\\my root\\my project\\my main.bicep", "d:\\my root\\my project\\my other bicep.bicep" },
        new string[] { "my main.bicep", "<root2>/my other bicep.bicep" },
        new string[] { "files/my main.bicep", "files/_root2_/my other bicep.bicep" },
        DisplayName = "HandlesPathsCorrectly: separate drives on Windows")]
    [DataRow(
        new string[] { "c:\\my ROOT\\my project\\my main.bicep", "C:\\My root\\My project\\My main 2.bicep", },
        new string[] { "my main.bicep", "My main 2.bicep" },
        new string[] { "files/my main.bicep", "files/My main 2.bicep" },
        DisplayName = "HandlesPathsCorrectly: case insensitive on Windows 2")]
    [DataRow(
        new string[] { "c:\\my ROOT\\my Project\\sub Folder\\my main.bicep", "C:\\My root\\My project\\My main 2.bicep", "C:\\My root\\My project 2\\My main 2.bicep", },
        new string[] { "sub Folder/my main.bicep", "My main 2.bicep", "<root2>/My main 2.bicep" },
        new string[] { "files/sub Folder/my main.bicep", "files/My main 2.bicep", "files/_root2_/My main 2.bicep" },
        DisplayName = "HandlesPathsCorrectly: case insensitive on Windows 3")]
    [DataRow(
        new string[] { "c:\\Deployment/a.txt", "c:\\deployment/b&.txt", "c:\\Deployment/B_.txt", },
        new string[] { "a.txt", "b&.txt", "B_.txt" },
        new string[] { "files/a.txt", "files/b_.txt", "files/B_.txt(2)" },
        DisplayName = "HandlesPathsCorrectly: case insensitive on Windows 4")]
#endif
    [DataRow(
        new string[] {
            $"{ROOT}my root/my project/my main.bicep",
            $"{ROOT}Users/username/.bicep/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json" },
        new string[] { "my main.bicep", "<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json" },
        new string[] { "files/my main.bicep", "files/_cache_/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json" },
        DisplayName = "HandlesPathsCorrectly: external module (in cache)")]
    [DataRow(
        new string[] {
            $"{ROOT}my root/my project/my main.bicep",
            $"{ROOT}Users/username/.bicep/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
            $"{ROOT}Users/username/.bicep/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.2$/main.json",
            $"{ROOT}Users/username/.bicep/br/mcr.microsoft.com/bicep$compute$virtual-machine/1.0.1$/main.json" },
        new string[] {
            "my main.bicep",
            "<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
            "<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.2$/main.json",
            "<cache>/br/mcr.microsoft.com/bicep$compute$virtual-machine/1.0.1$/main.json",
        },
        new string[] {
            "files/my main.bicep",
            "files/_cache_/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json",
            "files/_cache_/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.2$/main.json",
            "files/_cache_/br/mcr.microsoft.com/bicep$compute$virtual-machine/1.0.1$/main.json",
        },
        DisplayName = "HandlesPathsCorrectly: multiple external modules (in cache)")]
    [DataRow(
        new string[] { $"{ROOT}my & root/my&main.bicep", $"{ROOT}my & root/my [&] mainProject/my &[] main.bicep" },
        new string[] { "my&main.bicep", "my [&] mainProject/my &[] main.bicep" },
        new string[] { "files/my_main.bicep", "files/my ___ mainProject/my ___ main.bicep" },
        DisplayName = "HandlesPathsCorrectly:  Characters to avoid")]
    [DataRow(
        new string[] { $"{ROOT}ποταμός/Fluß im Regen.bicep", $"{ROOT}my other root/אמא שלי/אבא שלי.bicep" },
        new string[] { "Fluß im Regen.bicep", "<root2>/אבא שלי.bicep" },
        new string[] { "files/Fluß im Regen.bicep", "files/_root2_/אבא שלי.bicep" },
        DisplayName = "HandlesPathsCorrectly:  International characters")]
    [DataTestMethod]
    public void HandlesPathsCorrectly(
        string[] inputPaths,
        string[] expectedPaths,
        string[] expectedArchivePaths
    )
    {
        var fs = new MockFileSystem();

        var entrypointPath = inputPaths[0];

        var entrypointFolder = Path.GetDirectoryName(entrypointPath)!;
        if (entrypointFolder[^1] != '/' && entrypointFolder[^1] != '\\')
        {
            entrypointFolder += Path.DirectorySeparatorChar;
        }
        var files = inputPaths.Select(path => CreateSourceFile(fs, path, SourceArchive.SourceKind.Bicep, $"// {path}")).ToArray();

        using var stream = SourceArchive.PackSourcesIntoStream(files[0].SourceFile.FileUri, CacheRoot, files);
        SourceArchive sourceArchive = SourceArchive.UnpackFromStream(stream).UnwrapOrThrow();

        sourceArchive.EntrypointRelativePath.Should().Be(expectedPaths[0], "entrypoint path should be correct");

        sourceArchive.EntrypointRelativePath.Should().NotContain("username", "shouldn't have username in source paths");
        foreach (var file in sourceArchive.SourceFiles)
        {
            file.Path.Should().NotContain("username", "shouldn't have username in source paths");
            file.ArchivePath.Should().NotContain("username", "shouldn't have username in source paths");
        }

        for (int i = 0; i < inputPaths.Length; ++i)
        {
            var archivedTestFile = sourceArchive.SourceFiles.Single(f => f.Contents.Equals(files[i].SourceFile.GetOriginalSource()));
            archivedTestFile.Path.Should().Be(expectedPaths[i]);
            archivedTestFile.ArchivePath.Should().Be(expectedArchivePaths[i]);
        }
    }

    [DataRow(
        $"{ROOT}my root/my project/my_.bicep", "my_.bicep", "files/my_.bicep",
        $"{ROOT}my root/my project/my&.bicep", "my&.bicep", "files/my_.bicep(2)",
        $"{ROOT}my root/my project/my[.bicep", "my[.bicep", "files/my_.bicep(3)",
        DisplayName = "DuplicateNamesAfterMunging_ShouldHaveSeparateEntries: &")]
    [DataRow(
        $"{ROOT}my root/my project/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/a.txt",
            "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/a.txt",
            "files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123__path_too_long__.txt",
        $"{ROOT}my root/my project/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/b.txt",
            "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789/b.txt",
            "files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123__path_too_long__.txt(2)",
        $"{ROOT}my root/my project/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 extra characters here that get truncated/a.txt",
            "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 extra characters here that get truncated/a.txt",
            "files/123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123__path_too_long__.txt(3)",
        DisplayName = "DuplicateNamesAfterMunging_ShouldHaveSeparateEntries: truncated")]
#if WINDOWS_BUILD
    [DataRow(
        "c:\\my root\\my.bicep", "my.bicep", "files/my.bicep",
        "d:\\my root\\my project\\parent\\m&.bicep", "<root2>/m&.bicep", "files/_root2_/m_.bicep",
        "d:\\my root\\my project/parent\\m[.bicep", "<root2>/m[.bicep", "files/_root2_/m_.bicep(2)",
        DisplayName = "DuplicateNamesAfterMunging_ShouldHaveSeparateEntries: different drives")]
#endif
    [DataTestMethod]
    public void DuplicateNamesAfterMunging_ShouldHaveSeparateEntries(
        string inputBicepPath1, string expectedPath1, string expectedArchivePath1, // 1st bicep path, plus its expected path and archive path
        string inputBicepPath2, string expectedPath2, string expectedArchivePath2, // 2st bicep path, plus its expected path and archive path
        string? inputBicepPath3 = null, string? expectedPath3 = null, string? expectedArchivePath3 = null  // 3rd bicep path, plus its expected path and archive path
    )
    {
        string entrypointPath = $"{ROOT}my root/my project/my entrypoint.bicep";
        var fs = new MockFileSystem();

        var rootBicepFolder = new Uri(ROOT);
        fs.AddDirectory(rootBicepFolder.LocalPath);

        var entrypointFile = CreateSourceFile(fs, rootBicepFolder, entrypointPath, SourceArchive.SourceKind.Bicep, MainDotBicepSource);
        var sutFile1 = CreateSourceFile(fs, rootBicepFolder, inputBicepPath1, SourceArchive.SourceKind.Bicep, SecondaryDotBicepSource);
        var sutFile2 = CreateSourceFile(fs, rootBicepFolder, inputBicepPath2, SourceArchive.SourceKind.Bicep, SecondaryDotBicepSource);
        var sutFile3 = inputBicepPath3 is null ? null : CreateSourceFile(fs, rootBicepFolder, inputBicepPath3, SourceArchive.SourceKind.Bicep, SecondaryDotBicepSource);

        using var stream = sutFile3 is null ?
            SourceArchive.PackSourcesIntoStream(entrypointFile.SourceFile.FileUri, CacheRoot, entrypointFile, sutFile1, sutFile2) :
            SourceArchive.PackSourcesIntoStream(entrypointFile.SourceFile.FileUri, CacheRoot, entrypointFile, sutFile1, sutFile2, sutFile3);

        SourceArchive sourceArchive = SourceArchive.UnpackFromStream(stream).UnwrapOrThrow();

        var archivedFile1 = sourceArchive.SourceFiles.SingleOrDefault(f => f.Path == expectedPath1);
        var archivedFile2 = sourceArchive.SourceFiles.SingleOrDefault(f => f.Path == expectedPath2);
        var archivedFile3 = sourceArchive.SourceFiles.SingleOrDefault(f => f.Path == expectedPath3);

        archivedFile1.Should().NotBeNull($"Couldn't find source file \"{inputBicepPath1}\" in archive");
        archivedFile2.Should().NotBeNull($"Couldn't find source file \"{inputBicepPath2}\" in archive");
        if (inputBicepPath3 is not null)
        {
            archivedFile3.Should().NotBeNull($"Couldn't find source file \"{inputBicepPath3}\" in archive");
        }

        archivedFile1!.Path.Should().Be(expectedPath1);
        archivedFile1.ArchivePath.Should().Be(expectedArchivePath1);

        archivedFile2!.Path.Should().Be(expectedPath2);
        archivedFile2.ArchivePath.Should().Be(expectedArchivePath2);

        if (inputBicepPath3 is not null)
        {
            archivedFile3!.Path.Should().Be(expectedPath3);
            archivedFile3.ArchivePath.Should().Be(expectedArchivePath3);
        }
    }

    [TestMethod]
    public void GetSourceFiles_ForwardsCompat_ShouldIgnoreUnrecognizedPropertiesInMetadata()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""metadataVersion"": 1,
                  ""entryPoint"": ""file:///main.bicep"",
                  ""I am an unrecognized property name"": {},
                  ""bicepVersion"": ""0.18.19"",
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""files/main.bicep"",
                      ""kind"": ""bicep"",
                      ""I am also recognition challenged"": ""Hi, Mom!""
                    }
                  ]
                }"
            ),
            (
                "files/main.bicep",
                @"bicep contents"
            )
        );

        var sut = SourceArchive.UnpackFromStream(zip).UnwrapOrThrow();
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
                  ""entryPoint"": ""main.bicep"",
                  ""bicepVersion"": ""0.1.2"",
                  ""metadataVersion"": 1,
                  ""sourceFiles"": [
                    {
                      ""path"": ""main.bicep"",
                      ""archivePath"": ""files/main.bicep"",
                      ""kind"": ""bicep""
                    }
                  ]
                }"
            ),
            (
                "files/main.bicep",
                "bicep contents"
            )
        );

        var sut = SourceArchive.UnpackFromStream(zip).UnwrapOrThrow();
        var file = sut.SourceFiles.Single();

        file.Kind.Should().Be("bicep");
        file.Contents.Should().Be("bicep contents");
        file.Path.Should().Be("main.bicep");
    }

    [TestMethod]
    public void GetSourceFiles_ForwardsCompat_ShouldIgnoreFileEntriesNotInMetadata()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""main.bicep"",
                  ""I am an unrecognized property name"": {},
                  ""sourceFiles"": [
                    {
                      ""path"": ""main.bicep"",
                      ""archivePath"": ""files/main.bicep"",
                      ""kind"": ""bicep"",
                      ""I am also recognition challenged"": ""Hi, Mom!""
                    }
                  ],
                  ""bicepVersion"": ""0.1.2"",
                  ""metadataVersion"": 1
                }"
            ),
            (
                "I'm not mentioned in metadata.bicep",
                @"unmentioned contents"
            ),
            (
                "files/Nor am I.bicep",
                @"unmentioned contents 2"
            ),
            (
                "files/main.bicep",
                @"bicep contents"
            )
        );

        var sut = SourceArchive.UnpackFromStream(zip).UnwrapOrThrow();
        var file = sut.SourceFiles.Single();

        file.Kind.Should().Be("bicep");
        file.Contents.Should().Be("bicep contents");
        file.Path.Should().Contain("main.bicep");
    }

    [TestMethod]
    public void GetSourceFiles_ShouldGiveError_ForIncompatibleOlderVersion()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""file:///main.bicep"",
                  ""metadataVersion"": <version>,
                  ""bicepVersion"": ""0.whatever.0"",
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""main.bicep"",
                      ""kind"": ""bicep""
                    }
                  ]
                }".Replace("<version>", (SourceArchive.CurrentMetadataVersion - 1).ToString())
            ),
            (
                "main.bicep",
                @"bicep contents"
            )
        );

        SourceArchive.UnpackFromStream(zip).IsSuccess(out var sourceArchive, out var ex);
        sourceArchive.Should().BeNull();
        ex.Should().NotBeNull();
        ex!.Message.Should().StartWith("This source code was published with an older, incompatible version of Bicep (0.whatever.0). You are using version ");
    }

    [TestMethod]
    public void GetSourceFiles_ShouldGiveError_ForIncompatibleNewerVersion()
    {
        var zip = CreateGzippedTarredFileStream(
            (
                "__metadata.json",
                @"
                {
                  ""entryPoint"": ""file:///main.bicep"",
                  ""metadataVersion"": <version>,
                  ""bicepVersion"": ""0.whatever.0"",
                  ""sourceFiles"": [
                    {
                      ""path"": ""file:///main.bicep"",
                      ""archivePath"": ""main.bicep"",
                      ""kind"": ""bicep""
                    }
                  ]
                }".Replace("<version>", (SourceArchive.CurrentMetadataVersion + 1).ToString())
            ),
            (
                "main.bicep",
                @"bicep contents"
            )
        );

        var success = SourceArchive.UnpackFromStream(zip).IsSuccess(out _, out var ex);
        success.Should().BeFalse();
        ex.Should().NotBeNull();
        ex!.Message.Should().StartWith("This source code was published with a newer, incompatible version of Bicep (0.whatever.0). You are using version ");
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
