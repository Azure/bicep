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
    public sealed class TgzReader : IDisposable
    {
        private readonly TarReader tarReader;
        private readonly GZipStream gzipStream;

        public TgzReader(Stream stream)
        {
            this.gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            this.tarReader = new TarReader(gzipStream);
        }

        public (string Name, string Contents)? GetNextEntry()
        {
            if (this.tarReader.GetNextEntry() is { } tarEntry)
            {
                string contents = tarEntry.DataStream is not null ? new StreamReader(tarEntry.DataStream).ReadToEnd() : "";

                return (tarEntry.Name, contents);
            }

            return null;
        }

        public void Dispose()
        {
            this.tarReader.Dispose();
            this.gzipStream.Dispose();
        }
    }
}
