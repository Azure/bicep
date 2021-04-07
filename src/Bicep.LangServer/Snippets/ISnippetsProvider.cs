// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface ISnippetsProvider
    {
        IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets();

        Snippet? GetResourceBodyCompletionSnippet(string type);
    }
}
