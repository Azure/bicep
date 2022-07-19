// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
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
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, Compilation? bicepCompilation = null)
        {
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            
            this.BicepParamFile = bicepParamFile;
            this.BicepCompilation = bicepCompilation;
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
    }
}
