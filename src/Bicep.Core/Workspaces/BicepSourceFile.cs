// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public abstract class BicepSourceFile : ISourceFile
    {
        protected BicepSourceFile(ImmutableArray<int> lineStarts, ProgramSyntax programSyntax, Uri fileUri, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            FileUri = fileUri;
            Hierarchy = SyntaxHierarchy.Build(ProgramSyntax);
            LexingErrorLookup = lexingErrorLookup;
            ParsingErrorLookup = parsingErrorLookup;
            DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        protected BicepSourceFile(BicepSourceFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;
            LexingErrorLookup = original.LexingErrorLookup;
            ParsingErrorLookup = original.ParsingErrorLookup;
            DisabledDiagnosticsCache = original.DisabledDiagnosticsCache;
        }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public Uri FileUri { get; }

        public string GetOriginalSource() => ProgramSyntax.ToTextPreserveFormatting();

        public abstract BicepSourceFileKind FileKind { get; }

        public ISyntaxHierarchy Hierarchy { get; }

        public IDiagnosticLookup LexingErrorLookup { get; }

        public IDiagnosticLookup ParsingErrorLookup { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }

        public abstract BicepSourceFile ShallowClone();
    }
}
