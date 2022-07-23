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
                this.BicepCompilation = getCompilation(bicepFileUri);
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

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => this.allDiagnostics.Concat(BicepParamFile.ProgramSyntax.GetParseDiagnostics());
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;

        private Uri? TryGetBicepFileUri()
        {
            //TODO: consolidate this into one method
            var usingDeclarations = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(usingDeclarations.Count() == 0)
            {
                this.allDiagnostics.Add(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0)).UsingDeclarationNotSpecified());
                return null;
            }
            
            if(usingDeclarations.Count() > 1)
            {
                this.allDiagnostics.Add(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0)).MoreThenOneUsingDeclarationSpecified());
                return null;
            }

            if(!PathHelper.TryGetUsingPath(usingDeclarations.FirstOrDefault()!, out var bicepFilePath, out var failureBuilder))
            {       
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0)));
                this.allDiagnostics.Add(diagnostic);
                return null;
            }

            Uri.TryCreate(BicepParamFile.FileUri, bicepFilePath, out var bicepFileUri);
 
            if(!fileResolver.FileExists(bicepFileUri!))
            {
                this.allDiagnostics.Add(new DiagnosticBuilder.DiagnosticBuilderInternal(usingDeclarations.FirstOrDefault()!.Path.Span).UsingDeclarationRefrencesInvalidFile());
                return null; 
            }            

            return bicepFileUri;
        }
    }
}
