// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
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
    public class CompilationHelper
    {
        public static Compilation GetCompilation(DocumentUri documentUri,
            ICompilationManager compilationManager,
            IConfigurationManager configurationManager,
            IFeatureProvider features,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            INamespaceProvider namespaceProvider)
        {
            var fileUri = documentUri.ToUri();
            RootConfiguration? configuration;

            try
            {
                configuration = configurationManager.GetConfiguration(fileUri);
            }
            catch (ConfigurationException)
            {
                throw;
            }

            CompilationContext? context = compilationManager.GetCompilation(documentUri);
            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, new Workspace(), fileUri, configuration);
                return new Compilation(features, namespaceProvider, sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
