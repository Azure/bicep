// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public abstract class ObjectParser : IParser
    {
        public abstract JToken ExtractTokenFromObject(string fileContent);

        public JToken ExtractTokenFromObjectByPath(JToken token, string tokenSelectorPath)
        {
            var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();
            switch (selectTokens.Count)
            {
                case 0:
                    return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).NoJsonTokenOnPathOrPathInvalid()));
                case 1:
                    return selectTokens.First();
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
