// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.PrettyPrint.Options;

namespace Bicep.Core.PrettyPrint
{
    public class PrettyPrintOptions
    {
        private const int MinIndentSize = 1;

        private const int MaxIndentSize = 1000;

        public static PrettyPrintOptions Default = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false);

        public PrettyPrintOptions(
            NewlineOption newlineOption = NewlineOption.LF,
            IndentKindOption indentKindOption = IndentKindOption.Space,
            long indentSize = 2,
            bool insertFinalNewLine = false)
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
    }
}
