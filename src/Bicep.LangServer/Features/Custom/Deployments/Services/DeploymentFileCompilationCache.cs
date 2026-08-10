// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Deployments.Services
{
    public class DeploymentFileCompilationCache : IDeploymentFileCompilationCache
    {
        private readonly ConcurrentDictionary<DocumentUri, global::Bicep.Core.Semantics.Compilation> compilationCache = new();

        public void CacheCompilation(DocumentUri documentUri, global::Bicep.Core.Semantics.Compilation compilation)
        {
            compilationCache.TryAdd(documentUri, compilation);
        }

        public global::Bicep.Core.Semantics.Compilation? FindAndRemoveCompilation(DocumentUri documentUri)
        {
            if (compilationCache.TryRemove(documentUri, out global::Bicep.Core.Semantics.Compilation? compilation) && compilation is not null)
            {
                return compilation;
            }

            return null;
        }
    }
}
