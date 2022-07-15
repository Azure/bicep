// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class ParamsSemanticModel
    {
        public BicepParamFile bicepParamFile { get; }

        public ParamBinder paramBinder { get; }

        public Compilation? bicepCompilation { get;  set;}
        public ParamsSemanticModel(BicepParamFile bicepParamFile, Compilation? bicepCompilation = null)
        {
            this.bicepParamFile = bicepParamFile;
            this.paramBinder = new(bicepParamFile); 
            this.bicepCompilation = bicepCompilation;
        }

        public IEnumerable<IDiagnostic> GetDiagnostics()
            => bicepParamFile.ProgramSyntax.GetParseDiagnostics();
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.paramBinder.ParamFileSymbol;
    }
}
