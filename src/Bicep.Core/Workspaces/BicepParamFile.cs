// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepParamFile : BicepSourceFile
    {
        public BicepParamFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
            : base(lineStarts, programSyntax, fileUri)
        {
        }

        private BicepParamFile(BicepParamFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.ParamsFile;

        public override BicepSourceFile ShallowClone() => new BicepParamFile(this);
    }
}
