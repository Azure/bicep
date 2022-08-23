// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Baselines
{
    public record BaselineFile(
        TestContext TestContext,
        EmbeddedFile EmbeddedFile,
        string OutputFilePath)
    {
        public string ReadFromOutputFolder() => File.ReadAllText(OutputFilePath);

        public void WriteToOutputFolder(string contents) => File.WriteAllText(OutputFilePath, contents);

        public void ShouldHaveExpectedValue()
        {
            this.ReadFromOutputFolder().Should().EqualWithLineByLineDiffOutput(
                TestContext,
                EmbeddedFile.Contents,
                expectedLocation: EmbeddedFile.RelativeSourcePath,
                actualLocation: OutputFilePath);
        }

        public void ShouldHaveExpectedJsonValue()
        {
            JToken.Parse(this.ReadFromOutputFolder()).Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(EmbeddedFile.Contents),
                expectedLocation: EmbeddedFile.RelativeSourcePath,
                actualLocation: OutputFilePath);
        }
    }
}
