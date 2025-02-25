// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.SourceCode;
using Bicep.Core.Workspaces;
using FluentAssertions;
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

        private readonly ISourceFileFactory sourceFileFactory;

        // First will be entrypoint, must be a bicep file
        private List<ISourceFile> SourceFiles = new();

        private ISourceFile EntrypointFile => SourceFiles[0];

        public SourceArchiveBuilder(ISourceFileFactory sourceFileFactory)
        {
            this.sourceFileFactory = sourceFileFactory;
        }

        public SourceArchiveBuilder WithBicepFile(Uri fileUri, string contents)
        {
            SourceFiles.Add(this.sourceFileFactory.CreateBicepFile(fileUri, contents));
            return this;
        }

        public SourceArchiveBuilder WithBicepFile(string entrypoint, string contents)
        {
            WithBicepFile(PathHelper.FilePathToFileUrl(Rooted(entrypoint)), contents);
            return this;
        }

        public SourceArchive Build()
        {
            var stream = BuildStream();
            return SourceArchive.UnpackFromStream(stream).UnwrapOrThrow();
        }

        public Stream BuildStream()
        {
            if (SourceFiles.Count == 0)
            {
                // Add a default entrypoint
                WithBicepFile("foo/bar/entrypoint.bicep", "metadata hi = 'This is the bicep entrypoint source file'");
            }

            SourceFiles[0].Should().BeOfType<BicepFile>("Entrypoint should be a bicep file");

            return SourceArchive.PackSourcesIntoStream(
                EntrypointFile.Uri,
                null,
                [.. SourceFiles.Select(x => new SourceFileWithArtifactReference(x, null))]);
        }

        public BinaryData BuildBinaryData()
        {
            return BinaryData.FromStream(BuildStream());
        }
    }
}

