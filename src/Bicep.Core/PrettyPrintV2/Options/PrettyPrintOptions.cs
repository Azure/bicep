// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Options
{
    public record PrettyPrintOptions(
        NewlineKind NewlineKind,
        IndentKind IndentKind,
        int IndentSize,
        int Width,
        bool InsertFinalNewline)
    {

        private const int MinIndentSize = 1;

        private const int MaxIndentSize = 1000;

        public int IndentSize { get; } = Math.Min(MaxIndentSize, Math.Max(MinIndentSize, IndentSize));

        public static string Indent => " ";

        public static string Newline => "\n";
    }
}
