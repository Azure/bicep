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
    public static class FileSystemIOUriExtensions
    {
        public static string GetFileSystemPath(this IOUri uri) =>
            uri.TryGetLocalFilePath() ?? throw new InvalidOperationException($"The URI '{uri}' is not a valid local file URI.");
    }
}
