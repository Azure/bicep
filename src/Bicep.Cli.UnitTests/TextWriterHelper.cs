// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Cli.UnitTests
{
    public static class TextWriterHelper
    {
        public static async Task<string> InvokeWriterAction(Func<TextWriter, Task> action)
        {
            var buffer = new StringBuilder();
            using var writer = new StringWriter(buffer);

            await action(writer);

            writer.Flush();

            return buffer.ToString();
        }

        public static async Task<(string output, string error, int result)> InvokeWriterAction(Func<TextWriter, TextWriter, Task<int>> action)
        {
            var firstBuffer = new StringBuilder();
            using var firstWriter = new StringWriter(firstBuffer);

            var secondBuffer = new StringBuilder();
            using var secondWriter = new StringWriter(secondBuffer);

            var result = await action(firstWriter, secondWriter);

            firstWriter.Flush();
            secondWriter.Flush();

            return (firstBuffer.ToString(), secondBuffer.ToString(), result);
        }
    }
}

