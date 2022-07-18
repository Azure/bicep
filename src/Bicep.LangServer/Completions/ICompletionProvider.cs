// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public interface ICompletionProvider
    {
        IEnumerable<CompletionItem> GetFilteredCompletions(Compilation model, BicepCompletionContext context);
        IEnumerable<CompletionItem> GetFilteredParamsCompletions(ParamsSemanticModel paramsSemanticModel, ParamsCompletionContext paramsCompletionContext);
    }
}
