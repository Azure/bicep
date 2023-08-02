// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface ICompletionProvider
    {
        Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context, CancellationToken cancellationToken);
    }
}
