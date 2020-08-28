// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class FileHelper
    {
        public static string GetResultFilePath(TestContext testContext, string fileName)
        {
            string filePath = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            testContext.AddResultFile(filePath);

            return filePath;
        }

        public static string SaveResultFile(TestContext testContext, string fileName, string contents)
        {
            var filePath = GetResultFilePath(testContext, fileName);
            File.WriteAllText(filePath, contents);

            return filePath;
        }
    }
}

