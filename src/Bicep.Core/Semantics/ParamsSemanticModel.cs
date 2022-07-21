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
            // QUESTIONS: #1 loops through .bicep params first, then .bicepparam params - #2 and #3 do it the other way around
            // ErrorType in #3?
            
            // TODOS: optimize n^2 loops
            // create new ErrorDiagnostics
            // remove many comments

            var semanticDiagnostics = new List<IDiagnostic>();

            // 1. For each parameter without a default value declaration in .bicep file, check if there is an assigned parameter in the .bicepparam file (parameter metadata 'required' boolean)
            if (this.BicepCompilation is null)
            {
                return Enumerable.Empty<IDiagnostic>();
            }
            // parameters from .bicep file
            var requiredParameters = this.BicepCompilation.GetEntrypointSemanticModel().Parameters.Where(x => x.IsRequired);
            var parameterAssignmentSyntax = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>();

            // TODO: double for loops are inefficient, will optimize
            
            // iterate over declared parms in .bicep files
            foreach (var requiredParameter in requiredParameters)
            {
               // iterate over assigned params in .bicepparam file to see if the name can be found in the list
               var found = false;
               foreach (var syntax in parameterAssignmentSyntax)
               {
                    var parameterAssignmentSymbol = this.ParamBinder.GetSymbolInfo(syntax);

                    if (LanguageConstants.IdentifierComparer.Equals(requiredParameter.Name, parameterAssignmentSymbol?.Name))
                    {
                        found = true;
                        break;
                    }
               }

               if (!found)
               {
                // if not, add error
                semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(new TextSpan(0,0)).MissingParameterAssignment());
               }
            }

            // optimized version of 1
            // var sortedRequiredParameters =  requiredParameters.OrderBy(x => x.Name);
            // var sortedParameterAssignmentSyntax = parameterAssignmentSyntax.OrderBy(x => this.ParamBinder.GetSymbolInfo(x)?.Name);

            // var i = 0;
            // var j = 0;

            // while (i < sortedRequiredParameters.Count())
            // {
            //     var bicepParam = sortedRequiredParameters.ElementAt(i);
            //     var paramParam = this.ParamBinder.GetSymbolInfo(sortedParameterAssignmentSyntax.ElementAt(j));
            //     if (LanguageConstants.IdentifierComparer.Equals(bicepParam.Name, paramParam?.Name))
            //     {
            //         i++;
            //         j++;
            //     }
            //     else
            //     {
            //         // add an error to the list
            //         semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(new TextSpan(0,0)).NestedResourceNotAllowedInLoop()); // TODO: fix this error
            //         // if bicepParam comes after paramParam alphabetically, increment j
            //         if (string.Compare(bicepParam.Name, paramParam?.Name) < 0) // TODO: check <0 or >0
            //         {
            //             j++; // indicate on one with lower idx number
            //         }
            //         else
            //         {
            //             // else, increment i
            //             i++;
            //         }
                    
            //     }
            // }


            // 2.1. For each assigned parameter in .bicepparam file, check if there is a corresponding parameter declaration in the .bicep file
            foreach (var syntax in parameterAssignmentSyntax)
            {
                var found = false;
                var parameterAssignmentSymbol = this.ParamBinder.GetSymbolInfo(syntax);
                var bicepParameters = this.BicepCompilation.GetEntrypointSemanticModel().Parameters; // TODO: need to filter?
                foreach (var requiredParameter in bicepParameters)
                {
                    if (LanguageConstants.IdentifierComparer.Equals(requiredParameter.Name, parameterAssignmentSymbol?.Name))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(syntax.Span).MissingParameterDeclaration());
                }
            }

            // optimized version of 2.1
            // var x = 0;
            // var y = 0;

            // var bicepParameters = this.BicepCompilation.GetEntrypointSemanticModel().Parameters;
            // var sortedBicepParameters = bicepParameters.OrderBy(x => x.Name);

            // while (x < sortedParameterAssignmentSyntax.Count())
            // {
            //     var paramParam = this.ParamBinder.GetSymbolInfo(sortedParameterAssignmentSyntax.ElementAt(x));
            //     var bicepParam = sortedRequiredParameters.ElementAt(y);
            //     if (LanguageConstants.IdentifierComparer.Equals(bicepParam.Name, paramParam?.Name))
            //     {
            //         i++;
            //         j++;
            //     }
            //     else
            //     {
            //         // add an error to the list
            //         semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(new TextSpan(0,0)).NestedResourceNotAllowedInLoop()); // TODO: fix this error
            //         // if bicepParam comes after paramParam alphabetically, increment y
            //         if (string.Compare(bicepParam.Name, paramParam?.Name) < 0) // TODO: check <0 or >0
            //         {
            //             y++;
            //         }
            //         else
            //         {
            //             // else, increment x
            //             x++;
            //         }
                    
            //     }
            // }


            // 2.2. For each assigned parameter in .bicepparam file, call GetTypeInfo()
            foreach (var syntax in parameterAssignmentSyntax)
            {
                // If it's not ErrorType, call GetDeclaredType()
                if ((ParamsTypeManager.GetTypeInfo(syntax) is not ErrorType) && (ParamsTypeManager.GetDeclaredType(syntax) is { } declaredType) &&
                    (!TypeValidator.AreTypesAssignable(ParamsTypeManager.GetTypeInfo(syntax), declaredType)))
                {
                    semanticDiagnostics.Add(DiagnosticBuilder.ForPosition(syntax.Span).TypeMismatch());
                }
            }
            return semanticDiagnostics;
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
