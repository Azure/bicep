// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public record BicepFile : ISourceFile
    {
        public BicepFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax, bool isParamsFile)
        {
            FileUri = fileUri;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            IsParamsFile = isParamsFile;
            Hierarchy = new SyntaxHierarchy();
            Hierarchy.AddRoot(ProgramSyntax);
            DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public bool IsParamsFile { get; }

        public SyntaxHierarchy Hierarchy { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }
    }
}
