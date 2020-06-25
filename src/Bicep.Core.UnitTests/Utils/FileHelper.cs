using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class FileHelper
    {
        public static string GetResultFilePath(TestContext testContext, string fileName)
        {
            string directory = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName);
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            testContext.AddResultFile(filePath);

            return filePath;
        }

        public static void SaveResultFile(TestContext testContext, string fileName, string contents)
        {
            var filePath = GetResultFilePath(testContext, fileName);
            File.WriteAllText(filePath, contents);
        }
    }
}
