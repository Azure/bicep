// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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
        var outputs = EvaluateInputs([
            """
            type PersonType = {
              name: string
              age: int
            }
            """,
            """
            func sayHi(person PersonType) string => 'Hello ${person.name}, you are ${person.age} years old!'
            """,
            """
            var alice = {
              name: 'Alice'
              age: 30
            }
            """,
            """
            [ sayHi(alice), sayHi({ name: 'Bob', age: 25 }) ]
            """
        ]);

        outputs.Should().SatisfyRespectively(
            x => x.Should().BeEmpty(),
            x => x.Should().BeEmpty(),
            x => x.Should().BeEmpty(),
            x => x.Should().BeEquivalentToIgnoringNewlines("""
                [
                  [Orange]'Hello Alice, you are 30 years old!'[Reset]
                  [Orange]'Hello Bob, you are 25 years old!'[Reset]
                ]

                """));
    }

    [TestMethod]
    public void Expression_evaluation_succeeds_issue_18316()
    {
        // https://github.com/Azure/bicep/issues/18316
        var outputs = EvaluateInputs([
            """
            var varMockedEntraGroupIds = [
              {
                uniqueName: 'Reader-Group'
                roleToAssign: 'Reader'
                groupId: '11111111-1111-1111-1111-111111111111'
              }
              {
                uniqueName: 'Contributor-Group'
                roleToAssign: 'Contributor'
                groupId: '22222222-2222-2222-2222-222222222222'
              }
              {
                uniqueName: 'DevOps-Group'
                groupId: '33333333-3333-3333-3333-333333333333'
              }
            ]
            """,
            """
            var outRoleAssignments object[] = union(map(
              filter(varMockedEntraGroupIds, item => !contains(item.uniqueName, 'DevOps')),
              group => {
                principalId: group.groupId
                definition: group.roleToAssign
                relativeScope: ''
                principalType: 'Group'
              }
            ),[
              {
                principalId: '22222222-2222-2222-2222-222222222222'
                definition: 'Reader'
                relativeScope: ''
                principalType: 'ServicePrincipal'
              }
            ])
            """,
            """
            outRoleAssignments
            """
        ]);

        outputs.Should().SatisfyRespectively(
            x => x.Should().BeEmpty(),
            x => x.Should().BeEquivalentToIgnoringNewlines("""
                    [DarkYellow]definition[Reset]: [Blue]group[Reset][Reset].[Reset][DarkYellow]roleToAssign[Reset]
                                      [DarkYellow]~~~~~~~~~~~~ The property "roleToAssign" does not exist in the resource or type definition, although it might still be valid. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.[Reset]

                """),
            x => x.Should().BeEquivalentToIgnoringNewlines("""
                [
                  {
                    [DarkYellow]principalId[Reset]: [Orange]'11111111-1111-1111-1111-111111111111'[Reset]
                    [DarkYellow]definition[Reset]: [Orange]'Reader'[Reset]
                    [DarkYellow]relativeScope[Reset]: [Orange]''[Reset]
                    [DarkYellow]principalType[Reset]: [Orange]'Group'[Reset]
                  }
                  {
                    [DarkYellow]principalId[Reset]: [Orange]'22222222-2222-2222-2222-222222222222'[Reset]
                    [DarkYellow]definition[Reset]: [Orange]'Contributor'[Reset]
                    [DarkYellow]relativeScope[Reset]: [Orange]''[Reset]
                    [DarkYellow]principalType[Reset]: [Orange]'Group'[Reset]
                  }
                  {
                    [DarkYellow]principalId[Reset]: [Orange]'22222222-2222-2222-2222-222222222222'[Reset]
                    [DarkYellow]definition[Reset]: [Orange]'Reader'[Reset]
                    [DarkYellow]relativeScope[Reset]: [Orange]''[Reset]
                    [DarkYellow]principalType[Reset]: [Orange]'ServicePrincipal'[Reset]
                  }
                ]

                """));
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

    [DataRow("""
    var varMockedEntraGroupIds = [
      {
        uniqueName: 'Reader-Group'
        roleToAssign: 'Reader'
        groupId: '11111111-1111-1111-1111-111111111111'
      }
      {
        uniqueName: 'Contributor-Group'
        roleToAssign: 'Contributor'
        groupId: '22222222-2222-2222-2222-222222222222'
      }
      {
        uniqueName: 'DevOps-Group'
        groupId: '33333333-3333-3333-3333-333333333333'
      }
    ]
    """)]
    [DataRow("""
    var outRoleAssignments object[] = union(map(
      filter(varMockedEntraGroupIds, item => !contains(item.uniqueName, 'DevOps')),
      group => {
        principalId: group.groupId
        definition: group.roleToAssign
        relativeScope: ''
        principalType: 'Group'
      }
    ),[
      {
        principalId: '22222222-2222-2222-2222-222222222222'
        definition: 'Reader'
        relativeScope: ''
        principalType: 'ServicePrincipal'
      }
    ])
    """)]
    [DataRow("""
    var multilineString = '''
    Line 1
    Line 2
    Line 3
    '''
    """)]
    [DataRow("""
    var singleLineString = '${true} or ${false}?'
    """)]
    [DataRow("var foo = {\n")]
    [DataRow("""
    var test = {
      abc: 'def' // boo
    }
    """)]
    [TestMethod]
    public void ShouldSubmitBuffer_terminates_at_expected_point(string text)
    {
        EnsureSingleInput(text);
    }

    private static void EnsureSingleInput(string text)
    {        
        var input = text.NormalizeNewlines().Split('\n');
        for (var i = 0; i < input.Length; i++)
        {
            var isLastLine = i == input.Length - 1;
            // The console command always submits with a newline
            var partialText = string.Join("\n", input.Take(i + 1)) + "\n";
            ReplEnvironment.ShouldSubmitBuffer(partialText, input[i]).Should().Be(isLastLine);
        }
    }

    private static ImmutableArray<string> EvaluateInputs(IEnumerable<string> input)
    {
        var replEnvironment = CreateReplEnvironment();
        var outputs = new List<string>();
        foreach (var inputBlock in input)
        {
            EnsureSingleInput(inputBlock);
            var output = replEnvironment.EvaluateAndGetOutput(inputBlock);
            outputs.Add(AnsiHelper.ReplaceCodes(output));
        }

        return [..outputs];
    }
}
