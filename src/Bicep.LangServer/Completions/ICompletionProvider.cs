// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.SemanticModel;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface ICompletionProvider
    {
        IEnumerable<CompletionItem> GetFilteredCompletions(SemanticModel model, BicepCompletionContext context);
    }
}
