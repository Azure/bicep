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

        public int SizeInBytes => this.data.Length;

        public int SizeInCharacters => this.SizeInBytes * 4;

        public Encoding? TryDetectEncodingFromByteOrderMarks()
        {
            var utf8NoBom = new UTF8Encoding(false);
            using var reader = new StreamReader(this.data.ToStream(), utf8NoBom ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            reader.Peek();

            return Equals(reader.CurrentEncoding, utf8NoBom) ? null : reader.CurrentEncoding;
        }

        public string ReadAllText(Encoding? encoding)
        {
            using var reader = new StreamReader(this.data.ToStream(), encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            return reader.ReadToEnd();
        }

        public ReadOnlySpan<byte> ReadAllBytes() => this.data;

        //public ResultWithDiagnosticBuilder<AuxiliaryFileData> TryRead(int sizeLimit, SizeLimitUnit sizeLimitUnit, Encoding? encoding = null)
        //{
        //    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeLimit);

        //    using var reader = new StreamReader(this.data.ToStream(), encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        //    var sizeLimitInCharacters = sizeLimitUnit == SizeLimitUnit.Characters ? sizeLimit : sizeLimit * 4;

        //    char[] buffer = new char[sizeLimitInCharacters];
        //    int lengthRead = reader.Read(buffer, 0, sizeLimitInCharacters);

        //    if (!reader.EndOfStream)
        //    {
        //        return new(x => x.FileExceedsMaximumSize(this.uri, sizeLimit, sizeLimitUnit));
        //    }

        //    var content = new string(buffer, 0, lengthRead);

        //    return new(new AuxiliaryFileData(content, reader.CurrentEncoding));
        //}

        //public readonly struct SizeLimitUnit
        //{
        //    public static readonly SizeLimitUnit Characters = new("characters");

        //    public static readonly SizeLimitUnit Bytes = new("bytes");

        //    private readonly string value;

        //    private SizeLimitUnit(string value)
        //    {
        //        this.value = value;
        //    }

        //    public static implicit operator string(SizeLimitUnit unit) => unit.value;
        //}
    }
}
