// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;
using BicepCompilation = Bicep.Core.Semantics.Compilation;

namespace Bicep.LanguageServer.Features.Custom.Deployments.Services
{
    public class DeploymentFileCompilationCache : IDeploymentFileCompilationCache
    {
        private readonly ConcurrentDictionary<DocumentUri, BicepCompilation> compilationCache = new();

        public void CacheCompilation(DocumentUri documentUri, BicepCompilation compilation)
        {
            compilationCache.TryAdd(documentUri, compilation);
        }

        public BicepCompilation? FindAndRemoveCompilation(DocumentUri documentUri)
        {
            if (compilationCache.TryRemove(documentUri, out BicepCompilation? compilation) && compilation is not null)
            {
                return compilation;
            }

            return null;
        }
    }
}
