// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Creates compilation contexts.
    /// </summary>
    /// <remarks>This class exists only so we can mock fatal exceptions in tests.</remarks>
    public class BicepCompilationProvider: ICompilationProvider
    {
        private readonly ResourceTypeRegistrar resourceTypeRegistrar;

        public BicepCompilationProvider(ResourceTypeRegistrar resourceTypeRegistrar)
        {
            this.resourceTypeRegistrar = resourceTypeRegistrar;
        }

        public CompilationContext Create(string text)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            var parser = new Parser(text);
            var program = parser.Program();

            var compilation = new Compilation(resourceTypeRegistrar, program);

            return new CompilationContext(compilation, lineStarts);
        }
    }
}

