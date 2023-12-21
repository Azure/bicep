// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.FileIO.Abstractions;

namespace Bicep.Core.FileIO.Extensions
{
    public static class IFileStoreExtensions
    {
        public static FilePointer Combine(this IFileStore fileStore, FilePointer basePointer, string relativePointer) =>
            fileStore.Combine(basePointer, fileStore.Parse(relativePointer));
    }
}
