// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrint
{
    public static class PrettyPrinter
    {
        private static readonly IReadOnlyDictionary<int, string> IndentsBySize = Enumerable.Range(1, 8)
            .ToDictionary(size => size, size => new string(' ', size));

        public static string? PrintProgram(ProgramSyntax programSyntax, PrettyPrintOptions options)
        {
            if (programSyntax.GetParseDiagnostics().Count > 0)
            {
                return null;
            }

            Debug.Assert(options.IndentSize >= 1 && options.IndentSize <= 8);

            string indent = options.IndentKindOption == IndentKindOption.Space ?  IndentsBySize[options.IndentSize] : "\t";
            string newline = options.NewlineOption switch
            {
                NewlineOption.LF => "\n",
                NewlineOption.CRLF => "\r\n",
                NewlineOption.CR => "\r",
                _ => InferNewline(programSyntax)
            };

            var documentBuildVisitor = new DocumentBuildVisitor();
            var sb = new StringBuilder();

            var document = documentBuildVisitor.BuildDocument(programSyntax);
            document.Layout(sb, indent, newline);

            sb.TrimNewLines();

            if (options.InsertFinalNewline)
            {
                sb.Append(newline);
            }

            return sb.ToString();
        }

        private static string InferNewline(ProgramSyntax programSyntax)
        {
            var firstNewLine = (Token?)programSyntax.Children
                .FirstOrDefault(child => child is Token token && token.Type == TokenType.NewLine);

            if (firstNewLine != null)
            {
                return StringUtils.NewLineRegex.Match(firstNewLine.Text).Value;
            }

            return Environment.NewLine;
        }
    }
}
