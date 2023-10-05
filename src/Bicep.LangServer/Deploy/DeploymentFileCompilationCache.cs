// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentFileCompilationCache : IDeploymentFileCompilationCache
    {
        private readonly ConcurrentDictionary<DocumentUri, Compilation> compilationCache = new ConcurrentDictionary<DocumentUri, Compilation>();

        public void CacheCompilation(DocumentUri documentUri, Compilation compilation)
        {
            compilationCache.TryAdd(documentUri, compilation);
        }

        public Compilation? FindAndRemoveCompilation(DocumentUri documentUri)
        {
            if (compilationCache.TryRemove(documentUri, out Compilation? compilation) && compilation is not null)
            {
                return compilation;
            }

            return null;
        }
    }
}
