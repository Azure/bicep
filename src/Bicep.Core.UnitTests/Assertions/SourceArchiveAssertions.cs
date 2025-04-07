// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using Bicep.Core.SourceLink;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using static Bicep.Core.SourceLink.SourceArchive;

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

        public AndConstraint<SourceArchiveAssertions> HaveData(BinaryData data)
        {
            using var _scope = new AssertionScope();

            Subject.Should().NotBeNull();

            var ourData = Decompress(Subject!.PackIntoBinaryData());
            var theirData = Decompress(data);

            ourData.Should().Equal(theirData);

            return new(this);
        }

        public AndConstraint<SourceArchiveAssertions> HaveSourceFiles(IEnumerable<LinkedSourceFile> sourceFiles)
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

        private static IReadOnlyDictionary<string, string> Decompress(BinaryData sourceArchiveData)
        {
            var fileEntries = new Dictionary<string, string>();
            var gz = new GZipStream(sourceArchiveData.ToStream(), CompressionMode.Decompress);
            using var tarReader = new TarReader(gz);

            while (tarReader.GetNextEntry() is { } entry)
            {
                string contents = entry.DataStream is null ? "" : BinaryData.FromStream(entry.DataStream).ToString();
                fileEntries.Add(entry.Name, contents);
            }

            return fileEntries;
        }
    }
}
