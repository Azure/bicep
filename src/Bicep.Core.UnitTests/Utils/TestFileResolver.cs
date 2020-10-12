// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using Bicep.Core.FileSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestFileResolver : IFileResolver
    {
        private readonly IReadOnlyDictionary<string, string> files;

        public TestFileResolver(IReadOnlyDictionary<string, string> files)
        {
            this.files = files;
        }

        public static IFileResolver CreateForSingleFile(string fileName, string fileContents)
            => new TestFileResolver(new Dictionary<string, string> { [fileName] = fileContents });

        public string GetNormalizedFileName(string fileName)
            => fileName.Replace('\\', '/');

        public string? TryRead(string fileName, out string? failureMessage)
        {
            if (!files.TryGetValue(fileName, out var fileContents))
            {
                failureMessage = $"Failed fo find file \"{fileName}\"";
                return null;
            }
            
            failureMessage = null;
            return fileContents;
        }

        public string? TryResolveModulePath(string childFileName, string parentFileName)
        {
            if (Path.IsPathRooted(childFileName))
            {
                return GetNormalizedFileName(childFileName);
            }

            var parentDirName = Path.GetDirectoryName(parentFileName) ?? throw new ArgumentException($"Unable to resolve parent directory from path \"{parentFileName}\"");
            return GetNormalizedFileName(Path.Combine(parentDirName, childFileName));
        }
    }
}