// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
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

        public BicepCompilationProvider(IResourceTypeProvider resourceTypeProvider, IFileResolver fileResolver)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.fileResolver = fileResolver;
        }

        public CompilationContext Create(SourceFileGrouping sourceFileGrouping, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            var compilation = new Compilation(resourceTypeProvider, sourceFileGrouping, modelLookup);

            return new CompilationContext(compilation);
        }
    }
}
