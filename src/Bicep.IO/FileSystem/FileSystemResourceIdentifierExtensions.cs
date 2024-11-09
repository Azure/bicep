// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.FileSystem
{
    public static class FileSystemResourceIdentifierExtensions
    {
        public static string GetFileSystemPath(this ResourceIdentifier identifier) =>
            identifier.TryGetLocalFilePath() ?? throw new InvalidOperationException($"The file identifier '{identifier}' is not a valid local file path.");
    }
}
