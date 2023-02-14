// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public abstract class BicepSourceFile : ISourceFile
    {
        protected BicepSourceFile(ImmutableArray<int> lineStarts, ProgramSyntax programSyntax, Uri fileUri)
        {
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            FileUri = fileUri;
            Hierarchy = SyntaxHierarchy.Build(ProgramSyntax);
            DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        protected BicepSourceFile(BicepSourceFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;
            DisabledDiagnosticsCache = original.DisabledDiagnosticsCache;
        }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public Uri FileUri { get; }

        public abstract BicepSourceFileKind FileKind { get; }

        public ISyntaxHierarchy Hierarchy { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }

        public abstract BicepSourceFile ShallowClone();
    }
}
