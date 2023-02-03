// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OciArtifactModuleReferenceHelper
    {
        public static OciArtifactModuleReference GetModuleReferenceAndSaveManifestFile(
            TestContext testContext,
            string registry,
            string repository,
            string manifestFileContents,
            string testOutputPath,
            Uri parentModuleUri,
            string? digest,
            string? tag)
        {
            string? manifestFilePath = null;

            if (digest is not null)
            {
                manifestFilePath = Path.Combine(testOutputPath, "br", registry, repository.Replace("/", "$"), digest.Replace(":", "#"));
            }
            else if (tag is not null)
            {
                manifestFilePath = Path.Combine(testOutputPath, "br", registry, repository.Replace("/", "$"), tag + "$");
            }

            if (!string.IsNullOrWhiteSpace(manifestFilePath))
            {
                FileHelper.SaveResultFile(testContext, "manifest", manifestFileContents, manifestFilePath);
            }

            return new OciArtifactModuleReference(registry, repository, tag, digest, parentModuleUri);
        }
    }
}
