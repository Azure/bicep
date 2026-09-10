// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Core.Configuration;
using Bicep.Core.Documentation;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Services;

public class DocsGenerationOptionsResolver(
    InputOutputArgumentsResolver argumentsResolver,
    IFileSystem fileSystem)
{
    public BicepDocumentationGenerationOptions Resolve(
        IBicepConfiguration configuration,
        IReadOnlyDictionary<string, string> customValues)
    {
        var settings = configuration.Documentation.Data;

        return new(
            ResolveTemplateFile(configuration, settings.Template.File),
            ResolveTemplateRoot(configuration, settings.Template.IncludeRoot),
            customValues)
        {
            Examples = settings.Examples,
        };
    }

    private IOUri? ResolveTemplateFile(
        IBicepConfiguration configuration,
        string? configuredPath) =>
        configuredPath is not null
            ? argumentsResolver.PathToUri(ResolveConfiguredPath(configuration, configuredPath, "template.file"))
            : null;

    private IOUri? ResolveTemplateRoot(
        IBicepConfiguration configuration,
        string? configuredPath)
    {
        var fullPath = configuredPath is not null
            ? ResolveConfiguredPath(configuration, configuredPath, "template.includeRoot")
            : null;
        if (fullPath is null)
        {
            return null;
        }

        if (!fileSystem.Directory.Exists(fullPath))
        {
            throw new CommandLineException($"The template include root directory \"{fullPath}\" does not exist.");
        }

        return argumentsResolver
            .PathToUri(fileSystem.Path.Combine(fullPath, ".bicep-docs-root"))
            .Resolve(".");
    }

    private string ResolveConfiguredPath(
        IBicepConfiguration configuration,
        string configuredPath,
        string propertyName)
    {
        if (fileSystem.Path.IsPathRooted(configuredPath))
        {
            return argumentsResolver.GetFullPath(configuredPath);
        }

        if (configuration.ConfigFileUri is not { } configFileUri)
        {
            throw new CommandLineException(
                $"The documentation {propertyName} path \"{configuredPath}\" is relative, but no bicepconfig.json file was resolved.");
        }

        return argumentsResolver.GetFullPath(
            fileSystem.Path.Combine(configFileUri.Resolve(".").GetFilePath(), configuredPath));
    }
}
