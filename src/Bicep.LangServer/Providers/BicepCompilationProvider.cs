// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
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
        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;

        public BicepCompilationProvider(IResourceTypeProvider resourceTypeProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
        }

        public CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, documentUri.ToUri());
            return this.CreateContext(syntaxTreeGrouping, modelLookup);
        }

        public CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, current.Compilation.SourceFileGrouping);
            return this.CreateContext(syntaxTreeGrouping, modelLookup);
        }

        private CompilationContext CreateContext(SourceFileGrouping syntaxTreeGrouping, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping, modelLookup);
            return new CompilationContext(compilation);
        }
    }
}
