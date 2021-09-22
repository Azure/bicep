// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class TagEncoderTests
    {
        [DataRow("", "$")]
        [DataRow("v1", "v1$")]
        [DataRow("V1", "V1$1")]
        [DataRow("AbCd", "AbCd$5")]
        [DataRow("ABCD", "ABCD$f")]
        [DataRow("ABCDABCDABCDABCD", "ABCDABCDABCDABCD$ffff")]
        [DataRow("ABCDABCDABCDABCDAB", "ABCDABCDABCDABCDAB$3ffff")]
        [DataRow("ABCDABCDABCDABCDABCD", "ABCDABCDABCDABCDABCD$fffff")]
        [DataRow("ABCDABCDABCDABCDABCDAB", "ABCDABCDABCDABCDABCDAB$3fffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCD", "ABCDABCDABCDABCDABCDABCD$ffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDAB", "ABCDABCDABCDABCDABCDABCDAB$3ffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDABCD", "ABCDABCDABCDABCDABCDABCDABCD$fffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDABCDAB", "ABCDABCDABCDABCDABCDABCDABCDAB$3fffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDABCDABCD", "ABCDABCDABCDABCDABCDABCDABCDABCD$ffffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDABCDABCDA", "ABCDABCDABCDABCDABCDABCDABCDABCDA$1ffffffff")]
        [DataRow("ABCDABCDABCDABCDABCDABCDABCDABCDAbC", "ABCDABCDABCDABCDABCDABCDABCDABCDAbC$5ffffffff")]
        [DataRow(OciArtifactModuleReferenceTests.ExampleTagOfMaxLength, OciArtifactModuleReferenceTests.ExampleTagOfMaxLength + "$ffffff800000000000")]
        [DataTestMethod]
        public void EncoderShouldProduceExpectedOutut(string tag, string expected) => TagEncoder.Encode(tag).Should().Be(expected);

        [TestMethod]
        public void EncoderShouldThrowWhenMaxTagLengthIsExceeded() =>
            FluentActions
                .Invoking(() => TagEncoder.Encode(OciArtifactModuleReferenceTests.ExampleTagOfMaxLength + 'A'))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The specified tag '*' exceeds max length of 128.");

        [TestMethod]
        public void EncodingFullyCapitalizedStringOfMaxLengthShouldNotExceedMaxLinuxFileNameLength()
        {
            var fullyCapitalizedTag = new string('A', OciArtifactModuleReference.MaxTagLength);
            var encoded = TagEncoder.Encode(fullyCapitalizedTag);

            encoded.Length.Should().BeLessOrEqualTo(255);
            encoded.Should().EndWith("$ffffffffffffffffffffffffffffffff");
        }
    }
}
