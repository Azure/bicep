// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public abstract class ObjectParser : IObjectParser
    {
        public abstract JToken ExtractTokenFromObject(string fileContent);
        public abstract ErrorType GetParsingError(IPositionable positionable);
        public JToken ExtractTokenFromObjectByPath(JToken token, string tokenSelectorPath)
        {
            var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();
            switch (selectTokens.Count)
            {
                case 0: throw new JsonException($"Required length greater than 0.");
                case 1: return selectTokens.First();
                default:
                    var arrayToken = new JArray();
                    foreach (var selectToken in selectTokens)
                    {
                        arrayToken.Add(selectToken);
                    }
                    return arrayToken;
            }
        }
    }
}
