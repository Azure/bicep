// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Records which configuration file declared each value of a merged configuration.
/// </summary>
/// <remarks>
/// A file is recorded for the path of each value it set, and covers everything under that path that no later file set,
/// so a lookup uses the longest recorded prefix.
/// </remarks>
public sealed class ConfigurationProvenance
{
    private readonly ImmutableDictionary<ConfigurationPropertyPath, IOUri> declaringFiles;

    public ConfigurationProvenance(ImmutableDictionary<ConfigurationPropertyPath, IOUri> declaringFiles)
    {
        this.declaringFiles = declaringFiles;
    }

    /// <summary>
    /// Returns the file that declared the value at <paramref name="path"/>, or null if it comes from the built-in configuration.
    /// </summary>
    public IOUri? GetDeclaringFile(ConfigurationPropertyPath path)
    {
        for (var current = path; current.Count > 0; current = current.Parent)
        {
            if (this.declaringFiles.TryGetValue(current, out var file))
            {
                return file;
            }
        }

        return null;
    }
}