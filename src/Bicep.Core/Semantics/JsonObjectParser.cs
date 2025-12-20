// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics;

public class JsonObjectParser : ObjectParser
{
    protected override ResultWithDiagnostic<JToken> ExtractTokenFromObject(string fileContent, IPositionable positionable)
    {
        if (fileContent.TryFromJson<JToken>() is {} jToken)
        {
            return new(jToken);
        }

        return new(DiagnosticBuilder.ForPosition(positionable).UnparsableJsonType());
    }
}
