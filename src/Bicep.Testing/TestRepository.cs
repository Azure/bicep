// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Testing;

public static class TestRepository
{
    private static readonly string RootPath = FindRootPath();

    public static string GetAbsolutePath(string path) => Path.GetFullPath(path, RootPath);

    private static string FindRootPath()
    {
        var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        while (currentDirectory.Parent is { } parentDirectory)
        {
            if (Directory.Exists(Path.Join(currentDirectory.FullName, ".git")))
            {
                return Environment.GetEnvironmentVariable("TF_BUILD") is not null
                    ? Path.Join(currentDirectory.FullName, "bicep")
                    : currentDirectory.FullName;
            }

            currentDirectory = parentDirectory;
        }

        throw new InvalidOperationException($"Unable to determine the repository root path from directory {Environment.CurrentDirectory}");
    }
}
