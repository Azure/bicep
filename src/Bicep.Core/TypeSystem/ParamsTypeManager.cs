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

            if (paramsSemanticModel.Compilation.TryGetBicepFileSemanticModel() is not {} bicepSemanticModel)
            {
                return null;
            }

            var parameterDeclarations = bicepSemanticModel.Root.ParameterDeclarations;

            foreach (var parameterSymbol in parameterDeclarations)
            {
                if (LanguageConstants.IdentifierComparer.Equals(parameterSymbol.Name, parameterAssignmentSymbol?.Name))
                {
                    return bicepSemanticModel.GetDeclaredType(parameterSymbol.DeclaringParameter);
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
