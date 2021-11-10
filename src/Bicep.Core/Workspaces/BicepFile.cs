// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class BicepFile : ISourceFile
    {
        public BicepFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            FileUri = fileUri;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            Hierarchy = new SyntaxHierarchy();
            Hierarchy.AddRoot(ProgramSyntax);

            var disabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
            DisableNextLineDiagnosticDirectivesCache = disabledDiagnosticsCache.DisableNextLineDiagnosticDirectivesCache;
        }

        public BicepFile(BicepFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;
            DisableNextLineDiagnosticDirectivesCache = original.DisableNextLineDiagnosticDirectivesCache;
        }

        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public SyntaxHierarchy Hierarchy { get; }

        public ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes> DisableNextLineDiagnosticDirectivesCache { get; }
    }
}
