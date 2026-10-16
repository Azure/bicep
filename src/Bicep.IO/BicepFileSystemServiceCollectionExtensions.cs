// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepFileSystemServiceCollectionExtensions
{
    /// <summary>
    /// Registers the local file system and a <see cref="IFileExplorer"/> backed by it, unless already registered.
    /// </summary>
    public static IServiceCollection AddBicepFileSystem(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileSystem, LocalFileSystem>();
        services.TryAddSingleton<IFileExplorer, FileSystemFileExplorer>();

        return services;
    }
}
