// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.CodeAction;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions;

public static class CodeFixAssertionsExtensions
{
    public static CodeFixAssertions Should(this CodeFix diagnostic)
    {
        return new CodeFixAssertions(diagnostic);
    }
}

public class CodeFixAssertions : ObjectAssertions<CodeFix, CodeFixAssertions>
{
    public CodeFixAssertions(CodeFix subject)
        : base(subject)
    {
    }

    protected override string Identifier => "Fixable";

    public AndConstraint<CodeFixAssertions> HaveResult(string originalFile, string expectedResult, string because = "", params object[] becauseArgs)
    {
        Subject.Replacements.Should().HaveCount(1);
        var replacement = Subject.Replacements.Single();

        var actualText = originalFile.Remove(replacement.Span.Position, replacement.Span.Length);
        actualText = actualText.Insert(replacement.Span.Position, replacement.Text);

        actualText.Should().EqualIgnoringNewlines(expectedResult, because, becauseArgs);

        return new(this);
    }
}
