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
        public Compilation? bicepCompilation { get; }
        public BicepParamFile bicepParamFile { get; }
        public ParamsTypeManager ParamsTypeManager { get; }
        public ParamBinder paramBinder {get; }
        public ParamsSymbolContext ParamsSymbolContext { get; }
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, Compilation? bicepCompilation = null)
        {
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            
            this.bicepParamFile = bicepParamFile;
            this.bicepCompilation = bicepCompilation;
            this.paramBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(this, paramBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
        }

        public IEnumerable<IDiagnostic> GetDiagnostics()
            => bicepParamFile.ProgramSyntax.GetParseDiagnostics();
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.paramBinder.ParamFileSymbol;
    }
}
