// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface IObjectParser
    {
        JToken ExtractTokenFromObject(string fileContent);
        JToken ExtractTokenFromObjectByPath(JToken token, string tokenSelectorPath);
    }
}
