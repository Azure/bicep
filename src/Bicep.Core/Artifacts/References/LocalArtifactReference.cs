// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.References;

/// <summary>
/// A reference to a file on disk, such as a relative path or a <c>moduleAliasesMock</c> hit. Local references are never
/// restored; <see cref="LocalArtifactLoader"/> loads them.
/// </summary>
/// <remarks>
/// Two references are equal if they name the same file as the same artifact type, however they were written.
/// </remarks>
public sealed class LocalArtifactReference : ArtifactReference
{
    private readonly string displayReference;

    /// <param name="displayReference">How the reference is shown to the user, for example in diagnostics.</param>
    public LocalArtifactReference(ArtifactType type, IFileHandle file, string displayReference)
        : base(ArtifactSchemes.Local, type)
    {
        this.File = file;
        this.displayReference = displayReference;
    }

    public IFileHandle File { get; }

    public override string FullyQualifiedReference => this.displayReference;

    public override string UnqualifiedReference => this.displayReference;

    public override bool Equals(object? obj) =>
        obj is LocalArtifactReference other &&
        this.Type == other.Type &&
        this.File.Uri.Equals(other.File.Uri);

    public override int GetHashCode() => HashCode.Combine(this.Type, this.File.Uri);
}
