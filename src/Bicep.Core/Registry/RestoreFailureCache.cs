// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Registry;

/// <summary>
/// Remembers recent restore failures so that, as the user types and the compilation is rebuilt, references to
/// artifacts that cannot be restored are not retried on every keystroke.
/// </summary>
public sealed class RestoreFailureCache
{
    // Absolute rather than sliding expiration, so that a failed restore is eventually retried.
    private static readonly TimeSpan ExpirationInterval = TimeSpan.FromMinutes(30);

    private readonly ConcurrentDictionary<(CloudConfiguration Cloud, ArtifactReference Reference), Entry> failures = new();

    private readonly TimeProvider timeProvider;

    public RestoreFailureCache(TimeProvider? timeProvider = null)
    {
        this.timeProvider = timeProvider ?? TimeProvider.System;
    }

    public bool TryGetFailure(ArtifactReference reference, [NotNullWhen(true)] out DiagnosticBuilder.DiagnosticBuilderDelegate? error)
    {
        if (this.failures.TryGetValue(GetKey(reference), out var entry) && !this.IsExpired(entry))
        {
            error = entry.Error;
            return true;
        }

        error = null;
        return false;
    }

    public void Add(ArtifactReference reference, DiagnosticBuilder.DiagnosticBuilderDelegate error) =>
        this.failures.TryAdd(GetKey(reference), new(error, this.timeProvider.GetUtcNow().Add(ExpirationInterval)));

    public void Remove(ArtifactReference reference) => this.failures.TryRemove(GetKey(reference), out _);

    public void RemoveExpired()
    {
        // Enumerating a ConcurrentDictionary does not take a snapshot, but it does not lock either.
        foreach (var (key, entry) in this.failures)
        {
            if (this.IsExpired(entry))
            {
                this.failures.TryRemove(key, out _);
            }
        }
    }

    private bool IsExpired(Entry entry) => this.timeProvider.GetUtcNow() >= entry.Expiration;

    // Restore depends on the cloud (for authentication and the registry endpoint), so the same reference can
    // fail in one cloud and succeed in another.
    private static (CloudConfiguration, ArtifactReference) GetKey(ArtifactReference reference) => ((CloudConfiguration)reference.Configuration.Cloud, reference);

    private sealed record Entry(DiagnosticBuilder.DiagnosticBuilderDelegate Error, DateTimeOffset Expiration);
}
