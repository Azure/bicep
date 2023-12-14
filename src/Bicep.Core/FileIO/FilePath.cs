// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.FileIO
{
    public readonly struct FilePath
    {
        private FilePath(string value)
        {
            Value = value;
        }

        public static FilePath Create(string value, bool normalize = true)
        {
            if (normalize)
            {
                value = value.Replace("\\", "/");

                if (value.Length >= 2 && char.IsAsciiLetter(value[0]) && value[1] == ':')
                {
                    // Normalize Windows drive letter
                    value = char.ToLowerInvariant(value[0]) + value[1..];
                }
            }

            return new(value);
        }

        public string Value { get; }

        public static implicit operator string(FilePath filePath) => filePath.Value;

        public override string ToString() => this.Value;

        public ReadOnlySpan<char> AsSpan() => this.Value.AsSpan();
    }
}
