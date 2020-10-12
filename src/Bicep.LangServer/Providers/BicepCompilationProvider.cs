// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.FileSystem;
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

        public BicepCompilationProvider(IResourceTypeProvider resourceTypeProvider)
        {
            this.resourceTypeProvider = resourceTypeProvider;
        }

        public CompilationContext Create(DocumentUri documentUri, string text)
        {
            var mainFileName = documentUri.GetFileSystemPath();
            var fileResolver = new LspFileResolver(new Dictionary<string, string>{ [mainFileName] = text });
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, mainFileName);
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            return new CompilationContext(compilation);
        }
    }
}