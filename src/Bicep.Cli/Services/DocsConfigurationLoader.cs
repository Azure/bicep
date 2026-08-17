// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Cli.Arguments;
using Bicep.Core.Documentation;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bicep.Cli.Services;

internal static class DocsConfigurationLoader
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    public static DocsConfigurationContext Load(
        string? path,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem)
    {
        if (path is null)
        {
            return new(new(), new(IOUriScheme.File, "", "/"));
        }

        var fullPath = resolver.GetFullPath(path);
        if (!fileSystem.File.Exists(fullPath))
        {
            throw new CommandLineException($"The docs configuration file \"{fullPath}\" does not exist.");
        }

        try
        {
            var contents = fileSystem.File.ReadAllText(fullPath);
            using var document = JsonDocument.Parse(contents, DocumentOptions);
            ValidateNoDuplicateProperties(document.RootElement, "$");
            var configuration = JsonSerializer.Deserialize(
                contents,
                DocsConfigurationJsonContext.Default.BicepDocumentationConfiguration)
                ?? throw new CommandLineException($"The docs configuration file \"{fullPath}\" must contain a JSON object.");
            configuration = NormalizeAndValidate(configuration);

            return new(configuration, resolver.PathToUri(fullPath));
        }
        catch (JsonException exception)
        {
            throw new CommandLineException(
                $"The docs configuration file \"{fullPath}\" is invalid: {exception.Message}",
                exception);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new CommandLineException(
                $"Unable to read docs configuration file \"{fullPath}\": {exception.Message}",
                exception);
        }
    }

    public static IOUri ResolvePath(
        string path,
        DocsConfigurationContext context,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem) =>
        resolver.PathToUri(ResolveFullPath(path, context, resolver, fileSystem));

    private static string ResolveFullPath(
        string path,
        DocsConfigurationContext context,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem)
    {
        if (fileSystem.Path.IsPathRooted(path))
        {
            return resolver.GetFullPath(path);
        }

        return resolver.GetFullPath(fileSystem.Path.Combine(context.FileUri.Resolve(".").GetFilePath(), path));
    }

    public static string? ResolveModulePath(
        string? path,
        DocsConfigurationContext context,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem)
    {
        if (path is null)
        {
            return null;
        }

        if (Path.GetExtension(path).Equals(".bicep", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        var fullPath = resolver.GetFullPath(path);
        return fileSystem.Directory.Exists(fullPath)
            ? fileSystem.Path.Combine(fullPath, context.Configuration.EntryPoint)
            : fullPath;
    }

    public static IOUri? ResolveTemplateFile(
        string? cliPath,
        DocsConfigurationContext context,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem) =>
        cliPath is not null
            ? resolver.PathToUri(cliPath)
            : context.Configuration.Template.File is { } configuredPath
                ? ResolvePath(configuredPath, context, resolver, fileSystem)
                : null;

    public static IOUri? ResolveTemplateRoot(
        string? cliPath,
        DocsConfigurationContext context,
        InputOutputArgumentsResolver resolver,
        IFileSystem fileSystem)
    {
        string? fullPath = cliPath is not null
            ? resolver.GetFullPath(cliPath)
            : context.Configuration.Template.IncludeRoot is { } configuredPath
                ? ResolveFullPath(configuredPath, context, resolver, fileSystem)
                : null;

        if (fullPath is null)
        {
            return null;
        }

        if (!fileSystem.Directory.Exists(fullPath))
        {
            throw new CommandLineException($"The template include root directory \"{fullPath}\" does not exist.");
        }

        var normalizedPath = fileSystem.Path.EndsInDirectorySeparator(fullPath)
            ? fullPath
            : fullPath + fileSystem.Path.DirectorySeparatorChar;
        return resolver.PathToUri(normalizedPath);
    }

    public static ImmutableSortedDictionary<string, string> MergeCustomValues(
        DocsConfigurationContext context,
        IReadOnlyDictionary<string, string> cliValues)
    {
        var values = context.Configuration.Template.Values.ToBuilder();
        foreach (var (key, value) in cliValues)
        {
            values[key] = value;
        }

        return values.ToImmutable();
    }

    private static BicepDocumentationConfiguration NormalizeAndValidate(
        BicepDocumentationConfiguration configuration)
    {
        configuration = configuration with
        {
            EntryPoint = configuration.EntryPoint ?? "main.bicep",
            Output = configuration.Output ?? new(),
            Template = configuration.Template ?? new(),
            Examples = configuration.Examples ?? new(),
        };
        configuration = configuration with
        {
            Output = configuration.Output with
            {
                File = configuration.Output.File ?? "README.md",
            },
            Template = configuration.Template with
            {
                Values = configuration.Template.Values ??
                    ImmutableSortedDictionary<string, string>.Empty.WithComparers(StringComparer.Ordinal),
            },
        };

        var defaultExamples = new BicepDocumentationExamplesConfiguration();
        var sources = configuration.Examples.Sources.IsDefault
            ? defaultExamples.Sources
            : configuration.Examples.Sources
                .Select(source => source with
                {
                    Include = source.Include.IsDefault ? [] : source.Include,
                    Exclude = source.Exclude.IsDefault ? [] : source.Exclude,
                })
                .ToImmutableArray();
        var reassignments = configuration.Examples.Reassignments.IsDefault
            ? []
            : configuration.Examples.Reassignments
                .Select(reassignment => reassignment with
                {
                    From = reassignment.From with
                    {
                        Include = reassignment.From.Include.IsDefault ? [] : reassignment.From.Include,
                        Exclude = reassignment.From.Exclude.IsDefault ? [] : reassignment.From.Exclude,
                    },
                })
                .ToImmutableArray();

        configuration = configuration with
        {
            Examples = configuration.Examples with
            {
                Sources = sources,
                Reassignments = reassignments,
            },
        };

        ValidateRelativePath(configuration.EntryPoint, "entryPoint", allowNested: true);
        if (!configuration.EntryPoint.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase))
        {
            throw new CommandLineException("The docs configuration entryPoint must identify a .bicep file.");
        }

        ValidateFileName(configuration.Output.File, "output.file");

        if (configuration.Template.File is not null)
        {
            ValidateNonempty(configuration.Template.File, "template.file");
        }

        if (configuration.Template.IncludeRoot is not null)
        {
            ValidateNonempty(configuration.Template.IncludeRoot, "template.includeRoot");
        }

        foreach (var (key, value) in configuration.Template.Values)
        {
            ValidateNonempty(key, "template.values key");
        }

        foreach (var source in configuration.Examples.Sources)
        {
            if (source.Path != ".")
            {
                ValidateRelativePath(source.Path, "examples.sources[].path", allowNested: true);
            }

            ValidatePatterns(source.Include, source.Exclude, "examples.sources[]");
        }

        foreach (var reassignment in configuration.Examples.Reassignments)
        {
            ValidatePatterns(reassignment.From.Include, reassignment.From.Exclude, "examples.reassignments[].from");
            if (reassignment.From.Include.IsDefaultOrEmpty)
            {
                throw new CommandLineException("examples.reassignments[].from.include must contain at least one pattern.");
            }

            ValidateRelativePath(reassignment.To, "examples.reassignments[].to", allowNested: false);
        }

        return configuration with
        {
            Template = configuration.Template with
            {
                Values = configuration.Template.Values.ToImmutableSortedDictionary(StringComparer.Ordinal),
            },
        };
    }

    private static void ValidateNoDuplicateProperties(JsonElement element, string path)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name))
                {
                    throw new CommandLineException(
                        $"The docs configuration contains the duplicate property \"{path}.{property.Name}\".");
                }

                if (property.Value.ValueKind is JsonValueKind.Null)
                {
                    throw new CommandLineException(
                        $"The docs configuration property \"{path}.{property.Name}\" cannot be null.");
                }

                ValidateNoDuplicateProperties(property.Value, $"{path}.{property.Name}");
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                if (item.ValueKind is JsonValueKind.Null)
                {
                    throw new CommandLineException(
                        $"The docs configuration property \"{path}[{index}]\" cannot be null.");
                }

                ValidateNoDuplicateProperties(item, $"{path}[{index++}]");
            }
        }
    }

    private static void ValidatePatterns(
        ImmutableArray<string> includes,
        ImmutableArray<string> excludes,
        string path)
    {
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        foreach (var pattern in includes)
        {
            ValidateNonempty(pattern, $"{path}.include[]");
            matcher.AddInclude(pattern);
        }

        foreach (var pattern in excludes)
        {
            ValidateNonempty(pattern, $"{path}.exclude[]");
            matcher.AddExclude(pattern);
        }
    }

    private static void ValidateRelativePath(string value, string path, bool allowNested)
    {
        ValidateNonempty(value, path);
        if (value.StartsWith('/') ||
            value.StartsWith('\\') ||
            (value.Length > 1 && value[1] == ':') ||
            FilePathFacts.IsWindowsDosDevicePath(value))
        {
            throw new CommandLineException($"The docs configuration {path} must be a relative path.");
        }

        var segments = value.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(segment => segment is "." or "..") ||
            (!allowNested && segments.Length != 1))
        {
            throw new CommandLineException($"The docs configuration {path} cannot traverse directories.");
        }
    }

    private static void ValidateFileName(string value, string path)
    {
        ValidateRelativePath(value, path, allowNested: false);
        if (value.Any(FilePathFacts.IsForbiddenPathCharacter) ||
            FilePathFacts.IsForbiddenPathTerminatorCharacter(value[^1]) ||
            FilePathFacts.ContainsWindowsReservedFileName(value))
        {
            throw new CommandLineException($"The docs configuration {path} must be a portable file name.");
        }
    }

    private static void ValidateNonempty(string value, string path)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new CommandLineException($"The docs configuration {path} cannot be empty.");
        }
    }
}

internal sealed record DocsConfigurationContext(
    BicepDocumentationConfiguration Configuration,
    IOUri FileUri);

[JsonSourceGenerationOptions(
    AllowDuplicateProperties = true,
    AllowTrailingCommas = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    ReadCommentHandling = JsonCommentHandling.Skip,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(BicepDocumentationConfiguration))]
internal partial class DocsConfigurationJsonContext : JsonSerializerContext;
