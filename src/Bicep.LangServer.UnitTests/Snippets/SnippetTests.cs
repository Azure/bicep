// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Snippets
{
    [TestClass]
    public class SnippetTests
    {
        [DataRow("")]
        [DataRow("var foo")]
        [DataTestMethod]
        public void SnippetsWithoutPlaceholdersShouldParse(string text)
        {
            var snippet = new Snippet(text);
            snippet.Text.Should().Be(text);
            snippet.Placeholders.Should().BeEmpty();
            snippet.FormatDocumentation().Should().Be(text);
        }

        [TestMethod]
        public void MultilineSnippetShouldParseCorrectly()
        {
            const string text = "resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {\r\n  name: $5\r\n  $0\r\n}";

            var snippet = new Snippet(text);
            snippet.Text.Should().Be(text);

            snippet.Placeholders.Should().SatisfyRespectively(
                x =>
                {
                    x.Index.Should().Be(0);
                    x.Name.Should().BeNull();
                    x.Span.ToString().Should().Be("[93:95]");
                },
                x =>
                {
                    x.Index.Should().Be(1);
                    x.Name.Should().Be("Identifier");
                    x.Span.ToString().Should().Be("[9:24]");
                }, 
                x =>
                {
                    x.Index.Should().Be(2);
                    x.Name.Should().Be("Provider");
                    x.Span.ToString().Should().Be("[36:49]");
                }, 
                x =>
                {
                    x.Index.Should().Be(3);
                    x.Name.Should().Be("Type");
                    x.Span.ToString().Should().Be("[50:59]");
                },
                x =>
                {
                    x.Index.Should().Be(4);
                    x.Name.Should().Be("Version");
                    x.Span.ToString().Should().Be("[60:72]");
                },
                x =>
                {
                    x.Index.Should().Be(5);
                    x.Name.Should().BeNull();
                    x.Span.ToString().Should().Be("[87:89]");
                });

            snippet.FormatDocumentation().Should().Be("resource Identifier 'Microsoft.Provider/Type@Version' = {\r\n  name: \r\n  \r\n}");
        }

        [TestMethod]
        public void OneLineSnippetShouldParseCorrectly()
        {
            const string text = "output ${1:Identifier} ${2:Type} = $0";

            var snippet = new Snippet(text);
            snippet.Text.Should().Be(text);

            snippet.Placeholders.Should().SatisfyRespectively(
                x =>
                {
                    x.Index.Should().Be(0);
                    x.Name.Should().BeNull();
                    x.Span.ToString().Should().Be("[35:37]");
                },
                x =>
                {
                    x.Index.Should().Be(1);
                    x.Name.Should().Be("Identifier");
                    x.Span.ToString().Should().Be("[7:22]");
                },
                x =>
                {
                    x.Index.Should().Be(2);
                    x.Name.Should().Be("Type");
                    x.Span.ToString().Should().Be("[23:32]");
                });

            snippet.FormatDocumentation().Should().Be("output Identifier Type = ");
        }

        [TestMethod]
        public void SinglePropertySnippetShouldParseCorrectly()
        {
            const string text = "name: '$0'";

            var snippet = new Snippet(text);
            snippet.Text.Should().Be(text);

            snippet.Placeholders.Should().SatisfyRespectively(
                x =>
                {
                    x.Index.Should().Be(0);
                    x.Name.Should().BeNull();
                    x.Span.ToString().Should().Be("[7:9]");
                });

            snippet.FormatDocumentation().Should().Be("name: ''");
        }
    }
}
