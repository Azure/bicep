// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepParamFile : ISourceFile
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

        // TODO: not sure if this secondary constructor is necessary
        public BicepParamFile(BicepParamFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;
            DisabledDiagnosticsCache = original.DisabledDiagnosticsCache;
        }
        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public SyntaxHierarchy Hierarchy { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }
    }
}