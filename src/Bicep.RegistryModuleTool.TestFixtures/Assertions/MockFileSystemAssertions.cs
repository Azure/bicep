// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.TestFixtures.Assertions
{
    public static class MockFileSystemExtensions
    {
        public static MockFileSystemAssertions Should(this MockFileSystem fileSystem) => new(fileSystem);
    }

    public class MockFileSystemAssertions : ReferenceTypeAssertions<MockFileSystem, MockFileSystemAssertions>
    {
        public MockFileSystemAssertions(MockFileSystem subject)
            : base(subject)
        {
        }

        protected override string Identifier => nameof(MockFileSystem);

        public AndConstraint<MockFileSystemAssertions> HaveSameFilesAs(MockFileSystem expected)
        {
            this.Subject.AllFiles.Should().BeEquivalentTo(expected.AllFiles);

            using (new AssertionScope())
            {
                foreach (var path in this.Subject.AllFiles)
                {
                    var fileContents = this.Subject.GetFile(path).TextContents.ReplaceLineEndings();
                    var expectedFileContents = expected.GetFile(path).TextContents.ReplaceLineEndings();

                    fileContents.Should().Be(expectedFileContents);
                }
            }

            return new AndConstraint<MockFileSystemAssertions>(this);
        }
    }
}
