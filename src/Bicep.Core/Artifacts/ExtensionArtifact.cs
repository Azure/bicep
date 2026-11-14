// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts;

/// <summary>
/// An extension as the compiler and local deploy use it: its types, and the binary local deploy runs if it has one for
/// this machine. It is backed by files, which are read on demand.
/// </summary>
public sealed class ExtensionArtifact
{
    private readonly Lazy<IFileHandle?> lazyBinary;

    /// <param name="binary">This machine's binary, if the extension has one. It is only looked up when local deploy asks for it.</param>
    public ExtensionArtifact(IFileHandle types, Lazy<IFileHandle?> binary)
    {
        this.Types = types;
        this.lazyBinary = binary;
    }

    public IFileHandle Types { get; }

    public IFileHandle? Binary => this.lazyBinary.Value;
}
