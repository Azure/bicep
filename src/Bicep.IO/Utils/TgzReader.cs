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

        public (string Name, BinaryData Data)? GetNextEntry()
        {
            if (this.tarReader.GetNextEntry() is { } tarEntry)
            {
                var data = tarEntry.DataStream is not null ? BinaryData.FromStream(tarEntry.DataStream) : BinaryData.Empty;

                return (tarEntry.Name, data);
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
