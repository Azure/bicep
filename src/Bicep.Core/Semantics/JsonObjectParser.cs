// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class JsonObjectParser : ObjectParser
    {
        /// <summary>
        /// TryFromJson returns null if the fileContent is not a valid JSON object
        /// </summary>
        override protected JToken ExtractTokenFromObject(string fileContent)
            => fileContent.TryFromJson<JToken>();
        override protected Diagnostic GetExtractTokenErrorType(IPositionable positionable)
            => DiagnosticBuilder.ForPosition(positionable).UnparsableJsonType();
    }
}
