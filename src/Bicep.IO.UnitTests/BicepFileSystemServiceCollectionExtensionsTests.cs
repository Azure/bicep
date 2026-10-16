// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.IO.UnitTests;

[TestClass]
public class BicepFileSystemServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddBicepFileSystem_NoExistingRegistrations_RegistersLocalFileSystem()
    {
        using var provider = new ServiceCollection().AddBicepFileSystem().BuildServiceProvider();

        provider.GetRequiredService<IFileSystem>().Should().BeOfType<System.IO.Abstractions.FileSystem>();
        provider.GetRequiredService<IFileExplorer>().Should().BeOfType<FileSystemFileExplorer>();
    }

    [TestMethod]
    public void AddBicepFileSystem_FileSystemAlreadyRegistered_ExplorerUsesRegisteredFileSystem()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/test.txt", "contents");
        var services = new ServiceCollection().AddSingleton<IFileSystem>(fileSystem);

        using var provider = services.AddBicepFileSystem().BuildServiceProvider();

        provider.GetRequiredService<IFileExplorer>()
            .GetFile(IOUri.FromFilePath(fileSystem.Path.GetFullPath("/test.txt")))
            .ReadAllText().Should().Be("contents");
    }

    [TestMethod]
    public void AddBicepFileSystem_ServicesAlreadyRegistered_PreservesRegistrations()
    {
        var fileSystem = new MockFileSystem();
        var explorer = new InMemoryFileExplorer();
        var services = new ServiceCollection()
            .AddSingleton<IFileSystem>(fileSystem)
            .AddSingleton<IFileExplorer>(explorer);

        services.AddBicepFileSystem().AddBicepFileSystem();
        using var provider = services.BuildServiceProvider();

        services.Should().HaveCount(2);
        provider.GetRequiredService<IFileSystem>().Should().BeSameAs(fileSystem);
        provider.GetRequiredService<IFileExplorer>().Should().BeSameAs(explorer);
    }
}
