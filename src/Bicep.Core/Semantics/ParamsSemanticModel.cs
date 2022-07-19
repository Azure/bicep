// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class ParamsSemanticModel
    {
        public BicepParamFile BicepParamFile { get; }
        public ParamBinder ParamBinder { get; }
        public Compilation? BicepCompilation { get; }
        public ParamsTypeManager ParamsTypeManager { get; }
        public ParamsSymbolContext ParamsSymbolContext { get; }
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, Func<Uri, Compilation>? getCompilation = null)
        {
            //parse using statement and link bicep template 
            this.BicepParamFile = bicepParamFile;

            Uri? bicepFileUri = TryGetBicepFileUri();
            if(bicepFileUri is {} && getCompilation is {})
            {
                this.BicepCompilation = getCompilation(bicepFileUri);
            }

            //binder logic
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(ParamBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
        }
        

        public IEnumerable<IDiagnostic> GetDiagnostics()
            => BicepParamFile.ProgramSyntax.GetParseDiagnostics();
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;

        public Uri? TryGetBicepFileUri()
        {
            //TODO: collect all using statment related diagnostics here
            var usingDeclarations = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(!PathHelper.TryGetUsingPath(usingDeclarations.FirstOrDefault(), out var bicepFilePath, out var failureBuilder))
            {                
                var message = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0))).Message;

                throw new BicepException(message);
            }

            Uri.TryCreate(BicepParamFile.FileUri, bicepFilePath, out var bicepFileUri);

            return bicepFileUri;
        }
    }
}
