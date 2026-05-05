// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics;

public interface IObjectParser
{
    ResultWithDiagnostic<JToken> TryExtractFromObject(string fileContent, string? tokenSelectorPath, IPositionable[] positionable);
}
