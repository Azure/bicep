// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class DiagnosticTreeTests
    {
        private static readonly DiagnosticTree Sut = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {

            WriteDummyDiagnostic(0, 4, "aaaa");
            WriteDummyDiagnostic(5, 5, "bbbb");
            WriteDummyDiagnostic(5, 10, "cccc");
            WriteDummyDiagnostic(10, 10, "dddd");
            WriteDummyDiagnostic(20, 30, "eeee");
            WriteDummyDiagnostic(22, 25, "ffff");
            WriteDummyDiagnostic(120, 200, "gggg");
            WriteDummyDiagnostic(201, 210, "hhhh");
        }

        [TestMethod]
        [DataRow(0, 4)]
        [DataRow(1, 5)]
        [DataRow(5, 5)]
        [DataRow(5, 10)]
        [DataRow(10, 10)]
        [DataRow(20, 30)]
        [DataRow(22, 25)]
        [DataRow(120, 200)]
        [DataRow(201, 210)]
        [DataRow(0, 5)]
        [DataRow(0, 40)]
        [DataRow(4, 10)]
        [DataRow(4, 11)]
        [DataRow(5, 11)]
        [DataRow(21, 25)]
        [DataRow(22, 26)]
        [DataRow(21, 26)]
        [DataRow(120, 201)]
        [DataRow(200, 210)]
        [DataRow(0, 1000)]
        public void Contains_EnclosingSpan_ReturnsTrue(int start, int end)
        {
            var span = new TextSpan(start, end - start);

            var result = Sut.Contains(span);

            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(1, 2)]
        [DataRow(6, 9)]
        [DataRow(21, 24)]
        [DataRow(23, 25)]
        [DataRow(100, 199)]
        [DataRow(210, 210)]
        [DataRow(300, 400)]
        public void Contains_NotEnclosingSpan_ReturnsFalse(int start, int end)
        {
            var span = new TextSpan(start, end - start);

            var result = Sut.Contains(span);

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(1, 2)]
        [DataRow(6, 9)]
        [DataRow(21, 24)]
        [DataRow(23, 25)]
        [DataRow(100, 199)]
        [DataRow(210, 210)]
        [DataRow(300, 400)]
        public void Indexer_NotEnclosingSpan_ReturnsEmptyEnumerable(int start, int end)
        {
            var span = new TextSpan(start, end - start);

            var diagnostics = Sut[span];

            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(0, 4, new[] { "aaaa" })]
        [DataRow(5, 5, new[] { "bbbb" })]
        [DataRow(5, 10, new[] { "bbbb", "cccc", "dddd" })]
        [DataRow(10, 10, new[] { "dddd" })]
        [DataRow(22, 25, new[] { "ffff" })]
        [DataRow(21, 26, new[] { "ffff" })]
        [DataRow(8, 205, new[] { "dddd", "eeee", "ffff", "gggg" })]
        [DataRow(0, 1000, new[] { "aaaa", "bbbb", "cccc", "dddd", "eeee", "ffff", "gggg", "hhhh" })]
        public void Indexer_EnclosingSpan_ReturnsEnclosedDiagnostics(int start, int end, string[] expectedDiagnosticCodes)
        {
            var span = new TextSpan(start, end - start);

            var enclosedDiagnostics = Sut[span];

            enclosedDiagnostics.Select(x => x.Code).Should().BeEquivalentTo(expectedDiagnosticCodes);
        }

        private static void WriteDummyDiagnostic(int start, int end, string code) =>
            Sut.Write(new Diagnostic(new(start, end - start), DiagnosticLevel.Off, DiagnosticSource.Compiler, code, ""));
    }
}
