// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.SourceGraph
{
    public static class ISourceFileExtensions
    {
        public static string GetFileName(this ISourceFile sourceFile) => sourceFile.FileHandle.Uri.PathSegments.Last();
    }
}
