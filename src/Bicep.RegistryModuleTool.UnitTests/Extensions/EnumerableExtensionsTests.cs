// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.RegistryModuleTool.Extensions;
using System;

namespace Bicep.RegistryModuleTool.UnitTests.Extensions
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        private record NoProperty();

        public record WithProperty(string Name, long Number, string Fruit);

        [TestMethod]
        public void ToMarkdownTable_TypeWithoutProperty_ThrowsException()
        {
            var enumerable = new[] { new NoProperty() };

            FluentActions.Invoking(() => enumerable.ToMarkdownTable(_ => MarkdownTableColumnAlignment.Left)).Should()
                .Throw<ArgumentException>()
                .WithMessage("Expected NoProperty to have at least one property.");
        }

        [TestMethod]
        public void ToMarkdownTable_TypeWithProperty_ReturnsMarkdownString()
        {
            var enumerable = new[]
            {
                new WithProperty("foo", 12345, "Apple"),
                new WithProperty("bar", 99999999999, "Banana"),
                new WithProperty("foobar", -200, "Dragon Fruit"),
            };

            var markdown = enumerable.ToMarkdownTable(columnName => columnName switch
            {
                nameof(WithProperty.Name) => MarkdownTableColumnAlignment.Left,
                nameof(WithProperty.Number) => MarkdownTableColumnAlignment.Right,
                _ => MarkdownTableColumnAlignment.Center,
            });

            markdown.Should().Be(@"
| Name   | Number      | Fruit        |
| :----- | ----------: | :----------: |
| foo    | 12345       | Apple        |
| bar    | 99999999999 | Banana       |
| foobar | -200        | Dragon Fruit |
".ReplaceLineEndings().TrimStart('\r', '\n'));
        }
    }
}
