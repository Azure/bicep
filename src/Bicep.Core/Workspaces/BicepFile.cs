// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepFile : BicepSourceFile
    {
        public BicepFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
            : base(lineStarts, programSyntax, fileUri, lexingErrorLookup, parsingErrorLookup)
        {
        }

        private BicepFile(BicepFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.BicepFile;

        public override BicepSourceFile ShallowClone() => new BicepFile(this);
    }
}
