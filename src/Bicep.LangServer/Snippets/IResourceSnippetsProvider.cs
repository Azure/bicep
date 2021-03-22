// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface IResourceSnippetsProvider
    {
        IEnumerable<ResourceSnippet> GetResourceSnippets();

        string GetResourceDeclarationBody(string type);
    }
}
