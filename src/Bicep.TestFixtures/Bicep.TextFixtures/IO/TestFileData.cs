// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace Bicep.TextFixtures.IO
{
    public record TestFileData
    {
        public static readonly TestFileData Directory = new(true);

        private readonly BinaryData? data;

        public TestFileData(BinaryData data) => this.data = data;

        public TestFileData(string text) => this.data = BinaryData.FromString(text);

        public TestFileData(string text, Encoding encoding) => this.data = BinaryData.FromBytes([.. encoding.GetPreamble(), .. encoding.GetBytes(text)]);

        private TestFileData(bool _) => this.data = null;

        public static implicit operator TestFileData(string text) => new(text);

        public static implicit operator TestFileData(BinaryData data) => new(data);

        public bool IsDirectory => this.data is null;

        public BinaryData AsBinaryData() => this.data ?? throw new InvalidOperationException("This TestFileData represents a directory, not a file.");
    }
}
