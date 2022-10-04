// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.FileSystem;

namespace Bicep.Wasm
{
    public class InMemoryFileResolver : FileResolver
    {
        public InMemoryFileResolver(IReadOnlyDictionary<Uri, string> fileLookup)
            : base(new MockFileSystem(fileLookup.ToDictionary(x => x.Key.LocalPath, x => new MockFileData(x.Value))))
        {
        }
    }
}
