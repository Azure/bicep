// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepParamFile : IBicepSourceFile
    {
        public BicepParamFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            FileUri = fileUri;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            Hierarchy = new SyntaxHierarchy();
            Hierarchy.AddRoot(ProgramSyntax);
            DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public SyntaxHierarchy Hierarchy { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }
    }
}
