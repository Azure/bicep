// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Testing.Baselines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing;

public static class TestContextExtensions
{
    public static BaselineFileSet MaterializeBaseline(this TestContext testContext, TestEmbeddedFile embeddedFile)
        => BaselineFileSet.Materialize(testContext, embeddedFile);

    public static string GetUniqueOutputPath(this TestContext testContext)
        => Path.Combine(testContext.ResultsDirectory!, Guid.NewGuid().ToString());

    public static string GetResultFilePath(this TestContext testContext, string fileName, string? outputPath = null)
    {
        var filePath = Path.Combine(outputPath ?? testContext.GetUniqueOutputPath(), fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new AssertFailedException($"There is no directory path for file '{filePath}'."));
        testContext.AddResultFile(filePath);

        return filePath;
    }

    public static string SaveResultFile(this TestContext testContext, string fileName, string contents, string? outputPath = null, Encoding? encoding = null)
    {
        var resultPath = testContext.SaveResultFiles([new(fileName, contents, encoding)], outputPath);

        return Path.Combine(resultPath, fileName);
    }

    public static string SaveResultFiles(this TestContext testContext, TestResultFile[] files, string? outputPath = null)
    {
        outputPath ??= testContext.GetUniqueOutputPath();

        foreach (var (fileName, contents, encoding) in files)
        {
            var filePath = testContext.GetResultFilePath(fileName, outputPath);
            if (encoding is null)
            {
                File.WriteAllText(filePath, contents);
            }
            else
            {
                File.WriteAllText(filePath, contents, encoding);
            }
        }

        return outputPath;
    }
}
