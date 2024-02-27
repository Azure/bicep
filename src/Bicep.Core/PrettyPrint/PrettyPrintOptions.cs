// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.Core.PrettyPrint
{
    public class PrettyPrintOptions
    {
        private const int MinIndentSize = 1;

        private const int MaxIndentSize = 1000;

        public PrettyPrintOptions(
            NewlineOption newlineOption,
            IndentKindOption indentKindOption,
            long indentSize,
            bool insertFinalNewLine)
        {
            this.NewlineOption = newlineOption;
            this.IndentKindOption = indentKindOption;
            this.IndentSize = (int)Math.Min(MaxIndentSize, Math.Max(MinIndentSize, indentSize));
            this.InsertFinalNewline = insertFinalNewLine;
        }

        public NewlineOption NewlineOption { get; }

        public IndentKindOption IndentKindOption { get; }

        public int IndentSize { get; }

        public bool InsertFinalNewline { get; }

        public static PrettyPrintOptions FromV2Options(PrettyPrinterV2Options v2Options)
        {
            var newlineKind = v2Options.NewlineKind switch
            {
                NewlineKind.LF => NewlineOption.LF,
                NewlineKind.CRLF => NewlineOption.CRLF,
                NewlineKind.CR => NewlineOption.CR,
                _ => NewlineOption.LF,
            };

            var indentKind = v2Options.IndentKind switch
            {
                IndentKind.Space => IndentKindOption.Space,
                IndentKind.Tab => IndentKindOption.Tab,
                _ => IndentKindOption.Space
            };

            return new(newlineKind, indentKind, v2Options.IndentSize, v2Options.InsertFinalNewline);
        }
    }
}
