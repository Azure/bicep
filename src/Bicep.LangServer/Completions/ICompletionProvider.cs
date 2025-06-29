// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface ICompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context, CancellationToken cancellationToken);

        Task<CompletionItem> Resolve(CompletionItem completionItem, CancellationToken cancellationToken)
        {
            return Task.FromResult(completionItem);
        }
    }
}
