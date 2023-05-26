// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface IModuleReferenceCompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri templateUri, BicepCompletionContext context, CancellationToken cancellationToken);

        Task ResolveCompletion(CompletionItem completionItem, CancellationToken cancellationToken);
    }
}
