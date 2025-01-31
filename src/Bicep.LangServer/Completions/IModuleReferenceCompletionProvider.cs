// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Workspaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface IModuleReferenceCompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(BicepCompletionContext context, CancellationToken cancellationToken);
    }
}
