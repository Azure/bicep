// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Bicep.Cli
{
    public class TextWriterTraceListener(TextWriter textWriter) : TraceListener
    {
        private readonly TextWriter textWriter = textWriter;

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
