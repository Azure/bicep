// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class ParamSemanticModel
    {
        public BicepParamFile bicepParamFile { get; }

        // private readonly ParamBinder paramBinder; 
        public ParamSemanticModel(BicepParamFile bicepParamFile)
        {
            this.bicepParamFile = bicepParamFile;

            //TODO: Verify if SymbolContext is needed 
            // this.paramBinder = new(bicepParamFile, ); 
        }

        public IEnumerable<IDiagnostic> GetDiagnostics()
        => bicepParamFile.ProgramSyntax.GetParseDiagnostics();
    }
}
