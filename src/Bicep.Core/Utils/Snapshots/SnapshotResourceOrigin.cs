// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Utils.Snapshots;

/// <summary>
/// Where a predicted resource was declared.
/// </summary>
/// <param name="SymbolicNamePath">
/// Symbolic names from the root template to the resource, e.g. <c>["outer[1]", "inner", "request[0]"]</c>.
/// A null entry is a level whose symbolic name is unknown: the resource's own name when its template has none,
/// or ancestors the engine dropped above a deployment without a symbolic name. Preflight chains are padded
/// with nulls to their nesting depth (read from <paramref name="TemplatePath"/>), so a truncated chain never
/// looks rooted. Extensible resources have no path: one that lost every ancestor gets a single leading null,
/// but one that lost only some (a named deployment below an unnamed one) cannot be detected and can match a
/// rooted chain. Origins are unique only where every level has a symbolic name; copy instances without them
/// share an origin.
/// </param>
/// <param name="TemplatePath">
/// The engine's template path, e.g. <c>resources.mod.properties.template.resources[0]</c>. It locates resources
/// whose symbolic names are unknown, and engine parameter provenance is keyed by it. Null for extensible
/// resources, which the engine reports without one.
/// </param>
public sealed record SnapshotResourceOrigin(
    ImmutableArray<string?> SymbolicNamePath,
    string? TemplatePath)
{
    // Get-only so neither `with` nor an object initializer can bypass the normalization.
    public ImmutableArray<string?> SymbolicNamePath { get; } = SymbolicNamePath.IsDefault ? [] : SymbolicNamePath;

    public bool Equals(SnapshotResourceOrigin? other) =>
        other is not null &&
        string.Equals(TemplatePath, other.TemplatePath, StringComparison.Ordinal) &&
        SymbolicNamePath.SequenceEqual(other.SymbolicNamePath, StringComparer.Ordinal);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TemplatePath, StringComparer.Ordinal);
        foreach (var segment in SymbolicNamePath)
        {
            hash.Add(segment, StringComparer.Ordinal);
        }

        return hash.ToHashCode();
    }
}
