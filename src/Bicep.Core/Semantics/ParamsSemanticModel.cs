// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
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
        private List<IDiagnostic> semanticDiagnostics = new List<IDiagnostic>();
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, Compilation? bicepCompilation = null)
        {
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.BicepParamFile = bicepParamFile;
            this.BicepCompilation = bicepCompilation;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(this, ParamBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
            
        }

        /// <summary>
        /// Gets all the params semantic diagnostics unsorted. Does not include params parser and params lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDiagnostic> GetAdditionalSemanticDiagnostics()
        {
            if (this.BicepCompilation is null)
            {
                return Enumerable.Empty<IDiagnostic>();
            }
            var requiredParameters = this.BicepCompilation.GetEntrypointSemanticModel().Parameters;
            var parameterAssignmentSyntax = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>();

            var sortedRequiredParameters =  requiredParameters.OrderBy(x => x.Name);
            var sortedParameterAssignments = parameterAssignmentSyntax.OrderBy(x => this.ParamBinder.GetSymbolInfo(x)?.Name);

            // get errors relating to missing declarations or assignments
            AddMissingErrors(sortedRequiredParameters.ToArray(), sortedParameterAssignments.ToArray());
            // get errors relating to type mismatch of params between Bicep and params files
            AddTypeMismatchErrors(sortedParameterAssignments);

            return semanticDiagnostics;
        }

        private void AddMissingErrors(Metadata.ParameterMetadata[] sortedRequiredParameters, ParameterAssignmentSyntax[] sortedParameterAssignments)
        {
            var i = 0;
            var j = 0;

            while ((i < sortedRequiredParameters.Length) && (j < sortedParameterAssignments.Length))
            {
                var bicepParam = sortedRequiredParameters[i];
                var paramParam = this.ParamBinder.GetSymbolInfo(sortedParameterAssignments[j]);
                
                if (LanguageConstants.IdentifierComparer.Equals(bicepParam.Name, paramParam?.Name))
                {
                    i++;
                    j++;
                }
                else if (LanguageConstants.IdentifierComparer.Compare(bicepParam.Name, paramParam?.Name) < 0)
                {
                    if (bicepParam.IsRequired)
                    {
                        semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(new TextSpan(0,0)).MissingParameterAssignment(bicepParam.Name));
                    }
                    i++;
                }
                else
                {
                    semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(sortedParameterAssignments[j].Span).MissingParameterDeclaration(paramParam?.Name));
                    j++;
                }
            }

            // if i or j is at the end, emit errors for all remaining unpaired symbols 
            while (i < sortedRequiredParameters.Count())
            {
                if (sortedRequiredParameters[i].IsRequired)
                {
                    semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(new TextSpan(0,0)).MissingParameterAssignment(sortedRequiredParameters[i].Name));
                }
                i++;
            }
            while (j < sortedParameterAssignments.Count())
            {
                semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(sortedParameterAssignments[j].Span).MissingParameterDeclaration(this.ParamBinder.GetSymbolInfo(sortedParameterAssignments[j])?.Name));
                j++;
            }
        }
        
        private void AddTypeMismatchErrors(IOrderedEnumerable<ParameterAssignmentSyntax> sortedParameterAssignmentSyntax)
        {
            foreach (var syntax in sortedParameterAssignmentSyntax)
            {
                if ((ParamsTypeManager.GetTypeInfo(syntax) is not ErrorType) && (ParamsTypeManager.GetDeclaredType(syntax) is { } declaredType) &&
                    (!TypeValidator.AreTypesAssignable(ParamsTypeManager.GetTypeInfo(syntax), declaredType)))
                {
                    semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(syntax.Span).TypeMismatch(this.ParamBinder.GetSymbolInfo(syntax)?.Name, declaredType, ParamsTypeManager.GetTypeInfo(syntax)));
                }
            }
        }
        
        public IEnumerable<IDiagnostic> GetDiagnostics()
            => BicepParamFile.ProgramSyntax.GetParseDiagnostics()
                .Concat(ParamsTypeManager.GetAllDiagnostics())
                .Concat(GetAdditionalSemanticDiagnostics());
            
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;
    }
}
