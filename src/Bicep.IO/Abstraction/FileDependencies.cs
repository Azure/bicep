// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.IO.Abstraction;

/// <summary>
/// The files a cache entry was built from, so the cache can tell whether the entry is still current.
/// Records the version of every file read (or that it didn't exist), whether each file checked only for existence exists,
/// and reads that failed, so that changing, deleting or creating any of the files, or a failed read, makes the entry stale.
/// </summary>
public sealed class FileDependencies
{
    private readonly ImmutableArray<Dependency> dependencies;

    private FileDependencies(ImmutableArray<Dependency> dependencies)
    {
        this.dependencies = dependencies;
    }

    /// <summary>
    /// Returns true if every recorded file still has the recorded version, and every file checked for existence still does or doesn't exist.
    /// </summary>
    public bool IsCurrent() => this.dependencies.All(dependency => dependency.IsCurrent());

    private abstract record Dependency
    {
        public abstract bool IsCurrent();
    }

    /// <param name="Version">The version when the file was recorded, or <see langword="null"/> if it didn't exist.</param>
    private sealed record ContentDependency(IFileHandle File, FileVersion? Version) : Dependency
    {
        public override bool IsCurrent() => this.File.TryGetVersion() == this.Version;
    }

    private sealed record ExistenceDependency(IFileHandle File, bool Exists) : Dependency
    {
        public override bool IsCurrent() => this.File.Exists() == this.Exists;
    }

    /// <summary>
    /// A file that couldn't be read or checked (for example, because it was locked). Never current, so the entry is retried.
    /// </summary>
    private sealed record FailedReadDependency : Dependency
    {
        public static readonly FailedReadDependency Instance = new();

        public override bool IsCurrent() => false;
    }

    /// <summary>
    /// Records dependencies while a cache entry is built. Not thread-safe.
    /// </summary>
    public sealed class Builder
    {
        private readonly ImmutableArray<Dependency>.Builder dependencies = ImmutableArray.CreateBuilder<Dependency>();

        /// <summary>
        /// Records the current version of <paramref name="file"/>. Call this before reading the file,
        /// so that a change made while it's being read makes the entry stale.
        /// </summary>
        /// <returns>The recorded version, or <see langword="null"/> if the file doesn't exist.</returns>
        public FileVersion? AddFile(IFileHandle file)
        {
            var version = file.TryGetVersion();
            this.dependencies.Add(new ContentDependency(file, version));

            return version;
        }

        /// <summary>
        /// Records whether <paramref name="file"/> exists, for entries that don't depend on the file's content.
        /// </summary>
        /// <returns>True if the file exists.</returns>
        public bool AddExistence(IFileHandle file)
        {
            var exists = file.Exists();
            this.dependencies.Add(new ExistenceDependency(file, exists));

            return exists;
        }

        /// <summary>
        /// Records that a file couldn't be read or checked, for example because it was locked. The failure may be temporary,
        /// and the file's version may not change when it goes away, so the entry is never current and is loaded again.
        /// </summary>
        public void AddFailedRead() => this.dependencies.Add(FailedReadDependency.Instance);

        public FileDependencies Build() => new(this.dependencies.ToImmutable());
    }
}
