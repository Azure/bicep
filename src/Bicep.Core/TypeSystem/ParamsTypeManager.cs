// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class ParamsTypeManager : IParamsTypeManager
    {
        private readonly ParamsTypeAssignmentVisitor paramsTypeAssignmentVisitor;
        private readonly ParamsSemanticModel paramsSemanticModel;

        public ParamsTypeManager(ParamsSemanticModel paramsSemanticModel, ParamBinder binder)
        {
            this.paramsTypeAssignmentVisitor = new ParamsTypeAssignmentVisitor(this, binder);
            this.paramsSemanticModel = paramsSemanticModel; // used to query the symbol for a given syntax node
        }

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax)
        {
            var parameterAssignmentSymbol = paramsSemanticModel.ParamBinder.GetSymbolInfo(syntax);

            var bicepCompilation = paramsSemanticModel.BicepCompilation;
            if (bicepCompilation is null)
            {
                return null;
            }
            var semanticModel = bicepCompilation.GetEntrypointSemanticModel();
            var parameterDeclarations = semanticModel.Root.ParameterDeclarations;

            foreach (var parameterSymbol in parameterDeclarations)
            {
                if (LanguageConstants.IdentifierComparer.Equals(parameterSymbol.Name, parameterAssignmentSymbol?.Name))
                {
                    return semanticModel.GetDeclaredType(parameterSymbol.DeclaringParameter);
                }
            }
            return null;
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => paramsTypeAssignmentVisitor.GetTypeInfo(syntax);

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => paramsTypeAssignmentVisitor.GetAllDiagnostics();
    }
}
