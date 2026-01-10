// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics;

public abstract class ObjectParser : IObjectParser
{
    public ResultWithDiagnostic<JToken> TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable)
    {
        var result = ExtractTokenFromObject(fileContent, positionable[0]);
        if (result.IsSuccess(out var newToken) && tokenSelectorPath is {})
        {
            return TryExtractFromTokenByPath(newToken, tokenSelectorPath, positionable[1]);
        }

        return result;
    }

    protected abstract ResultWithDiagnostic<JToken> ExtractTokenFromObject(string fileContent, IPositionable positionable);

    private static ResultWithDiagnostic<JToken> TryExtractFromTokenByPath(JToken token, string tokenSelectorPath, IPositionable positionable)
    {
        try
        {
            var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();

            return selectTokens switch
            {
                [] => new(DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid()),
                [var singleToken] => new(singleToken),
                _ => new(new JArray(selectTokens))
            };
        }
        catch (JsonException)
        {
            return new(DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid());
        }
    }
}
