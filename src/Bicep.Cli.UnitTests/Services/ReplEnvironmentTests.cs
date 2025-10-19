// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Cli.UnitTests.Services;

[TestClass]
public class ReplEnvironmentTests
{
    private static ReplEnvironment CreateReplEnvironment()
        => ServiceBuilder.Create(x => x.AddSingleton<ReplEnvironment>())
            .Construct<ReplEnvironment>();

    [TestMethod]
    public void Var_syntax_is_highlighted()
    {
        var replEnvironment = CreateReplEnvironment();
        var output = GetHighlighted(replEnvironment, """
            var test = /*
            foo
            bar
            */
            v
            """);

        AnsiHelper.ReplaceCodes(output).Should().BeEquivalentToIgnoringNewlines("""
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray]> [Reset][Gray]var[Reset] [Blue]test[Reset] = [Green]/*[Reset][MoveCursorToLineStart][MoveCursorRight(2)][MoveCursorRight(13)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset][Green]foo[Reset][MoveCursorToLineStart][MoveCursorRight(3)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset][Green]bar[Reset][MoveCursorToLineStart][MoveCursorRight(3)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset][Green]*/[Reset][MoveCursorToLineStart][MoveCursorRight(2)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset][Blue]v[Reset][MoveCursorToLineStart][MoveCursorRight(1)][ShowCursor]
            """);
    }

    [TestMethod]
    public void Valid_var_syntax_raises_no_diagnostics()
    {
        var replEnvironment = CreateReplEnvironment();
        var result = replEnvironment.EvaluateInput("""
            var test = 'abc'
            """);

        result.AnnotatedDiagnostics.Should().BeEmpty();
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public void Type_syntax_is_highlighted()
    {
        var replEnvironment = CreateReplEnvironment();
        var output = GetHighlighted(replEnvironment, """
            type foo = {
              // This is a comment
              prop: string
            }
            """);

        AnsiHelper.ReplaceCodes(output).Should().BeEquivalentToIgnoringNewlines("""
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray]> [Reset][Gray]type[Reset] [Purple]foo[Reset] = {[MoveCursorToLineStart][MoveCursorRight(2)][MoveCursorRight(12)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset]  [Green]// This is a comment[Reset][MoveCursorToLineStart][MoveCursorRight(22)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset]  prop: string[MoveCursorToLineStart][MoveCursorRight(14)][ShowCursor]
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray][Reset]}[MoveCursorToLineStart][MoveCursorRight(1)][ShowCursor]
            """);
    }

    [TestMethod]
    public void Valid_type_syntax_raises_no_diagnostics()
    {
        var replEnvironment = CreateReplEnvironment();
        var result = replEnvironment.EvaluateInput("""
            type foo = {
              // This is a comment
              prop: string
            }
            """);

        result.AnnotatedDiagnostics.Should().BeEmpty();
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public void Function_syntax_is_highlighted()
    {
        var replEnvironment = CreateReplEnvironment();
        var output = GetHighlighted(replEnvironment, """
            func sayHi(name string) string => 'Hello ${name}!'
            """);

        AnsiHelper.ReplaceCodes(output).Should().BeEquivalentToIgnoringNewlines("""
            [HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Gray]> [Reset][Gray]func[Reset] [Blue]sayHi[Reset](name [Purple]string[Reset]) [Purple]string[Reset] => [Orange]'Hello [Reset][Reset]${[Reset][Blue]name[Reset][Reset]}[Reset][Orange]!'[Reset][MoveCursorToLineStart][MoveCursorRight(2)][MoveCursorRight(50)][ShowCursor]
            """);
    }

    [TestMethod]
    public void Valid_function_syntax_raises_no_diagnostics()
    {
        var replEnvironment = CreateReplEnvironment();
        var result = replEnvironment.EvaluateInput("""
            func sayHi(name string) string => 'Hello ${name}!'
            """);

        result.AnnotatedDiagnostics.Should().BeEmpty();
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public void Expression_evaluation_succeeds()
    {
        var replEnvironment = CreateReplEnvironment();
        replEnvironment.EvaluateInput("""
            type PersonType = {
              name: string
              age: int
            }
            """);
        replEnvironment.EvaluateInput("""
            func sayHi(person PersonType) string => 'Hello ${person.name}, you are ${person.age} years old!'
            """);
        replEnvironment.EvaluateInput("""
            var alice = {
              name: 'Alice'
              age: 30
            }
            """);
        var result = replEnvironment.EvaluateInput("""
            [ sayHi(alice), sayHi({ name: 'Bob', age: 25 }) ]
            """);

        result.AnnotatedDiagnostics.Should().BeEmpty();
        result.Value.Should().NotBeNull();

        var output = replEnvironment.HighlightSyntax(result.Value);
        AnsiHelper.ReplaceCodes(output).Should().BeEquivalentToIgnoringNewlines("""
            [
              [Orange]'Hello Alice, you are 30 years old!'[Reset]
              [Orange]'Hello Bob, you are 25 years old!'[Reset]
            ]
            """);
    }

    private static string GetHighlighted(ReplEnvironment replEnvironment, string input)
    {
        var lines = StringUtils.SplitOnNewLine(input).ToArray();

        var output = new List<string>();
        for (var i = 0; i < lines.Length; i++)
        {
            var prevLines = string.Concat(lines.Take(i).Select(x => x + "\n"));
            var runes = lines[i].Select(x => new Rune(x)).ToArray();

            var prefix = i == 0 ? "> " : "";
            output.Add(replEnvironment.HighlightInputLine(prefix, prevLines, runes, runes.Length, printPrevLines: false));
        }

        return string.Join('\n', output);
    }
}
