// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Highlighting;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Highlighting;

[TestClass]
public class SemanticTokenVisitorTests
{
    [TestMethod]
    public void Build_WithEscapeSequences_ExcludesEscapesFromStringTokens()
    {
        var bicepText = @"var foo = 'a\\b\'c\${d\n\r\t\u{1F600}z'";
        var result = CompilationHelper.Compile(bicepText);

        var stringTokenTexts = SemanticTokenVisitor.Build(result.Compilation.GetEntrypointSemanticModel())
            .Where(token => token.TokenType == SemanticTokenType.String)
            .Select(token => bicepText.Substring(token.Positionable.Span.Position, token.Positionable.Span.Length));

        stringTokenTexts.Should().Equal("'a", "b", "c", "d", "z'");
    }
}
