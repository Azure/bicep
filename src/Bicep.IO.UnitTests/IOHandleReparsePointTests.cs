// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;
using FluentAssertions;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.IO.UnitTests;

[TestClass]
public class IOHandleReparsePointTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IsReparsePoint_ReparsePointAttributeSet_ReturnsTrue(bool isDirectory)
    {
        var fileSystem = new MockFileSystem();
        var handle = CreateFileSystemHandle(fileSystem, isDirectory);
        var path = fileSystem.Path.TrimEndingDirectorySeparator(handle.Uri.GetFilePath());

        fileSystem.File.SetAttributes(path, fileSystem.File.GetAttributes(path) | FileAttributes.ReparsePoint);

        handle.IsReparsePoint().Should().BeTrue();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IsReparsePoint_RegularEntry_ReturnsFalse(bool isDirectory)
    {
        var handle = CreateFileSystemHandle(new MockFileSystem(), isDirectory);

        handle.IsReparsePoint().Should().BeFalse();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IsReparsePoint_EntryDoesNotExist_ReturnsFalse(bool isDirectory)
    {
        var fileSystem = new MockFileSystem();
        var explorer = new FileSystemFileExplorer(fileSystem);
        var uri = IOUri.FromFilePath(fileSystem.Path.GetFullPath("/missing/target"));
        IIOHandle handle = isDirectory ? explorer.GetDirectory(uri) : explorer.GetFile(uri);

        handle.IsReparsePoint().Should().BeFalse();
    }

    [TestMethod]
    public void IsReparsePoint_InMemoryHandles_ReturnsFalse()
    {
        var explorer = new InMemoryFileExplorer();
        var uri = IOUri.FromFilePath("/target");

        explorer.GetFile(uri).EnsureExists().IsReparsePoint().Should().BeFalse();
        explorer.GetDirectory(uri).EnsureExists().IsReparsePoint().Should().BeFalse();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IsReparsePoint_PhysicalSymbolicLink_ReturnsTrue(bool isDirectory)
    {
        var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(root);
        try
        {
            var target = Path.Combine(root, "target");
            var link = Path.Combine(root, "link");
            try
            {
                if (isDirectory)
                {
                    Directory.CreateDirectory(target);
                    Directory.CreateSymbolicLink(link, target);
                }
                else
                {
                    File.WriteAllText(target, "");
                    File.CreateSymbolicLink(link, target);
                }
            }
            catch (UnauthorizedAccessException) when (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Creating symbolic links requires Developer Mode or elevated privileges on Windows.");
            }

            var explorer = new FileSystemFileExplorer(new LocalFileSystem());
            var linkUri = IOUri.FromFilePath(link);
            var targetUri = IOUri.FromFilePath(target);
            IIOHandle linkHandle = isDirectory ? explorer.GetDirectory(linkUri) : explorer.GetFile(linkUri);
            IIOHandle targetHandle = isDirectory ? explorer.GetDirectory(targetUri) : explorer.GetFile(targetUri);

            linkHandle.IsReparsePoint().Should().BeTrue();
            targetHandle.IsReparsePoint().Should().BeFalse();
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static IIOHandle CreateFileSystemHandle(MockFileSystem fileSystem, bool isDirectory)
    {
        var explorer = new FileSystemFileExplorer(fileSystem);
        var uri = IOUri.FromFilePath(fileSystem.Path.GetFullPath("/target"));

        return isDirectory ? explorer.GetDirectory(uri).EnsureExists() : explorer.GetFile(uri).EnsureExists();
    }
}
