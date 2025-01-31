// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        private static ServiceBuilder Services => new();

        private static BicepCompletionContext CreateContextFromTextWithCursor(string text)
        {
            var (file, cursor) = ParserHelper.GetFileWithSingleCursor(text, "|");
            var compilation = Services.BuildCompilation(file);

            return BicepCompletionContext.Create(compilation, cursor);
        }

        [TestMethod]
        public void ZeroMatchingNodes_Create_ShouldThrow()
        {
            const string text = "var foo = 42";
            var compilation = Services.BuildCompilation(text);

            Action fail = () => BicepCompletionContext.Create(compilation, text.Length + 2);
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
    a == '1' ? (a == '2' ? |)
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

            cursors.Should().HaveCount(15);
            var cursorTuples = cursors.Select((cursor, cursorIndex) => (BicepCompletionContext.Create(compilation, cursor), cursor, cursorIndex));
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
            var cursorTuples = cursors.Select((cursor, cursorIndex) => (BicepCompletionContext.Create(compilation, cursor), cursor, cursorIndex));
            foreach (var (context, cursor, cursorIndex) in cursorTuples)
            {
                context.Kind.Should().NotHaveFlag(BicepCompletionContextKind.ArrayItem, "cursor index {0} with text offset {1} will not evaluate to the array item in current array context", cursorIndex, cursor);
            }
        }

        [DataTestMethod]
        [DataRow("var foo1 = [|]")]
        [DataRow("var foo2 = [ | ]")]
        [DataRow("var foo3 = [| ]")]
        [DataRow("var foo4 = [ |]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_Empty(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [,|]")]
        [DataRow("var foo2 = [, |]")]
        [DataRow("var foo3 = [|,]")]
        [DataRow("var foo4 = [ | ,]")]
        [DataRow("var foo5 = [,|,]")]
        [DataRow("var foo6 = [, |,]")]
        [DataRow("var foo7 = [, | ,]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_Empty_Commas(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [|, aSymbol]")]
        [DataRow("var foo2 = [ |, aSymbol]")]
        [DataRow("var foo3 = [ | , aSymbol]")]
        [DataRow("var foo4 = [| , aSymbol]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_NonEmpty_FirstItem(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a first value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [aSymbol,|, aSymbol]")]
        [DataRow("var foo2 = [aSymbol, |, aSymbol]")]
        [DataRow("var foo3 = [aSymbol,| , aSymbol]")]
        [DataRow("var foo4 = [aSymbol, | , aSymbol]")]
        [DataRow("var foo5 = [, |, aSymbol]")]
        [DataRow("var foo6 = [aSymbol, |,]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_NonEmpty_MiddleItem(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a middle value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [aSymbol,|]")]
        [DataRow("var foo2 = [aSymbol, |]")]
        [DataRow("var foo3 = [aSymbol, | ]")]
        [DataRow("var foo4 = [aSymbol,| ]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_NonEmpty_LastItem(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a last value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = |[]")]
        [DataRow("var foo2 = []|")]
        [DataRow("var foo3 = |[aSymbol]")]
        [DataRow("var foo4 = [aSymbol]|")]
        [DataRow("var foo5 = |[aSymbol,]")]
        [DataRow("var foo6 = [aSymbol,]|")]
        [DataRow("var foo7 = |[aSymbol, aSymbol]")]
        [DataRow("var foo8 = [aSymbol, aSymbol]|")]
        public void ContextKind_IsNot_ArrayItem_SingleLineArray_Closed_Outside(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().NotHaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is outside a closed single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [a|]")]
        [DataRow("var foo2 = [aSymbol, b|]")]
        [DataRow("var foo3 = [a|, bSymbol]")]
        [DataRow("var foo4 = [aSymbol, a|, bSymbol]")]
        [DataRow("var foo5 = [|a]")]
        [DataRow("var foo6 = [a,|a]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_AtSymbol(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [a |]")]
        [DataRow("var foo2 = [| a]")]
        [DataRow("var foo3 = [aSymbol, b |]")]
        [DataRow("var foo4 = [a |, bSymbol]")]
        [DataRow("var foo5 = [aSymbol, a |, bSymbol]")]
        [DataRow("var foo6 = [a,| a]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_NearSymbol(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a value area in a single line array");
        }

        [DataTestMethod]
        [DataRow("var foo1 = [(|)]")]
        [DataRow("var foo2 = [((|))]")]
        [DataRow("var foo3 = [(( | ))]")]
        [DataRow("var foo4 = [(|]")]
        [DataRow("var foo5 = [,(|)]")]
        [DataRow("var foo6 = [tmp ? |]")]
        [DataRow("var foo7 = [tmp ? a : |]")]
        [DataRow("var foo8 = [(tmp ? a : |)]")]
        [DataRow("var foo9 = [(tmp ? (tmp ? |) : a)]")]
        [DataRow("var foo10 = [(tmp ? a : b), (|)]")]
        [DataRow("var foo11 = [(tmp ? a : b), (|), a]")]
        [DataRow("var foo12 = [(tmp ? a : b), (|]")]
        public void ContextKind_Is_ArrayItem_SingleLineArray_Closed_TernaryAndParentheses(string text)
        {
            var context = CreateContextFromTextWithCursor(text);
            context.Kind.Should().HaveFlag(BicepCompletionContextKind.ArrayItem, $"cursor in {text} is a potential value area in a single line array");
        }
    }
}
