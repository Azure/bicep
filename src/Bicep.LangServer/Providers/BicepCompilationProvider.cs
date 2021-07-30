// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

        public CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Build(fileResolver, moduleDispatcher, workspace, documentUri.ToUri());
            return this.CreateContext(syntaxTreeGrouping);
        }

        public CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current)
        {
            var syntaxTreeGrouping = SourceFileGroupingBuilder.Rebuild(moduleDispatcher, workspace, current.Compilation.SourceFileGrouping);
            return this.CreateContext(syntaxTreeGrouping);
        }

        private CompilationContext CreateContext(SourceFileGrouping syntaxTreeGrouping)
        {
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);
            return new CompilationContext(compilation);
        }

    }
}
