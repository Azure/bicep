// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public abstract class ObjectParser : IObjectParser
    {
        public abstract JToken ExtractTokenFromObject(string fileContent);
        public abstract ErrorType GetExtractTokenError(IPositionable positionable);

        public ErrorType GetExtractTokenFromPathError(IPositionable positionable)
            =>  ErrorType.Create(DiagnosticBuilder.ForPosition(positionable).NoJsonTokenOnPathOrPathInvalid());

        public JToken ExtractTokenFromObjectByPath(JToken token, string tokenSelectorPath)
        {
            var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();

            switch (selectTokens.Count)
            {
                case 0: throw new JsonException($"Required length greater than 0.");
                case 1: return selectTokens.First();
                default: return new JArray(selectTokens);
            }
        }
    }
}
