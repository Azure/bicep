// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using Moq;
using System.CommandLine;
using System.CommandLine.IO;

namespace Bicep.RegistryModuleTool.TestFixtures.Mocks
{
    public class MockConsole : IConsole
    {
        private readonly Mock<IStandardStreamWriter> outMock = StrictMock.Of<IStandardStreamWriter>();

        private readonly Mock<IStandardStreamWriter> errorMock = StrictMock.Of<IStandardStreamWriter>();

        private readonly List<string> expectedOutLines = new();

        private readonly List<string> expectedErrorLines = new();

        private readonly List<string> actualOutLines = new();

        private readonly List<string> actualErrorLines = new();

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
            this.expectedOutLines.Add(expectedOutLine + Environment.NewLine);

            this.outMock.Setup(x => x.Write(It.IsAny<string>())).Callback<string>(actualOutLine => this.actualOutLines.Add(actualOutLine));

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
            this.expectedErrorLines.Add($"{expectedErrorLine.ReplaceLineEndings()}{Environment.NewLine}");

            this.errorMock.Setup(x => x.Write(It.IsAny<string>())).Callback<string>(this.actualErrorLines.Add);

            return this;
        }

        public IStandardStreamWriter Out => this.outMock.Object;

        public IStandardStreamWriter Error => this.errorMock.Object;

        public bool IsOutputRedirected => false;

        public bool IsErrorRedirected => false;

        public bool IsInputRedirected => false;

        public void Verify()
        {
            VerifyStringList(this.actualOutLines, this.expectedOutLines);
            VerifyStringList(this.actualErrorLines, this.expectedErrorLines);
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
