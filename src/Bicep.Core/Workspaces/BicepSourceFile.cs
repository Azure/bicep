// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public abstract class BicepSourceFile : ISourceFile
    {
        protected BicepSourceFile(
            Uri fileUri,
            ImmutableArray<int> lineStarts,
            ProgramSyntax programSyntax,
            RootConfiguration configuration,
            IFeatureProvider features,
            IDiagnosticLookup lexingErrorLookup,
            IDiagnosticLookup parsingErrorLookup)
        {
            this.Uri = fileUri;
            this.LineStarts = lineStarts;
            this.ProgramSyntax = programSyntax;
            this.Hierarchy = SyntaxHierarchy.Build(ProgramSyntax);
            this.Configuration = configuration;
            this.Features = features;
            this.LexingErrorLookup = lexingErrorLookup;
            this.ParsingErrorLookup = parsingErrorLookup;
            this.DisabledDiagnosticsCache = new DisabledDiagnosticsCache(ProgramSyntax, lineStarts);
        }

        protected BicepSourceFile(BicepSourceFile original)
        {
            this.Uri = original.Uri;
            this.LineStarts = original.LineStarts;
            this.ProgramSyntax = original.ProgramSyntax;
            this.Hierarchy = original.Hierarchy;
            this.Configuration = original.Configuration;
            this.Features = original.Features;
            this.LexingErrorLookup = original.LexingErrorLookup;
            this.ParsingErrorLookup = original.ParsingErrorLookup;
            this.DisabledDiagnosticsCache = original.DisabledDiagnosticsCache;
        }

        public Uri Uri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public string Text => ProgramSyntax.ToString();

        public abstract BicepSourceFileKind FileKind { get; }

        public ISyntaxHierarchy Hierarchy { get; }

        public RootConfiguration Configuration { get; }

        public IFeatureProvider Features { get; }

        public IDiagnosticLookup LexingErrorLookup { get; }

        public IDiagnosticLookup ParsingErrorLookup { get; }

        public DisabledDiagnosticsCache DisabledDiagnosticsCache { get; }

        public abstract BicepSourceFile ShallowClone();
    }
}
