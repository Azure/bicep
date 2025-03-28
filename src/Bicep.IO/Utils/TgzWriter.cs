// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Utils
{
    public sealed class TgzWriter : IDisposable
    {
        private readonly GZipStream gzipStream;
        private readonly TarWriter tarWriter;

        public TgzWriter(Stream stream)
        {
            this.gzipStream = new GZipStream(stream, CompressionMode.Compress);
            this.tarWriter = new TarWriter(gzipStream);
        }

        public void WriteEntry(string name, string contents)
        {
            var entry = new PaxTarEntry(TarEntryType.RegularFile, name)
            {
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
            };

            this.tarWriter.WriteEntry(entry);
        }

        public void Dispose()
        {
            this.tarWriter.Dispose();
            this.gzipStream.Dispose();
        }
    }
}
