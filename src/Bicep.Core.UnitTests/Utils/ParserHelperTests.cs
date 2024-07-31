// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class ParserHelperTests
    {
        [TestMethod]
        public void StartAndEndCursorsWithDifferentLengths()
        {
            var (file, selections) = ParserHelper.GetFileWithSelections("This is *a <!!file>> with <!!selections>>", '*', "<!!", ">>");
            file.Should().Be("This is a file with selections");
            selections.Should().HaveCount(3);

            selections[0].Length.Should().Be(0);
            GetTextAtSpan(file, new TextSpan(selections[0].Position, selections[0].Length + 1)).Should().Be("a");
            GetTextAtSpan(file, selections[1]).Should().Be("file");
            GetTextAtSpan(file, selections[2]).Should().Be("selections");

            string GetTextAtSpan(string text, TextSpan span)
            {
                return text.Substring(span.Position, span.Length);
            }
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

