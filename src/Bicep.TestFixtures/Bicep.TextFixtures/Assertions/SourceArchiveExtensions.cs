// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bicep.Core.SourceLink;
using Bicep.IO.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bicep.TextFixtures.Assertions
{
    public static class SourceArchiveExtensions
    {
        public static SourceArchiveAssertions Should(this SourceArchive? SourceArchive) => new(SourceArchive);
    }

    public class SourceArchiveAssertions : ReferenceTypeAssertions<SourceArchive?, SourceArchiveAssertions>
    {
        public SourceArchiveAssertions(SourceArchive? SourceArchive)
            : base(SourceArchive)
        {
        }

        protected override string Identifier => "SourceArchive";

        public AndConstraint<SourceArchiveAssertions> BeEquivalentTo(SourceArchive other)
        {
            var subjectData = this.Subject!.PackIntoBinaryData();
            var otherData = other.PackIntoBinaryData();

            var subjectEntries = Decompress(subjectData);
            var otherEntries = Decompress(otherData);

            subjectEntries.Should().Equal(otherEntries);

            return new(this);
        }

        public AndConstraint<SourceArchiveAssertions> HaveMetadataAndFiles(string expectedMetadataJson, params (string, string)[] expectedFileEntries)
        {
            using (new AssertionScope())
            {
                this.Subject.Should().NotBeNull();

                var tgzEntries = Decompress(this.Subject!.PackIntoBinaryData());

                tgzEntries.TryGetValue(SourceArchiveConstants.MetadataFileName, out var metadataJson).Should().BeTrue();

                var expectedMetadata = JsonSerializer.Deserialize(expectedMetadataJson, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata);
                expectedMetadata.Should().NotBeNull();

                var actualMetadata = JsonSerializer.Deserialize(metadataJson!, SourceArchiveMetadataSerializationContext.Default.SourceArchiveMetadata);
                actualMetadata.Should().NotBeNull();

                actualMetadata!.MetadataVersion.Should().Be(expectedMetadata!.MetadataVersion);
                actualMetadata.EntryPoint.Should().Be(expectedMetadata.EntryPoint);
                actualMetadata.SourceFiles.Should().BeEquivalentTo(expectedMetadata.SourceFiles);
                actualMetadata.DocumentLinks.Keys.Should().BeEquivalentTo(expectedMetadata.DocumentLinks.Keys);

                foreach (var (path, expectedLinks) in expectedMetadata.DocumentLinks)
                {
                    var actualLinks = actualMetadata.DocumentLinks[path];
                    actualLinks.Should().BeEquivalentTo(expectedLinks);
                }

                tgzEntries.Remove(SourceArchiveConstants.MetadataFileName);
                tgzEntries.ToDictionary(x => x.Key, x => x.Value.ReplaceLineEndings())
                    .Should()
                    .BeEquivalentTo(expectedFileEntries.ToDictionary(x => x.Item1, x => x.Item2.ReplaceLineEndings()));

                return new AndConstraint<SourceArchiveAssertions>(this);

            }
        }

        public AndConstraint<SourceArchiveAssertions> HaveData(BinaryData data)
        {
            using var _scope = new AssertionScope();

            Subject.Should().NotBeNull();

            var ourData = Decompress(Subject!.PackIntoBinaryData());
            var theirData = Decompress(data);

            ourData.Should().Equal(theirData);

            return new(this);
        }

        public AndConstraint<SourceArchiveAssertions> HaveSourceFiles(IEnumerable<ArchivedSourceFile> sourceFiles)
        {
            using (new AssertionScope())
            {
                foreach (var sourceFile in sourceFiles)
                {
                    Subject!.FindSourceFile(sourceFile.Metadata.Path).Should().BeEquivalentTo(sourceFile);
                }
            }

            return new(this);
        }

        private static Dictionary<string, string> Decompress(BinaryData sourceArchiveData)
        {
            var fileEntries = new Dictionary<string, string>();
            using var tgzReader = new TgzReader(sourceArchiveData.ToStream());

            while (tgzReader.GetNextEntry() is { } entry)
            {
                string contents = entry.Data.ToString();
                fileEntries.Add(entry.Name, contents);
            }

            return fileEntries;
        }
    }
}
