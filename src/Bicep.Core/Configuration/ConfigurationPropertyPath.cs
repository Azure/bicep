// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Json.Path;

namespace Bicep.Core.Configuration;

/// <summary>
/// The path to a configuration property, as the list of property names from the configuration root,
/// for example ["cloud", "profiles", "my.cloud"]. Compares by value.
/// </summary>
public readonly struct ConfigurationPropertyPath : IEquatable<ConfigurationPropertyPath>
{
    private readonly ImmutableArray<string> names;

    public ConfigurationPropertyPath(ImmutableArray<string> names)
    {
        this.names = names;
    }

    public static ConfigurationPropertyPath Root => default;

    public ImmutableArray<string> Names => this.names.IsDefault ? [] : this.names;

    public int Count => this.Names.Length;

    public ConfigurationPropertyPath Parent => new(this.Names.Slice(0, Math.Max(0, this.Count - 1)));

    public static ConfigurationPropertyPath Create(params ReadOnlySpan<string> names) => new([.. names]);

    public ConfigurationPropertyPath Append(string name) => new(this.Names.Add(name));

    public ConfigurationPropertyPath Append(ConfigurationPropertyPath path) => new(this.Names.AddRange(path.Names));

    /// <summary>
    /// Returns true if this path is under <paramref name="ancestor"/>, not counting <paramref name="ancestor"/> itself.
    /// </summary>
    public bool IsDescendantOf(ConfigurationPropertyPath ancestor) =>
        this.Count > ancestor.Count && this.Names.Take(ancestor.Count).SequenceEqual(ancestor.Names, StringComparer.Ordinal);

    /// <summary>
    /// Converts a path reported by System.Text.Json, such as "$.version" or "$['my.name'].x" (see <see cref="System.Text.Json.JsonException.Path"/>).
    /// The result stops at the first array index, because arrays are replaced as a whole. Returns <see cref="Root"/> if the path can't be parsed.
    /// </summary>
    public static ConfigurationPropertyPath FromJsonPath(string? jsonPath)
    {
        if (jsonPath is null || !JsonPath.TryParse(jsonPath, out var path))
        {
            return Root;
        }

        var names = ImmutableArray.CreateBuilder<string>();
        foreach (var segment in path.Segments)
        {
            if (segment.IsRecursive || segment.Selectors is not [NameSelector selector])
            {
                break;
            }

            names.Add(selector.Name);
        }

        return new(names.ToImmutable());
    }

    public bool Equals(ConfigurationPropertyPath other) => this.Names.SequenceEqual(other.Names, StringComparer.Ordinal);

    public override bool Equals(object? obj) => obj is ConfigurationPropertyPath other && this.Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var name in this.Names)
        {
            hashCode.Add(name, StringComparer.Ordinal);
        }

        return hashCode.ToHashCode();
    }

    public static bool operator ==(ConfigurationPropertyPath left, ConfigurationPropertyPath right) => left.Equals(right);

    public static bool operator !=(ConfigurationPropertyPath left, ConfigurationPropertyPath right) => !left.Equals(right);

    public override string ToString() => string.Join(".", this.Names);
}
