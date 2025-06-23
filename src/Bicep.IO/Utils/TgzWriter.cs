// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using System.Text;

namespace Bicep.IO.Utils
{
    public sealed class TgzWriter : IDisposable
    {
        private readonly GZipStream gzipStream;
        private readonly TarWriter tarWriter;

        public TgzWriter(Stream stream, bool leaveOpen = false)
        {
            this.gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen);
            this.tarWriter = new TarWriter(this.gzipStream, leaveOpen);
        }

        public void WriteEntry(string name, string contents)
        {
            var entry = new PaxTarEntry(TarEntryType.RegularFile, name)
            {
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
            };

            this.tarWriter.WriteEntry(entry);
        }

        public async Task WriteEntryAsync(string name, string contents)
        {
            var entry = new PaxTarEntry(TarEntryType.RegularFile, name)
            {
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
            };

            await this.tarWriter.WriteEntryAsync(entry);
        }

        public void Dispose()
        {
            this.tarWriter.Dispose();
            this.gzipStream.Dispose();
        }
    }
}
