// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class SourceArchiveExtensions
    {
        public static SourceArchiveAssertions Should(this SourceArchive? SourceArchive) => new(SourceArchive);
    }

    public class SourceArchiveAssertions : ReferenceTypeAssertions<SourceArchive?, SourceArchiveAssertions>
    {
        public SourceArchiveAssertions(SourceArchive? SourceArchive) : base(SourceArchive)
        {
        }

        protected override string Identifier => "SourceArchive";

        public AndConstraint<SourceArchiveAssertions> BeEquivalentTo(SourceArchive archive)
        {
            using var _scope = new AssertionScope();

            Subject.Should().NotBeNull();

            Subject!.GetEntrypointPath().Should().Be(archive.GetEntrypointPath());
            Subject.GetSourceFiles().Select(entry => entry.Metadata.Path).Should().BeEquivalentTo(archive.GetSourceFiles().Select(entry => entry.Metadata.Path));
            Subject.GetMetadataFileContents().Should().Be(archive.GetMetadataFileContents());

            var ourFiles = Subject.GetSourceFiles().ToArray();
            var theirFiles = archive.GetSourceFiles().ToArray();

            ourFiles.Count().Should().Be(theirFiles.Count());

            for (int i = 0; i < ourFiles.Count(); ++i) {
                var ourFile = ourFiles[i];
                var theirFile = theirFiles[i];

                ourFile.Metadata.Path.Should().Be(theirFile.Metadata.Path);
                ourFile.Metadata.Kind.Should().Be(theirFile.Metadata.Kind);
                ourFile.Metadata.ArchivedPath.Should().Be(theirFile.Metadata.ArchivedPath);

                using var _scope2 = new AssertionScope($"archived path: {ourFile.Metadata.ArchivedPath}");
                ourFile.Contents.Should().Be(theirFile.Contents);
            }

            return new(this);
        }
    }
}
