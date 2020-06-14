using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class FileHelper
    {
        public static void SaveResultFile(TestContext testContext, string fileName, string contents)
        {
            string directory = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName);
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, contents);

            testContext.AddResultFile(filePath);
        }
    }
}
