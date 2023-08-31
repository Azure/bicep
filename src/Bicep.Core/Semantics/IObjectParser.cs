// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Semantics
{
    public interface IObjectParser
    {
        bool TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, [NotNullWhen(true)] out JToken? newToken);
    }
}
