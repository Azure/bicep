// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Bicep.Cli.UnitTests;

public record CliResult(
    string Stdout,
    string Stderr,
    int ExitCode)
{
    public CliResult WithoutAnsi()
        => this with { Stdout = AnsiHelper.RemoveCodes(Stdout) };

    public CliResult WithVisibleAnsi()
        => this with { Stdout = AnsiHelper.ReplaceCodes(Stdout) };

    public CliResult WithReplaced([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, MatchEvaluator evaluator)
        => this with { Stdout = Regex.Replace(Stdout, pattern, evaluator) };

    public CliResult WithoutDurations()
        => WithReplaced(@"\d+\.\d+s", m => new string(' ', m.Value.Length));
}