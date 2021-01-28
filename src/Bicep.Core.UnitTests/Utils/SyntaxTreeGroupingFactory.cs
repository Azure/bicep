// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SyntaxTreeGroupingFactory
    {
        public static SyntaxTreeGrouping CreateFromText(string text)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            return CreateForFiles(new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri);
        }

        public static SyntaxTreeGrouping CreateForFiles(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var workspace = new Workspace();
            var syntaxTrees = files.Select(kvp => SyntaxTree.Create(kvp.Key, kvp.Value));
            workspace.UpsertSyntaxTrees(syntaxTrees);

            return SyntaxTreeGroupingBuilder.Build(new FileResolver(), workspace, entryFileUri);
        }
    }
}