// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SourceFileGroupingFactory
    {
        public static SourceFileGrouping CreateFromText(string text, IFileResolver fileResolver)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            return CreateForFiles(new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri, fileResolver);
        }

        public static SourceFileGrouping CreateForFiles(IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri, IFileResolver fileResolver)
        {
            var workspace = new Workspace();
            var sourceFiles = fileContentsByUri.Select(kvp => SourceFileFactory.CreateSourceFile(kvp.Key, kvp.Value));
            workspace.UpsertSourceFiles(sourceFiles);

            return SourceFileGroupingBuilder.Build(fileResolver, workspace, entryFileUri);
        }
    }
}
