// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Highlighting;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Handlers;

[TestClass]
public class BicepSemanticTokensHandlerTests
{
    [TestMethod]
    public void MapTokenType_can_convert_all_enum_values()
    {
        // This test just ensures that we don't forget to update the mapping function if we add new token types.
        foreach (var tokenType in (SemanticTokenType[])Enum.GetValues(typeof(SemanticTokenType)))
        {
            BicepSemanticTokensHandler.MapTokenType(tokenType).Should().NotBeNull();
        }
    }
}
