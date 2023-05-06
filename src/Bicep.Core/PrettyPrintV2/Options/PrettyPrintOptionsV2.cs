// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Options
{
    public record PrettyPrintOptionsV2(
        IndentKind IndentKind,
        NewlineKind NewlineKind,
        int IndentSize,
        int Width,
        bool InsertFinalNewline)
    {
        private const int MinIndentSize = 1;

        private const int MaxIndentSize = 1000;

        public int IndentSize { get; } = Math.Min(MaxIndentSize, Math.Max(MinIndentSize, IndentSize));

        public int Width { get; } = Math.Max(Width, 1);

        public string Indent { get; } = IndentKind == IndentKind.Space ? new string(' ', IndentSize) : "\t";

        public string Newline { get; } = NewlineKind switch
        {
            NewlineKind.CR => "\r",
            NewlineKind.LF => "\n",
            NewlineKind.CRLF => "\r\n",
            _ => throw new NotImplementedException(),
        };
    }
}
