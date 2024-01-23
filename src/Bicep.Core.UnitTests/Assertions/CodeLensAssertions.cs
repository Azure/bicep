// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.Core.UnitTests.Assertions;

public static class CodeLensAssertionsExtensions
{
    public static CodeLensAssertions Should(this CodeLens codeLens)
    {
        return new CodeLensAssertions(codeLens);
    }
}

public class CodeLensAssertions : ObjectAssertions<CodeLens, CodeLensAssertions>
{
    public CodeLensAssertions(CodeLens subject)
        : base(subject)
    {
    }

    protected override string Identifier => nameof(CodeLens);

    public AndConstraint<CodeLensAssertions> HaveCommandTitle(string title, string because = "", params object[] becauseArgs)
    {
        Subject.Command.Should().NotBeNull("Code lens command should not be null");
        Subject.Command!.Title.Should().Be(title, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CodeLensAssertions> HaveCommandName(string commandName, string because = "", params object[] becauseArgs)
    {
        Subject.Command.Should().NotBeNull("Code lens command should not be null");
        Subject.Command!.Name.Should().Be(commandName, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CodeLensAssertions> HaveCommandArguments(params string[] commandArguments)
    {
        Subject.Command.Should().NotBeNull("Code lens command should not be null");
        Subject.Command!.Arguments.Should().NotBeNull("Command args should not be null");
        var actualCommandArguments = Subject.CommandArguments();
        actualCommandArguments.Should().BeEquivalentTo(commandArguments);

        return new(this);
    }

    public AndConstraint<CodeLensAssertions> HaveNoCommandArguments()
    {
        Subject.CommandArguments().Should().BeEmpty("Command should have no arguments");

        return new(this);
    }

    public AndConstraint<CodeLensAssertions> HaveRange(Range range, string because = "", params object[] becauseArgs)
    {
        Subject.Range.Should().Be(range, because, becauseArgs);

        return new(this);
    }


}
