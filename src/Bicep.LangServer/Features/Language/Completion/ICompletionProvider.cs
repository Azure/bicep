// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Language.Completion
{
    public interface ICompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(global::Bicep.Core.Semantics.Compilation compilation, BicepCompletionContext context, CancellationToken cancellationToken);

        Task<CompletionItem> Resolve(CompletionItem completionItem, CancellationToken cancellationToken)
        {
            return Task.FromResult(completionItem);
        }
    }
}
