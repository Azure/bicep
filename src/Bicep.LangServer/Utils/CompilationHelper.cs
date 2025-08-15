// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Utils
{
    public class CompilationHelper
    {
        private readonly BicepCompiler bicepCompiler;
        private readonly ICompilationManager compilationManager;

        public CompilationHelper(BicepCompiler bicepCompiler, ICompilationManager compilationManager)
        {
            this.bicepCompiler = bicepCompiler;
            this.compilationManager = compilationManager;
        }

        public async Task<Compilation> GetRefreshedCompilation(DocumentUri documentUri)
        {
            // Bicep file could contain load functions like loadTextContent(..). We'll refresh compilation to detect changes in files referenced in load functions.
            compilationManager.RefreshCompilation(documentUri, forceReloadAuxiliaryFiles: true);
            var context = compilationManager.GetCompilation(documentUri);

            // CompilationContext will be null if the file is not open in the editor.
            // E.g. When user right clicks on a file from the explorer context menu without opening the file and invokes build/deploy
            if (context is null)
            {
                return await bicepCompiler.CreateCompilation(documentUri.ToIOUri());
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
