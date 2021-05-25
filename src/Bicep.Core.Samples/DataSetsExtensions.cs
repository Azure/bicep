// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Bicep.Types.Az;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Samples
{
    public static class DataSetsExtensions
    {
        public static IEnumerable<object[]> ToDynamicTestData(this IEnumerable<DataSet> source) => source.Select(ToDynamicTestData);

        public static object[] ToDynamicTestData(this DataSet ds) => new object[] {ds};

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF",  StringComparison.Ordinal);
            
        public static string SaveFilesToTestDirectory(this DataSet dataSet, TestContext testContext)
            => FileHelper.SaveEmbeddedResourcesWithPathPrefix(testContext, typeof(DataSet).Assembly, dataSet.GetStreamPrefix());

        public static Compilation CopyFilesAndCreateCompilation(this DataSet dataSet, TestContext testContext, out string outputDirectory)
        {
            outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), fileUri);

            return new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), syntaxTreeGrouping);
        }
    }
}

