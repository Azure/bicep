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
    }
}
