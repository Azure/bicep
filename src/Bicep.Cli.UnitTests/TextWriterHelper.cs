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

        public static (string, string) InvokeWriterAction(Action<TextWriter, TextWriter> action)
        {
            var firstBuffer = new StringBuilder();
            using var firstWriter = new StringWriter(firstBuffer);

            var secondBuffer = new StringBuilder();
            using var secondWriter = new StringWriter(secondBuffer);

            action(firstWriter, secondWriter);

            firstWriter.Flush();
            secondWriter.Flush();

            return (firstBuffer.ToString(), secondBuffer.ToString());
        }
    }
}
