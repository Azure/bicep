// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.Core.SourceLink;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts;

/// <summary>
/// What the entry point of a module contains, which decides how the compiler parses it.
/// </summary>
public enum ModuleTemplateKind
{
    /// <summary>A local file, parsed according to its file extension (Bicep, Bicep parameters or an ARM template).</summary>
    SourceFile,

    /// <summary>A compiled ARM template, such as the template of a module published to an OCI registry.</summary>
    ArmTemplate,

    /// <summary>A Template Spec resource, whose template is its <c>mainTemplate</c> property.</summary>
    TemplateSpec,
}

/// <summary>
/// A module as the compiler and the language server use it: the file the compiler loads, and the sources it was published
/// with, if any.
/// </summary>
public sealed class ModuleArtifact : Artifact
{
    private readonly IFileHandle? sourcesFile;

    /// <param name="sourcesFile">The archive of the sources the module was published with, if it can have one.</param>
    public ModuleArtifact(ArtifactReference reference, IFileHandle entryPoint, ModuleTemplateKind kind, IFileHandle? sourcesFile = null)
        : base(reference)
    {
        this.EntryPoint = entryPoint;
        this.Kind = kind;
        this.sourcesFile = sourcesFile;
    }

    public IFileHandle EntryPoint { get; }

    public ModuleTemplateKind Kind { get; }

    /// <summary>
    /// Reads the sources the module was published with. Fails with <see cref="SourceNotAvailableException"/> if it was
    /// published without sources, or can't have any.
    /// </summary>
    public ResultWithException<SourceArchive> TryGetSources() => this.sourcesFile is { } file
        ? SourceArchive.TryUnpackFromFile(file)
        : new(new SourceNotAvailableException());
}
