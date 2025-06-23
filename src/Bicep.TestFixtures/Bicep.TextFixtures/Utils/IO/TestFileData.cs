// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.TextFixtures.Utils.IO
{
    public readonly record struct TestFileData
    {
        public static readonly TestFileData Directory = new(true);

        private readonly string? text;

        public TestFileData(string text) => this.text = text;

        private TestFileData(bool _) => this.text = null;

        public static implicit operator TestFileData(string text) => new(text);

        public string Text => this.text ?? throw new InvalidOperationException("This TestFileData represents a directory, not a file.");

        public bool IsDirectory => this.text is null;
    }
}
