// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Creates compilation contexts.
    /// </summary>
    /// <remarks>This class exists only so we can mock fatal exceptions in tests.</remarks>
    public class BicepCompilationProvider: ICompilationProvider
    {
        private readonly INamespaceProvider namespaceProvider;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCompilationProvider(INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher)
        {
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
        }

        public CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, RootConfiguration configuration)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, documentUri.ToUri());
            return this.CreateContext(syntaxTreeGrouping, modelLookup, configuration);
        }

        public CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, RootConfiguration configuration)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, current.Compilation.SourceFileGrouping);
            return this.CreateContext(syntaxTreeGrouping, modelLookup, configuration);
        }

        private CompilationContext CreateContext(SourceFileGrouping syntaxTreeGrouping, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, RootConfiguration configuration)
        {
            var compilation = new Compilation(namespaceProvider, syntaxTreeGrouping, configuration, modelLookup);
            return new CompilationContext(compilation);
        }
    }
}
