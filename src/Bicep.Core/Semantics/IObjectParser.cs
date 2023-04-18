// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public interface IObjectParser
    {
        bool TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken);
        JToken ExtractTokenFromObject(string fileContent);
        ErrorDiagnostic GetExtractTokenErrorType(IPositionable positionable);
        bool TryExtractFromTokenByPath(JToken token, string tokenSelectorPath, IPositionable positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken);
    }
}
