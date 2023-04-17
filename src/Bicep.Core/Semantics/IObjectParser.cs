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
        abstract bool TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken);
        abstract JToken ExtractTokenFromObject(string fileContent);
        abstract ErrorDiagnostic GetExtractTokenErrorType(IPositionable positionable);
        abstract bool TryExtractFromTokenByPath(JToken token, string tokenSelectorPath, IPositionable positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, out JToken newToken);
    }
}
