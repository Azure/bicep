// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Baselines
{
    public record BaselineFile(
        TestContext TestContext,
        EmbeddedFile EmbeddedFile,
        string OutputFilePath)
    {

        private readonly static JsonSerializerOptions StjSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public string ReadFromOutputFolder() => File.ReadAllText(OutputFilePath);

        public void WriteToOutputFolder(string contents) => File.WriteAllText(OutputFilePath, contents);

        public void WriteJsonToOutputFolder<T>(T contents) => WriteToOutputFolder(JsonConvert.SerializeObject(contents, Formatting.Indented));

        public void WriteStjJsonToOutputFolder<T>(T contents) => WriteToOutputFolder(System.Text.Json.JsonSerializer.Serialize(contents, StjSerializerOptions));

        public Uri OutputFileUri => PathHelper.FilePathToFileUrl(OutputFilePath);

        public void ShouldHaveExpectedValue()
        {
            this.ReadFromOutputFolder().Should().EqualWithLineByLineDiffOutput(
                TestContext,
                EmbeddedFile.Contents,
                expectedPath: EmbeddedFile.RelativeSourcePath,
                actualPath: OutputFilePath);
        }

        public void ShouldHaveExpectedJsonValue()
        {
            this.ReadFromOutputFolder().FromJson<JToken>().Should().EqualWithJsonDiffOutput(
                TestContext,
                EmbeddedFile.Contents.TryFromJson<JToken>() ?? JValue.CreateNull(),
                expectedLocation: EmbeddedFile.RelativeSourcePath,
                actualLocation: OutputFilePath);
        }
    }
}
