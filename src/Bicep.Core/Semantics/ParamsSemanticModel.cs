// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public IFileResolver fileResolver { get; }
        private readonly ImmutableArray<IDiagnostic> compilationLoadDiagnostics;
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, IFileResolver fileResolver, Func<Uri, Compilation>? getCompilation = null)
        {
            this.fileResolver = fileResolver;

            //parse using statement and link bicep template 
            this.BicepParamFile = bicepParamFile;

            Uri? bicepFileUri = TryGetBicepFileUri(out var diagnosticWriter);
            compilationLoadDiagnostics = diagnosticWriter.GetDiagnostics().ToImmutableArray();
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
            => this.compilationLoadDiagnostics.Concat(BicepParamFile.ProgramSyntax.GetParseDiagnostics());
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;

        private Uri? TryGetBicepFileUri(out ToListDiagnosticWriter allDiagnostics)
        {
            allDiagnostics = ToListDiagnosticWriter.Create();
            var usingDeclarations = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(usingDeclarations.FirstOrDefault() is not {} usingDeclaration)
            {
                allDiagnostics.Write(new TextSpan(0, 0), x => x.UsingDeclarationNotSpecified());
                return null;
            }
            
            if(usingDeclarations.Count() > 1)
            {
                foreach(var declaration in usingDeclarations)
                {
                    allDiagnostics.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
                }
                return null;
            }

            if(!PathHelper.TryGetUsingPath(usingDeclaration, out var bicepFilePath, out var failureBuilder))
            {       
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(usingDeclaration.Path.Span));
                allDiagnostics.Write(diagnostic);
                return null;
            }

            if (!Uri.TryCreate(BicepParamFile.FileUri, bicepFilePath, out var bicepFileUri) || !fileResolver.FileExists(bicepFileUri))
            {
                allDiagnostics.Write(usingDeclaration.Path.Span, x => x.UsingDeclarationRefrencesInvalidFile());
                return null; 
            }            

            return bicepFileUri;
        }
    }
}
