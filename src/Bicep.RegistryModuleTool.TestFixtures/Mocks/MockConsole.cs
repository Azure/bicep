// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using FluentAssertions;

namespace Bicep.RegistryModuleTool.TestFixtures.Mocks
{
    public class MockConsole : IConsole
    {
        private readonly StringWriter outWriter = new();

        private readonly StringWriter errorWriter = new();

        private readonly List<string> expectedOutLines = new();

        private readonly List<string> expectedErrorLines = new();

        public TextWriter Out => this.outWriter;

        public TextWriter Error => this.errorWriter;

        public MockConsole ExpectOutLines(params string[] expectedOutLines) => this.ExpectOutLines((IEnumerable<string>)expectedOutLines);

        public MockConsole ExpectOutLines(IEnumerable<string> expectedOutLines)
        {
            foreach (var expectedOutLine in expectedOutLines)
            {
                this.ExpectOutLine(expectedOutLine);
            }

            return this;
        }

        public MockConsole ExpectOutLine(string expectedOutLine)
        {
            this.expectedOutLines.Add(expectedOutLine);

            return this;
        }

        public MockConsole ExpectErrorLines(params string[] expectedErrorLines) => this.ExpectErrorLines((IEnumerable<string>)expectedErrorLines);

        public MockConsole ExpectErrorLines(IEnumerable<string> expectedErrorLines)
        {
            foreach (var expectedErrorLine in expectedErrorLines)
            {
                this.ExpectErrorLine(expectedErrorLine);
            }

            return this;
        }

        public MockConsole ExpectErrorLine(string expectedErrorLine)
        {
            this.expectedErrorLines.Add(expectedErrorLine.ReplaceLineEndings());

            return this;
        }

        public void Verify()
        {
            var actualOutLines = GetLines(this.outWriter);
            var actualErrorLines = GetLines(this.errorWriter);
            VerifyStringList(actualOutLines, this.expectedOutLines);
            VerifyStringList(actualErrorLines, this.expectedErrorLines);
        }

        private static List<string> GetLines(StringWriter writer)
        {
            var content = writer.ToString().ReplaceLineEndings(Environment.NewLine);
            var lines = content.Split(Environment.NewLine).ToList();

            // Remove trailing empty entry produced by a final newline
            if (lines.Count > 0 && lines[^1] == string.Empty)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines;
        }

        private static void VerifyStringList(List<string> a, List<string> b)
        {
            if (a.Count != b.Count)
            {
                a.Should().Equal(b);
            }
            else
            {
                for (var i = 0; i < a.Count; ++i)
                {
                    a[i].ReplaceLineEndings().Should().Be(b[i].ReplaceLineEndings());
                }
            }
        }
    }
}

