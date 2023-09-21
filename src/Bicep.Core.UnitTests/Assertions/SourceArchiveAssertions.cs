// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceCode;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
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

            Subject!.EntrypointPath.Should().Be(archive.EntrypointPath);
            Subject.SourceFiles.Select(entry => entry.Path).Should().BeEquivalentTo(archive.SourceFiles.Select(entry => entry.Path));

            var ourFiles = Subject.SourceFiles.ToArray();
            var theirFiles = archive.SourceFiles.ToArray();

            ourFiles.Count().Should().Be(theirFiles.Count());

            for (int i = 0; i < ourFiles.Count(); ++i) {
                var ourFile = ourFiles[i];
                var theirFile = theirFiles[i];

                ourFile.Path.Should().Be(theirFile.Path);
                ourFile.Kind.Should().Be(theirFile.Kind);
                ourFile.ArchivePath.Should().Be(theirFile.ArchivePath);

                using var _scope2 = new AssertionScope($"archived path: {ourFile.ArchivePath}");
                ourFile.Contents.Should().Be(theirFile.Contents);
            }

            return new(this);
        }
    }
}
