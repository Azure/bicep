// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface IResourceSnippetsProvider
    {
        IEnumerable<ResourceSnippet> GetResourceSnippets();
    }
}
