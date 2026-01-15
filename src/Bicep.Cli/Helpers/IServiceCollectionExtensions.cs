// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Commands;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Environment = Bicep.Core.Utils.Environment;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Cli.Helpers;

/// <summary>
/// Contains the collection extensions for adding the CLI commands and configuration of those commands.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the CLI commands to the DI container. These are resolved when the commands are registered with the
    /// <c>CommandLineBuilder</c>.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// We are using convention to register the commands; essentially everything in the same namespace as the
    /// <see cref="BuildCommand"/> and that implements <see cref="ICommand"/> will be registered.
    ///
    /// See https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection for reference.
    /// </remarks>
    public static IServiceCollection AddCommands(this IServiceCollection services) =>
        // this is hardcoded to make the code trim-safe
        services
            .AddSingleton<BuildCommand>()
            .AddSingleton<TestCommand>()
            .AddSingleton<BuildParamsCommand>()
            .AddSingleton<DecompileCommand>()
            .AddSingleton<DecompileParamsCommand>()
            .AddSingleton<FormatCommand>()
            .AddSingleton<GenerateParametersFileCommand>()
            .AddSingleton<PublishCommand>()
            .AddSingleton<PublishExtensionCommand>()
            .AddSingleton<RestoreCommand>()
            .AddSingleton<LintCommand>()
            .AddSingleton<JsonRpcCommand>()
            .AddSingleton<LocalDeployCommand>()
            .AddSingleton<SnapshotCommand>()
            .AddSingleton<DeployCommand>()
            .AddSingleton<WhatIfCommand>()
            .AddSingleton<TeardownCommand>()
            .AddSingleton<ConsoleCommand>()
            .AddSingleton<RootCommand>();
}
