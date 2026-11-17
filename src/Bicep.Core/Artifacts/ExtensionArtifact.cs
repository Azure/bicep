// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts;

/// <summary>
/// An extension as the compiler and local deploy use it: its types, and the binary local deploy runs if it has one for
/// this machine.
/// </summary>
public sealed class ExtensionArtifact : Artifact
{
    private readonly Lazy<IFileHandle?> lazyBinary;

    /// <param name="binary">This machine's binary, if the extension has one. It is only looked up when local deploy asks for it.</param>
    public ExtensionArtifact(ArtifactReference reference, IFileHandle types, Lazy<IFileHandle?> binary)
        : base(reference)
    {
        this.Types = types;
        this.lazyBinary = binary;
    }

    public IFileHandle Types { get; }

    public IFileHandle? Binary => this.lazyBinary.Value;
}
