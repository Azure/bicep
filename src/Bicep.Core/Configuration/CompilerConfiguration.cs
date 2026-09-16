// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.Core.SemanticVersioning;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Configures the Bicep compiler itself.
/// </summary>
public sealed record CompilerOptions
{
    /// <summary>
    /// Gets the raw, unparsed accepted Bicep compiler version or version range, or null if unspecified.
    /// </summary>
    public string? Version { get; init; }
}

/// <summary>
/// Provides the "bicep" section of a Bicep configuration.
/// </summary>
public sealed class CompilerConfiguration : ConfigurationSection<CompilerOptions>, IBicepCompilerConfiguration
{
    public CompilerConfiguration(CompilerOptions data, VersionRange? version, IOUri? declaringConfigUri = null)
        : base(data)
    {
        Version = version;
        DeclaringConfigUri = declaringConfigUri;
    }

    public VersionRange? Version { get; }

    public IOUri? DeclaringConfigUri { get; }

    public static CompilerConfiguration Bind(JsonElement element)
    {
        var data = element.ToNonNullObject<CompilerOptions>();

        VersionRange? version = null;
        if (data.Version is { } versionString)
        {
            if (!VersionRange.TryParse(versionString, out version))
            {
                throw new ConfigurationException($"The bicep.version property \"{versionString}\" is not a valid Bicep version or version range.");
            }
        }

        return new(data, version);
    }

    /// <summary>
    /// Returns a copy of this configuration annotated with the URI of the config file that declared
    /// "bicep.version".
    /// </summary>
    public CompilerConfiguration WithDeclaringUri(IOUri declaringConfigUri) => new(Data, Version, declaringConfigUri);
}
