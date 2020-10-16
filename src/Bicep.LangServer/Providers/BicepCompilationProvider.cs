// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.FileSystem;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
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

        public CompilationContext Create(DocumentUri documentUri, string text)
        {
            var mainFileName = documentUri.GetFileSystemPath();
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.BuildWithPreloadedFile(fileResolver, mainFileName, text);
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            return new CompilationContext(compilation);
        }
    }
}