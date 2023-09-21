// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2
{
    public class PrettyPrinterV2Context
    {
        private readonly IDiagnosticLookup lexingErrorLookup;

        private readonly IDiagnosticLookup parsingErrorLookup;

        private PrettyPrinterV2Context(SyntaxBase syntaxToPrint, PrettyPrinterV2Options options, string indent, string newline, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            this.SyntaxToPrint = syntaxToPrint;
            this.Options = options;
            this.Indent = indent;
            this.Newline = newline;
            this.lexingErrorLookup = lexingErrorLookup;
            this.parsingErrorLookup = parsingErrorLookup;
        }

        public SyntaxBase SyntaxToPrint { get; }

        public PrettyPrinterV2Options Options { get; }

        public string Indent { get; }

        public string Newline { get; }

        public int Width => this.Options.Width;

        public bool InsertFinalNewline => this.Options.InsertFinalNewline;

        public static PrettyPrinterV2Context Create(SyntaxBase syntaxToPrint, PrettyPrinterV2Options options, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            var indent = options.IndentKind == IndentKind.Space ? new string(' ', options.IndentSize) : "\t";
            var newline = options.NewlineKind switch
            {
                NewlineKind.CR => "\r",
                NewlineKind.LF => "\n",
                NewlineKind.CRLF => "\r\n",
                NewlineKind.Auto => NewlineFinder.TryFindNewline(syntaxToPrint) ?? "\r",
                _ => throw new NotImplementedException(),
            };

            return new(syntaxToPrint, options, indent, newline, lexingErrorLookup, parsingErrorLookup);
        }

        public bool HasSyntaxError(SyntaxBase syntax) => this.lexingErrorLookup.Contains(syntax) || this.parsingErrorLookup.Contains(syntax);

        private class NewlineFinder : CstVisitor
        {
            public string? newline;

            public static string? TryFindNewline(SyntaxBase syntax)
            {
                var finder = new NewlineFinder();

                finder.Visit(syntax);

                return finder.newline;
            }

            public override void VisitToken(Token token)
            {
                if (token.IsOf(TokenType.NewLine))
                {
                    this.newline = StringUtils.MatchNewline(token.Text);
                }
            }

            protected override void VisitInternal(SyntaxBase node)
            {
                if (this.newline is not null)
                {
                    return;
                }

                base.VisitInternal(node);
            }
        }
    }
}
