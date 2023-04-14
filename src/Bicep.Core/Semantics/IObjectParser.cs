// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public interface IObjectParser
    {
        public abstract JToken ExtractTokenFromObject(string fileContent);
        public abstract JToken ExtractTokenFromObjectByPath(JToken token, string tokenSelectorPath);
    }
}
