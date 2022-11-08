// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public interface IOciArtifactModuleRepositoryCompletionProvider
    {
        Task<CompletionList> GetOciArtifactModuleRepositoryPathCompletionsAsync(Uri templateUri);
    }
}
