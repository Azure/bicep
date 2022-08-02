// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Baselines
{
    public record EmbeddedFile(
        Assembly Assembly,
        string StreamPath)
    {
        private readonly Lazy<string> contentsLazy = new(() => {
            var stream = Assembly.GetManifestResourceStream(StreamPath);

            return new StreamReader(stream!).ReadToEnd();
        });

        public string Contents => contentsLazy.Value;

        public string FileName => Path.GetFileName(StreamPath);

        public string RelativeSourcePath => Path.Combine("src", Assembly.GetName().Name!, StreamPath);

        public static IEnumerable<EmbeddedFile> LoadAll(Assembly assembly, string streamPathPrefix, Func<string, bool> shouldLoad)
        {
            // Set the convention that all embedded resource files are in a folder named "Files"
            var combinedPathPrefix = $"Files/{streamPathPrefix}/";

            foreach (var streamName in assembly.GetManifestResourceNames()
                .Where(p => p.StartsWith(combinedPathPrefix, StringComparison.Ordinal)))
            {
                if (shouldLoad(streamName))
                {
                    yield return new(assembly, streamName);
                }
            }
        }

        public override string ToString() => StreamPath;
    }
}
