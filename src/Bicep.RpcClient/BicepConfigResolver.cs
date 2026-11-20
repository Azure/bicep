// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.RpcClient.SemanticVersioning;

namespace Bicep.RpcClient;

/// <summary>
/// Resolves version constraints before a Bicep CLI installation is available.
/// </summary>
internal static class BicepConfigResolver
{
    private const int MaxExtendsDepth = 64;

    private static readonly JsonSerializerOptions ConfigOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private sealed record ConfigFile(
        [property: JsonPropertyName("extends")] string? Extends,
        [property: JsonPropertyName("bicep")] BicepSection? Bicep);

    private sealed record BicepSection([property: JsonPropertyName("version")] string? Version);

    /// <summary>
    /// Finds the nearest bicepconfig.json above a source file and follows its relative extends chain.
    /// The nearest layer declaring bicep.version wins. Invalid chains and constraints throw.
    /// </summary>
    public static async Task<ResolvedConfigChain> ResolveConfigChain(string sourceFilePath, CancellationToken cancellationToken = default)
    {
        var configPath = FindNearestConfigFile(sourceFilePath, cancellationToken);
        var visited = new HashSet<string>(Path.DirectorySeparatorChar == '\\' ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        var resolved = new ResolvedConfigChain(null, null);
        while (configPath is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!visited.Add(configPath))
            {
                throw new InvalidOperationException($"Cycle detected while resolving the 'extends' chain at '{configPath}'.");
            }

            if (visited.Count > MaxExtendsDepth)
            {
                throw new InvalidOperationException($"The 'extends' chain exceeds the maximum supported depth of {MaxExtendsDepth}.");
            }

            var config = await ReadConfigFile(configPath, cancellationToken).ConfigureAwait(false);
            if (resolved.BicepVersionRange is null && config.Bicep?.Version is { } value)
            {
                if (!VersionRange.TryParse(value, out var range) || range is null)
                {
                    throw new InvalidOperationException($"The 'bicep.version' property '{value}' in '{configPath}' is not a valid Bicep version or version range.");
                }

                if (!range.IsSatisfiable)
                {
                    throw new InvalidOperationException($"The 'bicep.version' constraint '{value}' in '{configPath}' can never be satisfied by any version.");
                }

                resolved = new(range, configPath);
            }

            var extendsPath = config.Extends;
            if (string.IsNullOrWhiteSpace(extendsPath))
            {
                break;
            }

            if (Path.IsPathRooted(extendsPath))
            {
                throw new InvalidOperationException($"The 'extends' property in '{configPath}' must be a relative path.");
            }

            var configDirectory = Path.GetDirectoryName(configPath)
                ?? throw new InvalidOperationException($"Unable to determine the directory containing '{configPath}'.");
            configPath = Path.GetFullPath(Path.Combine(configDirectory, extendsPath));
        }

        return resolved;
    }

    private static string? FindNearestConfigFile(string sourceFilePath, CancellationToken cancellationToken)
    {
        var directory = Directory.GetParent(Path.GetFullPath(sourceFilePath));
        while (directory is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var candidate = Path.Combine(directory.FullName, "bicepconfig.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static async Task<ConfigFile> ReadConfigFile(string configPath, CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(configPath);
        try
        {
            return await JsonSerializer.DeserializeAsync<ConfigFile>(stream, ConfigOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new JsonException($"The configuration in '{configPath}' must be an object.");
        }
        catch (JsonException exception)
        {
            throw new JsonException($"Unable to read Bicep configuration '{configPath}': {exception.Message}", exception);
        }
    }
}
