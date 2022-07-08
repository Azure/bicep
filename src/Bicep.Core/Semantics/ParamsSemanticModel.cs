// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class ParamsSemanticModel
    {
        public ParamsSemanticModel(BicepParamFile sourceFile, ISymbolContext symbolContext)
        {
            ParamBinder = new ParamBinder(sourceFile, symbolContext);
        }
            
        public ParamBinder ParamBinder { get; }
        
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;
    }
}
