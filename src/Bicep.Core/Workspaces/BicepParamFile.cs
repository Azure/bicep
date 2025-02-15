// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepParamFile : BicepSourceFile
    {
        public BicepParamFile(
            Uri fileUri,
            ImmutableArray<int> lineStarts,
            ProgramSyntax programSyntax,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IDiagnosticLookup lexingErrorLookup,
            IDiagnosticLookup parsingErrorLookup)
            : base(
                  fileUri,
                  lineStarts,
                  programSyntax,
                  configurationManager,
                  featureProviderFactory,
                  lexingErrorLookup,
                  parsingErrorLookup)
        {
        }

        private BicepParamFile(BicepParamFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.ParamsFile;

        public override BicepSourceFile ShallowClone() => new BicepParamFile(this);
    }
}
