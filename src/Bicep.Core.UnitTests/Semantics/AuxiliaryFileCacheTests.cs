// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Semantics;

[TestClass]
public class AuxiliaryFileCacheTests
{
    [TestMethod]
    public void TryReadAsBinaryData_should_return_the_same_cached_result()
    {
        Uri fileUri = new("file:///path/to/file.txt");
        var binaryData = BinaryData.FromString("hello");

        var fileResolverMock = StrictMock.Of<IFileResolver>();
        fileResolverMock.Setup(x => x.TryReadAsBinaryData(fileUri, null))
            .Returns(new ResultWithDiagnostic<BinaryData>(binaryData));

        var sut = new AuxiliaryFileCache(fileResolverMock.Object);

        {
            for (var i = 0; i < 10; i++)
            {
                // attempt this a few times, to verify the file is only loaded once
                var result = sut.Read(fileUri).Unwrap();
                object.ReferenceEquals(binaryData, result.Content).Should().BeTrue();
                result.FileUri.Should().Be(fileUri);
            }

            fileResolverMock.Verify(x => x.TryReadAsBinaryData(fileUri, null), Times.Once);
        }
    }

    [TestMethod]
    public void TryReadAsBinaryData_caches_negative_results()
    {
        Uri fileUri = new("file:///path/to/file.txt");
        var binaryData = BinaryData.FromString("hello");

        var fileResolverMock = StrictMock.Of<IFileResolver>();
        fileResolverMock.Setup(x => x.TryReadAsBinaryData(fileUri, null))
            .Returns(new ResultWithDiagnostic<BinaryData>(x => x.FilePathInterpolationUnsupported()));

        var sut = new AuxiliaryFileCache(fileResolverMock.Object);

        {
            for (var i = 0; i < 10; i++)
            {
                // attempt this a few times, to verify the file is only loaded once
                sut.Read(fileUri).IsSuccess(out _, out var errorBuilder).Should().BeFalse();
                errorBuilder!(DiagnosticBuilder.ForDocumentStart()).Code.Should().Be("BCP092");
            }

            fileResolverMock.Verify(x => x.TryReadAsBinaryData(fileUri, null), Times.Once);
        }
    }

    [TestMethod]
    public void ClearEntries_should_clear_cached_results()
    {
        Uri fileUri = new("file:///path/to/file.txt");
        var binaryData = BinaryData.FromString("hello");

        var fileResolverMock = StrictMock.Of<IFileResolver>();
        fileResolverMock.Setup(x => x.TryReadAsBinaryData(fileUri, null))
            .Returns(new ResultWithDiagnostic<BinaryData>(binaryData));

        var sut = new AuxiliaryFileCache(fileResolverMock.Object);

        sut.Read(fileUri).Unwrap();
        sut.GetEntries().Should().BeEquivalentTo(new[] { fileUri });

        // clear a different file, verify the original is still cached
        sut.ClearEntries(new[] { new Uri("file:///different/file.txt") });
        sut.GetEntries().Should().BeEquivalentTo(new[] { fileUri });

        sut.ClearEntries(new[] { fileUri });
        sut.GetEntries().Should().BeEmpty();
    }

    [TestMethod]
    public void PruneInactiveEntries_should_clear_inactive_entries()
    {
        Uri fileUri = new("file:///path/to/file.txt");
        var binaryData = BinaryData.FromString("hello");

        var fileResolverMock = StrictMock.Of<IFileResolver>();
        fileResolverMock.Setup(x => x.TryReadAsBinaryData(fileUri, null))
            .Returns(new ResultWithDiagnostic<BinaryData>(binaryData));

        var sut = new AuxiliaryFileCache(fileResolverMock.Object);

        sut.Read(fileUri).Unwrap();
        sut.GetEntries().Should().BeEquivalentTo(new[] { fileUri });

        sut.PruneInactiveEntries(new[] { fileUri });
        sut.GetEntries().Should().BeEquivalentTo(new[] { fileUri });

        sut.PruneInactiveEntries(new[] { new Uri("file:///different/file.txt") });
        sut.GetEntries().Should().BeEmpty();
    }
}
