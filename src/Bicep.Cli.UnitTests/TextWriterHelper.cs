// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

namespace Bicep.Cli.UnitTests
{
    public static class TextWriterHelper
    {
        public static async Task<string> InvokeWriterAction(Func<TextWriter, Task> action)
        {
            var buffer = new StringBuilder();
            using var writer = new StringWriter(buffer);

            await action(writer);

            await writer.FlushAsync();

            return buffer.ToString();
        }

        public static async Task<CliResult> InvokeWriterAction(Func<TextWriter, TextWriter, Task<int>> action)
        {
            var firstBuffer = new StringBuilder();
            using var firstWriter = new StringWriter(firstBuffer);

            var secondBuffer = new StringBuilder();
            using var secondWriter = new StringWriter(secondBuffer);

            var result = await action(firstWriter, secondWriter);

            await firstWriter.FlushAsync();
            await secondWriter.FlushAsync();

            return new(firstBuffer.ToString(), secondBuffer.ToString(), result);
        }
    }
}

