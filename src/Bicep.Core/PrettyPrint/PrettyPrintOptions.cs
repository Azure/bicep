// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrint.Options;

namespace Bicep.Core.PrettyPrint
{
    public class PrettyPrintOptions(
        NewlineOption newlineOption,
        IndentKindOption indentKindOption,
        long indentSize,
        bool insertFinalNewLine)
    {
        private const int MinIndentSize = 1;

        private const int MaxIndentSize = 1000;

        public NewlineOption NewlineOption { get; } = newlineOption;

        public IndentKindOption IndentKindOption { get; } = indentKindOption;

        public int IndentSize { get; } = (int)Math.Min(MaxIndentSize, Math.Max(MinIndentSize, indentSize));

        public bool InsertFinalNewline { get; } = insertFinalNewLine;
    }
}
