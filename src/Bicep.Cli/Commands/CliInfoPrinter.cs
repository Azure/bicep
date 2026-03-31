// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Compression;
using Bicep.Core.Exceptions;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands
{
    public static class CliInfoPrinter
    {
        public static void PrintVersion(IOContext io, IEnvironment environment)
        {
            var output = $@"Bicep CLI version {environment.GetVersionString()}{System.Environment.NewLine}";

            io.Output.Writer.Write(output);
            io.Output.Writer.Flush();
        }

        public static void PrintLicense(IOContext io)
        {
            WriteEmbeddedResource(io.Output.Writer, "LICENSE.deflated");
        }

        public static void PrintThirdPartyNotices(IOContext io)
        {
            WriteEmbeddedResource(io.Output.Writer, "NOTICE.deflated");
        }

        private static void WriteEmbeddedResource(TextWriter writer, string streamName)
        {
            using var stream = typeof(CliInfoPrinter).Assembly.GetManifestResourceStream(streamName)
                ?? throw new BicepException($"The resource stream '{streamName}' is missing from this executable.");

            using var decompressor = new DeflateStream(stream, CompressionMode.Decompress);

            using var reader = new StreamReader(decompressor);
            string? line = null;
            while ((line = reader.ReadLine()) is not null)
            {
                writer.WriteLine(line);
            }
        }
    }
}
