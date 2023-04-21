// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Snippets
{
    public interface ISnippetsProvider
    {
        IEnumerable<Snippet> GetNestedResourceDeclarationSnippets(ResourceTypeReference resourceTypeReference);

        IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets();

        IEnumerable<Snippet> GetModuleBodyCompletionSnippets(ModuleDeclarationSyntax moduleDeclarationSyntax, TypeSymbol typeSymbol);

        IEnumerable<Snippet> GetObjectBodyCompletionSnippets(TypeSymbol typeSymbol);

        IEnumerable<Snippet> GetResourceBodyCompletionSnippets(ResourceType resourceType, bool isExistingResource, bool isResourceNested);
    }
}
