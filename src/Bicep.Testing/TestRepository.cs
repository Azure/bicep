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
            // Identify the Bicep repository itself using the solution file rather than relying on .git,
            // which can identify either the Bicep submodule or its containing BicepMirror repository in ADO.
            if (File.Exists(Path.Join(currentDirectory.FullName, "Bicep.sln")) ||
                File.Exists(Path.Join(currentDirectory.FullName, "Bicep.slnx")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = parentDirectory;
        }

        throw new InvalidOperationException($"Unable to determine the repository root path from directory {Environment.CurrentDirectory}");
    }
}
