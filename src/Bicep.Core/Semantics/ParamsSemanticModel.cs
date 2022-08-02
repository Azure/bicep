// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Semantics
{
    public class ParamsSemanticModel
    {
        public BicepParamFile BicepParamFile { get; }
        public ParamBinder ParamBinder { get; }
        public Compilation? BicepCompilation { get; }
        public ParamsTypeManager ParamsTypeManager { get; }
        public ParamsSymbolContext ParamsSymbolContext { get; }
        public Lazy<ImmutableArray<IDiagnostic>> AllDiagnostics { get; }
        private readonly ImmutableArray<IDiagnostic> compilationLoadDiagnostics;
        
        public static async Task<ParamsSemanticModel> Build(
            BicepParamFile bicepParamFile,
            Func<Uri, Task<(Compilation? compilation, ErrorBuilderDelegate? compilationLoadBuilder)>> buildBicepCompilation)
        {
            var diagnostics = ToListDiagnosticWriter.Create();

            Compilation? bicepCompilation = null;
            var fileUri = TryGetBicepFileUri(bicepParamFile, diagnostics);
            if (fileUri is not null)
            {
                (bicepCompilation, var compilationLoadBuilder) = await buildBicepCompilation(fileUri);
                // TODO maybe just output this from TryGetBicepFileUri
                var usingSyntax = bicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>().First();

                if (compilationLoadBuilder is not null)
                {
                    diagnostics.Write(compilationLoadBuilder(DiagnosticBuilder.ForPosition(usingSyntax.Path)));
                }
            }

            return new(bicepParamFile, bicepCompilation, diagnostics.GetDiagnostics().ToImmutableArray());
        }


        public ParamsSemanticModel(BicepParamFile bicepParamFile, Compilation? bicepCompilation, ImmutableArray<IDiagnostic> compilationLoadDiagnostics)
        {

            //parse using statement and link bicep template 
            this.BicepParamFile = bicepParamFile;
            this.BicepCompilation = bicepCompilation;
            this.compilationLoadDiagnostics = compilationLoadDiagnostics;

            //binder logic
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.BicepParamFile = bicepParamFile;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(this, ParamBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
            // lazy load single use diagnostic set
            this.AllDiagnostics = new Lazy<ImmutableArray<IDiagnostic>>(() => GetAllDiagnostics());
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

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var parameters = this.BicepCompilation.GetEntrypointSemanticModel().Parameters;
            var parameterAssignments = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.ParamBinder.GetSymbolInfo(x) is not null);

            // get diagnostics relating to missing parameter assignments or declarations
            GetParameterMismatchDiagnostics(diagnosticWriter, parameters, parameterAssignments);
            // get diagnostics relating to type mismatch of params between Bicep and params files
            GetTypeMismatchDiagnostics(diagnosticWriter, parameterAssignments);

            return diagnosticWriter.GetDiagnostics();
        }
        
        private void GetParameterMismatchDiagnostics(IDiagnosticWriter diagnosticWriter, ImmutableArray<Metadata.ParameterMetadata> parameters, IEnumerable<ParameterAssignmentSyntax> parameterAssignments)
        {
            var parametersDict = parameters.ToDictionary(x => x.Name, LanguageConstants.IdentifierComparer);
            var parametersAssignmentsDict = parameterAssignments.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);
            
            // parameters that are declared but not assigned
            var missingRequiredParams = parametersDict.Values
                .Where(x => x.IsRequired)
                .Where(x => !parametersAssignmentsDict.ContainsKey(x.Name))
                .OrderBy(x => x.Name);
            // parameters that are assigned but not declared
            var missingAssignedParams = parametersAssignmentsDict
                .Where(x => !parametersDict.ContainsKey(x.Key))
                .Select(x => x.Value)
                .OrderBy(x => x.Name.IdentifierName);
            
            foreach (var requiredParam in missingRequiredParams)
            {
                diagnosticWriter.Write(new TextSpan(0, 0), x => x.MissingParameterAssignment(requiredParam.Name));
            }

            foreach (var assignedParam in missingAssignedParams)
            {
                diagnosticWriter.Write(assignedParam.Span, x => x.MissingParameterDeclaration(this.ParamBinder.GetSymbolInfo(assignedParam)?.Name));
            }
        }
        
        private void GetTypeMismatchDiagnostics(IDiagnosticWriter diagnosticWriter, IEnumerable<ParameterAssignmentSyntax> parameterAssignments)
        {
            foreach (var syntax in parameterAssignments)
            {
                if (ParamsTypeManager.GetTypeInfo(syntax) is not ErrorType &&
                    ParamsTypeManager.GetDeclaredType(syntax) is { } declaredType &&
                    !TypeValidator.AreTypesAssignable(ParamsTypeManager.GetTypeInfo(syntax), declaredType))
                {
                    diagnosticWriter.Write(syntax.Span, x => x.ParameterTypeMismatch(this.ParamBinder.GetSymbolInfo(syntax)?.Name, declaredType, ParamsTypeManager.GetTypeInfo(syntax)));
                }
            }
        }

        public ImmutableArray<IDiagnostic> GetAllDiagnostics()
            => BicepParamFile.ProgramSyntax.GetParseDiagnostics()
                .Concat(ParamsTypeManager.GetAllDiagnostics())
                .Concat(this.compilationLoadDiagnostics)
                .Concat(GetAdditionalSemanticDiagnostics()).ToImmutableArray();
            
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;

        private static Uri? TryGetBicepFileUri(BicepParamFile bicepParamFile, IDiagnosticWriter diagnostics)
        {
            diagnostics = ToListDiagnosticWriter.Create();
            var usingDeclarations = bicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(usingDeclarations.FirstOrDefault() is not {} usingDeclaration)
            {
                diagnostics.Write(new TextSpan(0, 0), x => x.UsingDeclarationNotSpecified());
                return null;
            }
            
            if(usingDeclarations.Count() > 1)
            {
                foreach(var declaration in usingDeclarations)
                {
                    diagnostics.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
                }
                return null;
            }

            if(!PathHelper.TryGetUsingPath(usingDeclaration, out var bicepFilePath, out var failureBuilder))
            {       
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(usingDeclaration.Path.Span));
                diagnostics.Write(diagnostic);
                return null;
            }

            if (!Uri.TryCreate(bicepParamFile.FileUri, bicepFilePath, out var bicepFileUri))
            {
                diagnostics.Write(usingDeclaration.Path.Span, x => x.UsingDeclarationReferencesInvalidFile());
                return null; 
            }            

            return bicepFileUri;
        }
    }
}
