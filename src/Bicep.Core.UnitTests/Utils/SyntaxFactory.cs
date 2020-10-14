// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SyntaxFactory
    {
        public static SyntaxTreeGrouping CreateFromText(string text)
        {
            var entryFileName = "/main.bicep";

            return CreateForFiles(new Dictionary<string, string> { [entryFileName] = text }, entryFileName);
        }

        public static SyntaxTreeGrouping CreateForFiles(IReadOnlyDictionary<string, string> files, string entryFileName)
        {
            var fileResolver = new InMemoryFileResolver(files);

            return SyntaxTreeGroupingBuilder.Build(fileResolver, entryFileName);
        }
    }
}