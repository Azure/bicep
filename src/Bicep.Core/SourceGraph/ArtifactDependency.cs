// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;

namespace Bicep.Core.SourceGraph;

/// <summary>
/// A source file's dependency on an artifact (a module or an extension), and whether the artifact could be loaded.
/// </summary>
public abstract record ArtifactDependency
{
    private ArtifactDependency(BicepSourceFile referencingFile, IArtifactReferenceSyntax? syntax)
    {
        this.ReferencingFile = referencingFile;
        this.Syntax = syntax;
    }

    /// <summary>
    /// Gets the file that declares the dependency.
    /// </summary>
    public BicepSourceFile ReferencingFile { get; }

    /// <summary>
    /// Gets the declaration, or <c>null</c> for an implicit extension, which is declared in the Bicep configuration.
    /// </summary>
    public IArtifactReferenceSyntax? Syntax { get; }

    /// <summary>
    /// Gets the reference the declaration resolved to, or <c>null</c> if it did not resolve.
    /// </summary>
    public abstract ArtifactReference? Reference { get; }

    /// <summary>
    /// Gets the artifact, or the error that explains why it is not available.
    /// </summary>
    public abstract ResultWithDiagnosticBuilder<Artifact> TryGetArtifact();

    /// <summary>
    /// The artifact was loaded.
    /// </summary>
    public sealed record Loaded : ArtifactDependency
    {
        public Loaded(BicepSourceFile referencingFile, IArtifactReferenceSyntax? syntax, Artifact artifact)
            : base(referencingFile, syntax)
        {
            this.Artifact = artifact;
        }

        public Artifact Artifact { get; }

        public override ArtifactReference Reference => this.Artifact.Reference;

        public override ResultWithDiagnosticBuilder<Artifact> TryGetArtifact() => new(this.Artifact);
    }

    /// <summary>
    /// The artifact is remote and has not been restored into the local cache.
    /// </summary>
    public sealed record NotRestored : ArtifactDependency
    {
        public NotRestored(BicepSourceFile referencingFile, IArtifactReferenceSyntax? syntax, RemoteArtifactReference reference)
            : base(referencingFile, syntax)
        {
            this.Reference = reference;
        }

        public override RemoteArtifactReference Reference { get; }

        public override ResultWithDiagnosticBuilder<Artifact> TryGetArtifact() =>
            new(x => x.ArtifactRequiresRestore(this.Reference.FullyQualifiedReference));
    }

    /// <summary>
    /// The declaration did not resolve to a reference, or the artifact could not be loaded: its restore failed, it is
    /// part of a cycle, or it is a local extension that could not be extracted.
    /// </summary>
    public sealed record Failed : ArtifactDependency
    {
        /// <param name="reference">The reference the declaration resolved to, or <c>null</c> if it did not resolve.</param>
        public Failed(BicepSourceFile referencingFile, IArtifactReferenceSyntax? syntax, ArtifactReference? reference, DiagnosticBuilder.DiagnosticBuilderDelegate error)
            : base(referencingFile, syntax)
        {
            this.Reference = reference;
            this.Error = error;
        }

        public override ArtifactReference? Reference { get; }

        public DiagnosticBuilder.DiagnosticBuilderDelegate Error { get; }

        public override ResultWithDiagnosticBuilder<Artifact> TryGetArtifact() => new(this.Error);
    }
}
