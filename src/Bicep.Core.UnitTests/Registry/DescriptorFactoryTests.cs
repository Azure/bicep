// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class DescriptorFactoryTests
    {
        [TestMethod]
        public void UnknownAlgorithmShouldThrow()
        {
            Action fail = () => DescriptorFactory.ComputeDigest("fake", Stream.Null);
            fail.Should().Throw<NotImplementedException>().WithMessage("Unknown hash algorithm 'fake'.");
        }

        [DataRow("sha256", "", "sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [DataRow("sha256", "Hello there!", "sha256:89b8b8e486421463d7e0f5caf60fb9cb35ce169b76e657ab21fc4d1d6b093603")]
        [DataRow("sha512","", "sha512:cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [DataRow("sha512", "Hello there!", "sha512:d0a1a241f4879b8fd8f9a2be55b004860f0e6f453ea8b42c8ad0e8cfc3721819dac6ec52f45b36044046b15cb8720874f701524aeac291921a865467781da456")]
        [DataTestMethod]
        public void ShouldComputeCorrectDigest(string algorithmIdentifier, string content, string expectedDigest)
        {
            using var stream = new MemoryStream();

            // make sure we write without BOM
            using var writer = new StreamWriter(stream, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);
            writer.Write(content);

            // the write is buffered, so we actually see it
            writer.Flush();

            // reset for reading
            stream.Position = 0;

            var actual = DescriptorFactory.ComputeDigest(algorithmIdentifier, stream);
            actual.Should().Be(expectedDigest);
        }
    }
}
