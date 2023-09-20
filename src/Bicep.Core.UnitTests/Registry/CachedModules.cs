// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Registry;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Registry;


public static class CachedModules
{
    // Get all cached modules from the local on-disk registry cache
    public static ImmutableArray<CachedModule> GetCachedRegistryModules(string cacheRootDirectory)
    {
        var cacheDir = new DirectoryInfo(cacheRootDirectory);
        if (!cacheDir.Exists)
        {
            return ImmutableArray<CachedModule>.Empty;
        }

        // we create the "br" folder with same casing on all file systems
        var brDir = cacheDir.EnumerateDirectories().SingleOrDefault(dir => string.Equals(dir.Name, "br"));

        // the directory structure is .../br/<registry>/<repository>/<tag>
        var moduleDirectories = brDir?
            .EnumerateDirectories()
            .SelectMany(registryDir => registryDir.EnumerateDirectories())
            .SelectMany(repoDir => repoDir.EnumerateDirectories());

        return moduleDirectories?
            .Select(moduleDirectory => new CachedModule(
                moduleDirectory.FullName,
                UnobfuscateFolderName(moduleDirectory.Parent!.Parent!.Name),
                UnobfuscateFolderName(moduleDirectory.Parent!.Name),
                UnobfuscateFolderName(moduleDirectory.Name)))
            .ToImmutableArray()
        ?? ImmutableArray<CachedModule>.Empty;
    }

    private static string UnobfuscateFolderName(string folderName)
    {
        return folderName.Replace("$", "/").TrimEnd('/');
    }
}
