// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepDeployFile : BicepSourceFile
    {
        public BicepDeployFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
            : base(lineStarts, programSyntax, fileUri, lexingErrorLookup, parsingErrorLookup)
        {
        }

        private BicepDeployFile(BicepDeployFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.DeployFile;

        public override BicepSourceFile ShallowClone() => new BicepDeployFile(this);
    }
}
