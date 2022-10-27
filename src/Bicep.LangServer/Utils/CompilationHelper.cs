// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Utils
{
    public static class CompilationHelper
    {
        public static Compilation GetCompilation(
            DocumentUri documentUri,
            Uri fileUri,
            IApiVersionProviderFactory apiVersionProviderFactory,
            IBicepAnalyzer bicepAnalyzer,
            ICompilationManager compilationManager,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            INamespaceProvider namespaceProvider)
        {
            // Bicep file could contain load functions like loadTextContent(..). We'll refresh compilation to detect changes in files referenced in load functions.
            compilationManager.RefreshCompilation(fileUri);
            CompilationContext? context = compilationManager.GetCompilation(documentUri);
            // CompilationContext will be null if the file is not open in the editor.
            // E.g. When user right clicks on a file from the explorer context menu without opening the file and invokes build/deploy
            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, new Workspace(), fileUri);
                return new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
