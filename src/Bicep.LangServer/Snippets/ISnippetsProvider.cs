// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface ISnippetsProvider
    {
        IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets();

        IEnumerable<Snippet> GetResourceBodyCompletionSnippets(TypeSymbol typeSymbol, bool isExistingResource);
    }
}
