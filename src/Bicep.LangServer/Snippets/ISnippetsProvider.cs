// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface ISnippetsProvider
    {
        IEnumerable<Snippet> GeNestedChildResourceSnippets(TypeSymbol typeSymbol);

        IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets();

        IEnumerable<Snippet> GetModuleBodyCompletionSnippets(TypeSymbol typeSymbol);

        IEnumerable<Snippet> GetObjectBodyCompletionSnippets(TypeSymbol typeSymbol);

        IEnumerable<Snippet> GetResourceBodyCompletionSnippets(TypeSymbol typeSymbol, bool isExistingResource);
    }
}
