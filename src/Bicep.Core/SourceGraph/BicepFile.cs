// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public class BicepFile : BicepSourceFile
    {
        public BicepFile(
            Uri fileUri,
            IFileHandle fileHandle,
            ImmutableArray<int> lineStarts,
            ProgramSyntax programSyntax,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IAuxiliaryFileCache auxiliaryFileCache,
            IDiagnosticLookup lexingErrorLookup,
            IDiagnosticLookup parsingErrorLookup)
            : base(
                  fileUri,
                  fileHandle,
                  lineStarts,
                  programSyntax,
                  configurationManager,
                  featureProviderFactory,
                  auxiliaryFileCache,
                  lexingErrorLookup,
                  parsingErrorLookup)
        {
        }

        private BicepFile(BicepFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.BicepFile;

        public override BicepSourceFile ShallowClone() => new BicepFile(this);
    }
}
