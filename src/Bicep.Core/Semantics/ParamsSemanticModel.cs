// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
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
        public List<IDiagnostic> allDiagnostics { get; } = new();
        public IFileResolver fileResolver { get; }
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, IFileResolver fileResolver, Func<Uri, Compilation>? getCompilation = null)
        {
            this.fileResolver = fileResolver;

            //parse using statement and link bicep template 
            this.BicepParamFile = bicepParamFile;

            Uri? bicepFileUri = TryGetBicepFileUri();
            if(bicepFileUri is {} && getCompilation is {})
            {   
                //TODO: move all logic for bicepFileUri outside of constructor
                if(fileResolver.FileExists(bicepFileUri))
                {
                    this.BicepCompilation = getCompilation(bicepFileUri);
                }
                else
                {
                    //TODO: throw warning diagnostics if file path is not valid
                }
            }

            //binder logic
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.BicepParamFile = bicepParamFile;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(this, ParamBinder);
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

        private Uri? TryGetBicepFileUri()
        {
            var usingDeclarations = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(!PathHelper.TryGetUsingPath(usingDeclarations.FirstOrDefault(), out var bicepFilePath, out var failureBuilder))
            {       
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0)));
                this.allDiagnostics.Add(diagnostic);
            }

            Uri.TryCreate(BicepParamFile.FileUri, bicepFilePath, out var bicepFileUri);

            return bicepFileUri;
        }
    }
}
