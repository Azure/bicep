// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.FileIO.Abstractions
{
    public readonly struct FilePointer
    {
        public FilePointer(string value, FilePointerKind kind)
        {
            this.Value = value;
            this.Kind = kind;
        }

        public string Value { get; }

        public FilePointerKind Kind { get; }

        public bool IsAbsolute => this.Kind is FilePointerKind.Absolute;

        public bool IsRelative => this.Kind is FilePointerKind.Relative;

        public static implicit operator string(FilePointer fileId) => fileId.Value;

        public static implicit operator ReadOnlySpan<char>(FilePointer fileId) => fileId.AsSpan();

        public override string ToString() => Value;

        public ReadOnlySpan<char> AsSpan() => Value.AsSpan();
    }
}
