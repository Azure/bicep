// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceGraph;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface IModuleReferenceCompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(BicepSourceFile sourceFile, BicepCompletionContext context, CancellationToken cancellationToken);

        Task<CompletionItem> ResolveCompletionItem(CompletionItem completionItem, CancellationToken cancellationToken);
    }
}
