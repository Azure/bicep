// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
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

        public static Compilation CopyFilesAndCreateCompilation(this DataSet dataSet, TestContext testContext, out string outputDirectory, out Uri fileUri)
        {
            outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, new Workspace(), fileUri);

            return new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), sourceFileGrouping);
        }
    }
}

