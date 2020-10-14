// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Text;

namespace Bicep.Cli.UnitTests
{
    public static class TextWriterHelper
    {
        public static string InvokeWriterAction(Action<TextWriter> action)
        {
            var buffer = new StringBuilder();
            using var writer = new StringWriter(buffer);

            action(writer);

            writer.Flush();

            return buffer.ToString();
        }

        public static (string output, string error, int result) InvokeWriterAction(Func<TextWriter, TextWriter, int> action)
        {
            var firstBuffer = new StringBuilder();
            using var firstWriter = new StringWriter(firstBuffer);

            var secondBuffer = new StringBuilder();
            using var secondWriter = new StringWriter(secondBuffer);

            var result = action(firstWriter, secondWriter);

            firstWriter.Flush();
            secondWriter.Flush();

            return (firstBuffer.ToString(), secondBuffer.ToString(), result);
        }
    }
}

