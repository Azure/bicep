// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;

namespace Bicep.TextFixtures.Utils.IO
{
    public class InMemoryTestFileSet : TestFileSet
    {
        public InMemoryTestFileSet()
            : base(new InMemoryFileExplorer())
        {
        }

        public static TestFileSet Create(params (string, TestFileData)[] files) => new InMemoryTestFileSet().AddFiles(files);

        public override IOUri GetUri(string path) => TestFileUri.FromInMemoryPath(path);
    }
}
