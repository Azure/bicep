// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Workspaces;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SourceFileGroupingFactory
    {
        public static SourceFileGrouping CreateFromText(ServiceBuilder services, string text)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            return CreateForFiles(services, new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri);
        }

        public static SourceFileGrouping CreateForFiles(ServiceBuilder services, IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri)
            => services.WithWorkspaceFiles(fileContentsByUri).SourceFileGrouping.Build(entryFileUri);
    }
}
