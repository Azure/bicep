// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.PrettyPrintV2
{
    public static class NewlineKindExtensions
    {
        public static string ToEscapeSequence(this NewlineKind newlineKind) => newlineKind switch
        {
            NewlineKind.LF => "\n",
            NewlineKind.CRLF => "\r\n",
            NewlineKind.CR => "\r",
            _ => throw new InvalidOperationException($"Unrecognized newline kind '{newlineKind}'."),
        };
    }
}
