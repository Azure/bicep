// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph;

public class BicepReplFile : BicepSourceFile
{
    public BicepReplFile(
        IFileHandle fileHandle,
        IDirectoryHandle auxiliaryDirectoryHandle,
        ImmutableArray<int> lineStarts,
        ProgramSyntax programSyntax,
        IConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory,
        IAuxiliaryFileCache auxiliaryFileCache,
        IDiagnosticLookup lexingErrorLookup,
        IDiagnosticLookup parsingErrorLookup)
        : base(
            fileHandle.Uri.ToUri(),
            fileHandle,
            lineStarts,
            programSyntax,
            configurationManager,
            featureProviderFactory,
            auxiliaryFileCache,
            lexingErrorLookup,
            parsingErrorLookup)
    {
        AuxiliaryDirectoryHandle = auxiliaryDirectoryHandle;
    }

    private BicepReplFile(BicepReplFile original) : base(original)
    {
        AuxiliaryDirectoryHandle = original.AuxiliaryDirectoryHandle;
    }

    public override BicepSourceFileKind FileKind => BicepSourceFileKind.ReplFile;

    public IDirectoryHandle AuxiliaryDirectoryHandle { get; }

    public override BicepSourceFile ShallowClone() => new BicepReplFile(this);

    protected override IDirectoryHandle GetDirectoryHandle() => this.AuxiliaryDirectoryHandle;
}
