// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Core.SourceCode;
using Bicep.Core.Workspaces;
using static Bicep.Core.SourceCode.SourceArchive;

namespace Bicep.Core.UnitTests.Utils
{
    public class SourceArchiveBuilder
    {
#if WINDOWS_BUILD
        private static string Rooted(string path) => $"c:\\{path}";
#else
        private static string Rooted(string path) => $"/{path}";
#endif

        private string? cacheRoot = null;
        private Uri entrypointUri = PathHelper.FilePathToFileUrl(Rooted("foo/bar/entrypoint.bicep"));
        private string entrypointBicepContents = "metadata hi = 'This is the bicep entrypoint source file'";

        public SourceArchiveBuilder WithCacheRoot(string cacheRoot)
        {
            this.cacheRoot = cacheRoot;
            return this;
        }

        public SourceArchiveBuilder WithEntrypointFile(Uri entrypoint, string entrypointBicepContents)
        {
            this.entrypointUri = entrypoint;
            this.entrypointBicepContents = entrypointBicepContents;
            return this;
        }

        public SourceArchiveBuilder WithEntrypointFile(string entrypoint, string entrypointBicepContents)
        {
            this.entrypointUri = PathHelper.FilePathToFileUrl(Rooted(entrypoint));
            this.entrypointBicepContents = entrypointBicepContents;
            return this;
        }

        public SourceArchive Build()
        {
            var stream = BuildStream();
            return SourceArchive.UnpackFromStream(stream).UnwrapOrThrow();
        }

        public Stream BuildStream()
        {
            return SourceArchive.PackSourcesIntoStream(
                entrypointUri,
                cacheRoot,
                new SourceFileWithArtifactReference[] {
                    new SourceFileWithArtifactReference(
                        SourceFileFactory.CreateBicepFile(entrypointUri, entrypointBicepContents),
                    null)});
        }

        public BinaryData BuildBinaryData()
        {
            return BinaryData.FromStream(BuildStream());
        }
    }
}

