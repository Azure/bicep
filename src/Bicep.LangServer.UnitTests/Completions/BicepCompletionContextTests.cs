// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class BicepCompletionContextTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        [TestMethod]
        public void ZeroMatchingNodes_Create_ShouldThrow()
        {
            const string text = "var foo = 42";
            var compilation = Services.BuildCompilation(text);

            Action fail = () => BicepCompletionContext.Create(BicepTestConstants.Features, compilation, text.Length + 2);
            fail.Should().Throw<ArgumentException>().WithMessage("The specified offset 14 is outside the span of the specified ProgramSyntax node.");
        }

        [TestMethod]
        public void ShouldArrayItemTypeFlowThrough_True()
        {
            const string text = @"
var a string = 'test'
resource foo 'Microsoft.Foo/bar@2020-01-01' = {
  name: 'foo'
  dependsOn: [
    (|)
    ((|))
    a == '1' ? |
    a == '1' ? | :
    a == '1' ? : |
    a == '1' ? a == '2' ? |
    a == '1' ? a == '2' ? : |
    a == '1' ? a == '2' ? | : aResource : aResource
    a == '1' ? (a == '2' ? |
    a == '1' ? (a == '2' ? |)
    (a == '1' ? aResource : |
    (a == '1' ? aResource : |)
    (a == '1' ? (|))
    (a == '1' ? (|) :)
    (a == '1' ? () : (|))
    (a == '1' ? aResource : (|))
    (a == '1' ? : (|))
  ]
}
";
            var (file, cursors) = ParserHelper.GetFileWithCursors(text, '|');
            var compilation = Services.BuildCompilation(file);

            cursors.Should().HaveCount(17);
            var cursorTuples = cursors.Select((cursor, cursorIndex) => (BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursor), cursor, cursorIndex));
            foreach (var (context, cursor, cursorIndex) in cursorTuples)
            {
                context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, "cursor index {0} with text offset {1} expects an array item", cursorIndex, cursor);
            }
        }

        [TestMethod]
        public void ShouldArrayItemTypeFlowThrough_False_WithinObject()
        {
            const string text = @"
var a string = 'test'
resource foo 'Microsoft.Foo/bar@2020-01-01' = {
  name: 'foo'
  properties: {
    environmentVariables: [
      {
        key: (|)
        key: ((|))
        key: a == '1' ? |
        key: a == '1' ? | :
        key: a == '1' ? : |
        key: a == '1' ? a == '2' ? |
        key: a == '1' ? a == '2' ? : |
        key: a == '1' ? a == '2' ? | : aResource : aResource
        key: a == '1' ? (a == '2' ? |
        key: a == '1' ? (a == '2' ? |)
        key: (a == '1' ? aResource : |
        key: (a == '1' ? aResource : |)
        key: (a == '1' ? (|))
        key: (a == '1' ? (|) :)
        key: (a == '1' ? () : (|))
        key: (a == '1' ? aResource : (|))
        key: (a == '1' ? : (|))
      }
    ]
  }

";
            var (file, cursors) = ParserHelper.GetFileWithCursors(text, '|');
            var compilation = Services.BuildCompilation(file);

            cursors.Should().HaveCount(17);
            var cursorTuples = cursors.Select((cursor, cursorIndex) => (BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursor), cursor, cursorIndex));
            foreach (var (context, cursor, cursorIndex) in cursorTuples)
            {
                context.Kind.Should().NotHaveFlag(BicepCompletionContextKind.ArrayItem, "cursor index {0} with text offset {1} will not evaluate to the array item in current array context", cursorIndex, cursor);
            }
        }
    }
}
