// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2
{
    public class PrettyPrinterV2Context
    {
        private readonly IDiagnosticLookup lexingErrorLookup;

        private readonly IDiagnosticLookup parsingErrorLookup;

        private PrettyPrinterV2Context(PrettyPrinterV2Options options, string indent, string newline, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            this.Options = options;
            this.Indent = indent;
            this.Newline = newline;
            this.lexingErrorLookup = lexingErrorLookup;
            this.parsingErrorLookup = parsingErrorLookup;
        }

        public PrettyPrinterV2Options Options { get; }

        public string Indent { get; }

        public string Newline { get; }

        public int Width => this.Options.Width;

        public bool InsertFinalNewline => this.Options.InsertFinalNewline;

        public static PrettyPrinterV2Context From(SemanticModel semanticModel) =>
            Create(semanticModel.Configuration.Formatting.Data, semanticModel.LexingErrorLookup, semanticModel.ParsingErrorLookup);

        public static PrettyPrinterV2Context Create(PrettyPrinterV2Options options, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            var indent = options.IndentKind == IndentKind.Space ? new string(' ', options.IndentSize) : "\t";
            var newline = options.NewlineKind.ToEscapeSequence();

            return new(options, indent, newline, lexingErrorLookup, parsingErrorLookup);
        }

        public bool HasSyntaxError(SyntaxBase syntax) => this.lexingErrorLookup.Contains(syntax) || this.parsingErrorLookup.Contains(syntax);
    }
}
