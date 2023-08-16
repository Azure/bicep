// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Mock;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OciArtifactModuleReferenceHelper
    {
        public static OciModuleReference GetModuleReferenceAndSaveManifestFile(
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

            var artifactReferenceMock = StrictMock.Of<IOciArtifactReference>();
            artifactReferenceMock.SetupGet(m => m.Registry).Returns(registry);
            artifactReferenceMock.SetupGet(m => m.Repository).Returns(repository);
            artifactReferenceMock.SetupGet(m => m.Digest).Returns(digest);
            artifactReferenceMock.SetupGet(m => m.Tag).Returns(tag);
            artifactReferenceMock.SetupGet(m => m.ArtifactId).Returns($"{registry}/{repository}:{tag ?? digest}");

            return new OciModuleReference(artifactReferenceMock.Object, parentModuleUri);
        }
    }
}
