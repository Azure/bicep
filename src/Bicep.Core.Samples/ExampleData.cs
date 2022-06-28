// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;

namespace Bicep.Core.Samples
{
    public record ExampleData(
        string BicepStreamName,
        string JsonStreamName,
        string SymbolicNamesJsonStreamName,
        string OutputFolderName,
        bool IsExtensibilitySample)
    {
        public static string GetDisplayName(MethodInfo info, object[] data)
            => $"{info.Name}_{((ExampleData)data[0]).BicepStreamName}";

        public static IEnumerable<ExampleData> GetAllExampleData()
        {
            const string pathPrefix = "user_submitted/";
            const string bicepExtension = ".bicep";

            foreach (var streamName in typeof(ExampleData).Assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal)))
            {
                var extension = Path.GetExtension(streamName);
                if (!StringComparer.OrdinalIgnoreCase.Equals(extension, bicepExtension))
                {
                    continue;
                }

                var outputFolderName = streamName
                    .Substring(0, streamName.Length - bicepExtension.Length)
                    .Substring(pathPrefix.Length)
                    .Replace('/', '_');

                var isExtensibilitySample = streamName.StartsWith($"{pathPrefix}extensibility", StringComparison.Ordinal);
                var jsonStreamName = Path.ChangeExtension(streamName, "json");
                var symbolicNamesJsonStreamName = Path.ChangeExtension(streamName, "symbolicnames.json");

                yield return new ExampleData(
                    BicepStreamName: streamName,
                    JsonStreamName: jsonStreamName,
                    SymbolicNamesJsonStreamName: symbolicNamesJsonStreamName,
                    OutputFolderName: outputFolderName,
                    IsExtensibilitySample: isExtensibilitySample);
            }
        }

        public static string GetBaselineUpdatePath(string streamName)
            => Path.Combine("src", "Bicep.Core.Samples", streamName);
    }
}
