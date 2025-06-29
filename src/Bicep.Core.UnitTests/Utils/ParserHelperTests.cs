// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class ParserHelperTests
    {
        private string GetTextAtSpan(string text, TextSpan span)
        {
            return text.Substring(span.Position, span.Length);
        }

        [TestMethod]
        public void EscapedCursor()
        {
            var (file, selections) = ParserHelper.GetFileWithSelections("This is the |first cursor, an ||escaped cursor, and the |second cursor");
            file.Should().Be("This is the first cursor, an |escaped cursor, and the second cursor");
            selections.Should().HaveCount(2);

            selections[0].Length.Should().Be(0);
            GetTextAtSpan(file, new TextSpan(selections[0].Position, selections[0].Length + 5)).Should().Be("first");

            selections[1].Length.Should().Be(0);
            GetTextAtSpan(file, new TextSpan(selections[1].Position, selections[1].Length + 6)).Should().Be("second");
        }

        [TestMethod]
        public void StartAndEndCursorsWithDifferentLengths()
        {
            var (file, selections) = ParserHelper.GetFileWithSelections("This is *a <!!file>> with <!!selections>>", "*", "**", "<!!", ">>");
            file.Should().Be("This is a file with selections");
            selections.Should().HaveCount(3);

            selections[0].Length.Should().Be(0);
            GetTextAtSpan(file, new TextSpan(selections[0].Position, selections[0].Length + 1)).Should().Be("a");
            GetTextAtSpan(file, selections[1]).Should().Be("file");
            GetTextAtSpan(file, selections[2]).Should().Be("selections");
        }

        [TestMethod]
        public void SequentialSpans()
        {
            var (file, selections) = ParserHelper.GetFileWithSelections("This is|<<a>><<file>>"); // Using start/end with different lengths
            file.Should().Be("This isafile");
            selections.Should().HaveCount(3);

            selections[0].Length.Should().Be(0);
            GetTextAtSpan(file, new TextSpan(selections[0].Position, selections[0].Length + 1)).Should().Be("a");
            GetTextAtSpan(file, selections[1]).Should().Be("a");
            GetTextAtSpan(file, selections[2]).Should().Be("file");

            string GetTextAtSpan(string text, TextSpan span)
            {
                return text.Substring(span.Position, span.Length);
            }
        }
    }
}

