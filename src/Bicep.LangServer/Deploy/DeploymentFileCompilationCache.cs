// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentFileCompilationCache: IDeploymentFileCompilationCache
    {
        private readonly ConcurrentDictionary<DocumentUri, Compilation> compilationCache = new ConcurrentDictionary<DocumentUri, Compilation>();

        private readonly ICompilationManager compilationManager;
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProvider features;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;

        public DeploymentFileCompilationCache(
            ICompilationManager compilationManager,
            IConfigurationManager configurationManager,
            IFeatureProvider features,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            INamespaceProvider namespaceProvider)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.features = features;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.namespaceProvider = namespaceProvider;
        }

        public void GenerateAndCacheCompilation(DocumentUri documentUri)
        {
            var compilation = CompilationHelper.GetCompilation(
                documentUri,
                compilationManager,
                configurationManager,
                features,
                fileResolver,
                moduleDispatcher,
                namespaceProvider);

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
