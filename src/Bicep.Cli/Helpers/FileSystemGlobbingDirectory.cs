// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using DirectoryInfoBase = Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase;
using FileInfoBase = Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase;
using FileSystemInfoBase = Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase;

namespace Bicep.Cli.Helpers;

internal sealed class FileSystemGlobbingDirectory(IDirectoryInfo directory, bool isParentPath = false) : DirectoryInfoBase
{
    public override string Name => isParentPath ? ".." : directory.Name;

    public override string FullName => directory.FullName;

    public override DirectoryInfoBase? ParentDirectory =>
        directory.Parent is { } parent ? new FileSystemGlobbingDirectory(parent) : null;

    public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
    {
        if (!directory.Exists)
        {
            yield break;
        }

        foreach (var entry in directory.EnumerateFileSystemInfos())
        {
            yield return entry switch
            {
                IDirectoryInfo child => new FileSystemGlobbingDirectory(child),
                IFileInfo file => new GlobbingFile(file),
                _ => throw new InvalidOperationException($"Unsupported file system entry '{entry.FullName}'."),
            };
        }
    }

    public override DirectoryInfoBase? GetDirectory(string path)
    {
        if (path == "..")
        {
            var parent = directory.FileSystem.DirectoryInfo.New(directory.FileSystem.Path.Combine(directory.FullName, path));
            return new FileSystemGlobbingDirectory(parent, isParentPath: true);
        }

        return directory.GetDirectories(path) switch
        {
            [] => null,
            [var child] => new FileSystemGlobbingDirectory(child),
            _ => throw new InvalidOperationException($"Multiple directories match '{path}' under '{directory.FullName}'."),
        };
    }

    public override FileInfoBase GetFile(string path) =>
        new GlobbingFile(directory.FileSystem.FileInfo.New(directory.FileSystem.Path.Combine(directory.FullName, path)));

    private sealed class GlobbingFile(IFileInfo file) : FileInfoBase
    {
        public override string Name => file.Name;

        public override string FullName => file.FullName;

        public override DirectoryInfoBase? ParentDirectory =>
            file.Directory is { } parent ? new FileSystemGlobbingDirectory(parent) : null;
    }
}
