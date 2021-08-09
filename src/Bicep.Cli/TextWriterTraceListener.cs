// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;

namespace Bicep.Cli
{
    public class TextWriterTraceListener : TraceListener
    {
        private readonly TextWriter textWriter;

        public TextWriterTraceListener(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public override void Write(string? message)
        {
            textWriter.WriteLine($"TRACE: {message}");
        }

        public override void WriteLine(string? message)
        {
            textWriter.WriteLine($"TRACE: {message}");
        }
    }
}
