// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class ExecExtensionReferenceTests
    {
        // ── Parsing ────────────────────────────────────────────────────────────────

        [DataRow("/usr/local/bin/my-ext")]
        [DataRow("/path/to/my extension")]   // spaces are fine – it's an opaque string here
        [DataRow("my-extension")]            // PATH lookup
        [DataRow("my_ext.exe")]             // Windows-style name
        [DataTestMethod]
        public void TryParse_ValidRawValue_Succeeds(string rawValue)
        {
            var result = ExecExtensionReference.TryParse(
                BicepTestConstants.DummyBicepFile,
                rawValue,
                BicepTestConstants.FileExplorer);

            result.IsSuccess(out var reference, out _).Should().BeTrue();
            reference.Should().NotBeNull();
        }

        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void TryParse_EmptyOrWhitespaceRawValue_ReturnsBCP201(string rawValue)
        {
            var result = ExecExtensionReference.TryParse(
                BicepTestConstants.DummyBicepFile,
                rawValue,
                BicepTestConstants.FileExplorer);

            result.IsSuccess(out var reference, out var failureBuilder).Should().BeFalse();
            reference.Should().BeNull();
            failureBuilder!.Should().HaveCode("BCP201");
        }

        // ── Reference properties ─────────────────────────────────────────────────

        [TestMethod]
        public void FullyQualifiedReference_PrefixesExecScheme()
        {
            var rawValue = "/usr/local/bin/my-ext";
            var reference = Parse(rawValue);

            reference.FullyQualifiedReference.Should().Be($"exec:{rawValue}");
        }

        [TestMethod]
        public void UnqualifiedReference_EqualsRawValue()
        {
            var rawValue = "my-binary";
            var reference = Parse(rawValue);

            reference.UnqualifiedReference.Should().Be(rawValue);
        }

        [TestMethod]
        public void Scheme_IsExec()
        {
            var reference = Parse("my-binary");

            reference.Scheme.Should().Be(ArtifactReferenceSchemes.Exec);
        }

        [TestMethod]
        public void IsExternal_IsFalse()
        {
            // exec: binaries live on the local machine, not a remote registry.
            var reference = Parse("/usr/local/bin/my-ext");

            reference.IsExternal.Should().BeFalse();
        }

        // ── Equality ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void SameRawValues_AreEqual()
        {
            var first = Parse("/usr/local/bin/my-ext");
            var second = Parse("/usr/local/bin/my-ext");

            first.Equals(second).Should().BeTrue();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [TestMethod]
        public void DifferentRawValues_AreNotEqual()
        {
            var first = Parse("/usr/local/bin/my-ext");
            var second = Parse("my-ext");

            first.Equals(second).Should().BeFalse();
        }

        [TestMethod]
        public void RawValueComparison_IsCaseSensitive()
        {
            var lower = Parse("my-ext");
            var upper = Parse("My-Ext");

            lower.Equals(upper).Should().BeFalse();
        }

        // ── Helper ───────────────────────────────────────────────────────────────

        private static ExecExtensionReference Parse(string rawValue)
        {
            var result = ExecExtensionReference.TryParse(
                BicepTestConstants.DummyBicepFile,
                rawValue,
                BicepTestConstants.FileExplorer);

            result.IsSuccess(out var reference, out var failureBuilder).Should().BeTrue(
                $"'{rawValue}' should parse successfully but got a failure builder");
            return reference!;
        }
    }
}
