// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2
{
    public record PrettyPrinterV2Options(
        IndentKind IndentKind = IndentKind.Space,
        NewlineKind NewlineKind = NewlineKind.LF,
        int IndentSize = 2,
        int Width = 80,
        bool InsertFinalNewline = true)
    {
        public static readonly PrettyPrinterV2Options Default = new();

        private const int MinIndentSize = 0;

        private const int MaxIndentSize = 1000;

        private const int MinWidth = 0;

        private readonly int indentSize = GetValidIndentSize(IndentSize);

        private readonly int width = GetValidWidth(Width);

        public int IndentSize
        {
            get => indentSize;
            init => indentSize = GetValidIndentSize(value);
        }

        public int Width
        {
            get => width;
            init => width = GetValidWidth(value);
        }

        private static int GetValidIndentSize(int indentSize) => Math.Min(MaxIndentSize, Math.Max(MinIndentSize, indentSize));

        private static int GetValidWidth(int width) => Math.Max(MinWidth, width);
    }
}
