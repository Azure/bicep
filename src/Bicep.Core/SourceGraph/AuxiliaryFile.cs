// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public record AuxiliaryFile
    {
        private readonly BinaryData data;

        public AuxiliaryFile(IOUri uri, BinaryData data)
        {
            this.Uri = uri;
            this.data = data;
        }

        public IOUri Uri { get; }

        public Encoding? TryDetectEncodingFromByteOrderMarks()
        {
            var utf8NoBom = new UTF8Encoding(false);
            using var reader = new StreamReader(this.data.ToStream(), utf8NoBom ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            reader.Peek();

            return Equals(reader.CurrentEncoding, utf8NoBom) ? null : reader.CurrentEncoding;
        }

        public ResultWithDiagnosticBuilder<string> TryReadText(Encoding? encoding, int? charactersLimit)
        {
            using var reader = new StreamReader(this.data.ToStream(), encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            if (charactersLimit is null)
            {
                return new(reader.ReadToEnd());
            }

            char[] buffer = new char[charactersLimit.Value];
            int charactersRead = reader.ReadBlock(buffer, 0, charactersLimit.Value);

            if (charactersRead == charactersLimit && !reader.EndOfStream)
            {
                return new(x => x.FileExceedsMaximumSize(this.Uri, charactersLimit.Value, "characters"));
            }

            return new(new string(buffer, 0, charactersRead));
        }

        public ResultWithDiagnosticBuilder<ReadOnlyMemory<byte>> TryReadBytes(int bytesLimit)
        {
            if (this.data.Length > bytesLimit)
            {
                return new(x => x.FileExceedsMaximumSize(this.Uri, bytesLimit, "bytes"));
            }

            return new(this.data);
        }
    }
}
