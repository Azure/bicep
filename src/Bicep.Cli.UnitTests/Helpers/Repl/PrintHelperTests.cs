// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Drawing;
using System.Text;
using System.Web.Services.Description;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using PrintHelper = Bicep.Cli.Helpers.Repl.PrintHelper;

namespace Bicep.Cli.UnitTests.Helpers.Repl;

[TestClass]
public class PrintHelperTests
{
    [TestMethod]
    public void PrintInputLine_returns_expected_output()
    {
        var input = "var test = 'foo'";

        var result = PrintHelper.PrintInputLine("> ", input, input.Length);
        AnsiHelper.ReplaceCodes(result).Should().Be(string.Concat(
            "[HideCursor]",
            "[MoveCursorToLineStart]",
            "[ClearToEndOfScreen]",
            "[Gray]> [Reset]var test = 'foo'",
            "[MoveCursorToLineStart]",
            "[MoveCursorRight(2)]",
            "[MoveCursorRight(16)]",
            "[ShowCursor]"));
    }

    [TestMethod]
    public void PrintWithSyntaxHighlighting_returns_expected_output()
    {
        var compilation = CompilationHelper.Compile("""
            var foo = {
              abc: 'def'
              ghi: 123
            }
            """).Compilation;

        var model = compilation.GetEntrypointSemanticModel();

        var result = PrintHelper.PrintWithSyntaxHighlighting(model, model.SourceFile.ProgramSyntax.ToString());
        AnsiHelper.ReplaceCodes(result).Should().Be("""
            [Gray]var[Reset] [Blue]foo[Reset] = {
              [DarkYellow]abc[Reset]: [Orange]'def'[Reset]
              [DarkYellow]ghi[Reset]: [Orange]123[Reset]
            }
            """);
    }

    [TestMethod]
    public void PrintWithAnnotations_allows_annotating_highlighted_syntax()
    {
        var sourceText = """
            var foo = {
              abc: def
              ghi: 123,
            }
            """;
        var compilation = CompilationHelper.Compile(sourceText).Compilation;

        var model = compilation.GetEntrypointSemanticModel();
        var highlighted = PrintHelper.PrintWithSyntaxHighlighting(model, model.SourceFile.ProgramSyntax.ToString());

        var result = PrintHelper.PrintWithAnnotations(
            sourceText,
            model.GetAllDiagnostics().Select(x => new PrintHelper.AnnotatedDiagnostic(x)),
            highlighted);

        result = AnsiHelper.ReplaceCodes(result);

        AnsiHelper.ReplaceCodes(result).Should().Be("""
            [Gray]var[Reset] [Blue]foo[Reset] = {
                [Orange]~~~ Variable "foo" is declared but never used.[Reset]
              [DarkYellow]abc[Reset]: [Blue]def[Reset]
                   [Red]~~~ The name "def" does not exist in the current context.[Reset]
              [DarkYellow]ghi[Reset]: [Orange]123[Reset][Reset],[Reset]
                       [Red]^ Unexpected new line character after a comma.[Reset]
            
            """);
    }
}
