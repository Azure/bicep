// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Formats.Tar;
using System.IO.Compression;
using Bicep.Core.Registry.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ExtensionV1ArchiveTests
    {
        [TestMethod]
        public void Read_NonArchiveContent_Throws()
        {
            var binaryData = BinaryData.FromString("this is not a valid tgz archive");

            Action fail = () => ExtensionV1Archive.Read(binaryData);

            fail.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Read_ArchiveMissingTypesEntry_ThrowsDescriptiveError()
        {
            var binaryData = BuildGzippedTar(("not-types.txt", BinaryData.FromString("hello")));

            Action fail = () => ExtensionV1Archive.Read(binaryData);

            fail.Should().Throw<InvalidOperationException>()
                .WithMessage("The extension package does not contain a 'types.tgz' entry.");
        }

        private static BinaryData BuildGzippedTar(params (string ArchivePath, BinaryData Data)[] entries)
        {
            using var stream = new MemoryStream();

            using (var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                using var tarWriter = new TarWriter(gzStream, leaveOpen: true);

                foreach (var (archivePath, data) in entries)
                {
                    tarWriter.WriteEntry(new PaxTarEntry(TarEntryType.RegularFile, archivePath)
                    {
                        DataStream = data.ToStream(),
                    });
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            return BinaryData.FromStream(stream);
        }
    }
}
