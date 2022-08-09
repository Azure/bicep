// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
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
        public Lazy<ImmutableArray<IDiagnostic>> AllDiagnostics { get; }
        public IFileResolver fileResolver { get; }
        private ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?> DeclarationsByAssignment;
        private ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>? AssignmentsByDeclaration;
        private readonly ImmutableArray<IDiagnostic> compilationLoadDiagnostics;
        
        public ParamsSemanticModel(BicepParamFile bicepParamFile, IFileResolver fileResolver, Func<Uri, Compilation>? getCompilation = null)
        {
            this.fileResolver = fileResolver;

            //parse using statement and link bicep template 
            this.BicepParamFile = bicepParamFile;

            Uri? bicepFileUri = TryGetBicepFileUri(out var diagnosticWriter);
            compilationLoadDiagnostics = diagnosticWriter.GetDiagnostics().ToImmutableArray();
            if(bicepFileUri is {} && getCompilation is {})
            {   
                this.BicepCompilation = getCompilation(bicepFileUri);
            }

            //binder logic
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.BicepParamFile = bicepParamFile;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext); 
            this.ParamsTypeManager = new(this, ParamBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
            this.AssignmentsByDeclaration = InitializeDeclarationToAssignmentDictionary();
            this.DeclarationsByAssignment = InitializeAssignmentToDeclarationDictionary();
            // lazy load single use diagnostic set
            this.AllDiagnostics = new Lazy<ImmutableArray<IDiagnostic>>(() => GetAllDiagnostics());
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

        public ParameterSymbol? GetParameterDeclaration(ParameterAssignmentSymbol parameterAssignmentSymbol)
        {
            return this.DeclarationsByAssignment[parameterAssignmentSymbol];
        }

        public ParameterAssignmentSymbol? GetParameterAssignment(ParameterSymbol parameterSymbol)
        {
            if (this.AssignmentsByDeclaration is null)
            {
                return null;
            }
            return this.AssignmentsByDeclaration[parameterSymbol];
        }

        private ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>? InitializeDeclarationToAssignmentDictionary()
        {
            if (this.BicepCompilation is null)
            {
                return null;
            }

            var assignmentsByDeclaration = this.BicepCompilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.ToDictionary(x => x, _ => (ParameterAssignmentSymbol?)null);
            var parameterAssignments = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.ParamBinder.GetSymbolInfo(x) is not null);
            var assignmentsBySymbolName = parameterAssignments.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in assignmentsByDeclaration.Keys)
            {
                if (assignmentsBySymbolName.TryGetValue(declaration.Name, out var parameterAssignmentSyntax))
                {
                    assignmentsByDeclaration[declaration] = (ParameterAssignmentSymbol?)this.ParamBinder.GetSymbolInfo(parameterAssignmentSyntax);
                }
            }
            return assignmentsByDeclaration.ToImmutableDictionary();
        }

        private ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?> InitializeAssignmentToDeclarationDictionary()
        {
            var declarationsByAssignment = this.ParamBinder.ParamFileSymbol.ParameterAssignmentSymbols.ToDictionary(x => x, _ => (ParameterSymbol?)null);
            if (this.BicepCompilation is null)
            {
                return declarationsByAssignment.ToImmutableDictionary();
            }

            var parameterDeclarations = this.BicepCompilation.GetEntrypointSemanticModel().Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
            var declarationsBySymbolName = parameterDeclarations.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in declarationsByAssignment.Keys)
            {
                if (declarationsBySymbolName.TryGetValue(declaration.Name, out var parameterDeclarationSyntax))
                {
                    declarationsByAssignment[declaration] = (ParameterSymbol?)this.BicepCompilation.GetEntrypointSemanticModel().Binder.GetSymbolInfo(parameterDeclarationSyntax);
                }
            }
            return declarationsByAssignment.ToImmutableDictionary();
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

            var parameters = this.BicepCompilation.GetEntrypointSemanticModel().Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
            var parameterAssignments = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.ParamBinder.GetSymbolInfo(x) is not null);

            // get diagnostics relating to missing parameter assignments or declarations
            GetParameterMismatchDiagnostics(diagnosticWriter, parameters, parameterAssignments);
            // get diagnostics relating to type mismatch of params between Bicep and params files
            GetTypeMismatchDiagnostics(diagnosticWriter, parameterAssignments);

            return diagnosticWriter.GetDiagnostics();
        }
        
        private void GetParameterMismatchDiagnostics(IDiagnosticWriter diagnosticWriter, IEnumerable<ParameterDeclarationSyntax> parameters, IEnumerable<ParameterAssignmentSyntax> parameterAssignments)
        {
            // parameters that are assigned but not declared
            var missingAssignedParams = new List<ParameterAssignmentSyntax>();

            foreach (var parameterAssignment in parameterAssignments)
            {
                if (this.ParamBinder.GetSymbolInfo(parameterAssignment) is ParameterAssignmentSymbol symbol && GetParameterDeclaration(symbol) is null)
                {
                    missingAssignedParams.Add(parameterAssignment);
                }
            }

            // parameters that are declared but not assigned
            var missingRequiredParams = new List<String>();

            

            if (this.BicepCompilation is null)
            {
                return;
            }

            var parameterMetadata = this.BicepCompilation.GetEntrypointSemanticModel().Parameters;
            var metadataByName = parameterMetadata.ToDictionary(x => x.Name, x => x);

            foreach (var parameter in parameters)
            {
                if (this.BicepCompilation.GetEntrypointSemanticModel().Binder.GetSymbolInfo(parameter) is ParameterSymbol symbol && GetParameterAssignment(symbol) is null &&
                    metadataByName[parameter.Name.IdentifierName].IsRequired)
                {
                    missingRequiredParams.Add(parameter.Name.IdentifierName);
                }
            }
            // emit diagnostic only if there is a using statement
            var usingDeclarationSyntax = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>().SingleOrDefault();
            if (usingDeclarationSyntax is not null && missingRequiredParams.Count() > 0)
            {
                diagnosticWriter.Write(usingDeclarationSyntax.Path, x => x.MissingParameterAssignment(missingRequiredParams));
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

        private Uri? TryGetBicepFileUri(out ToListDiagnosticWriter allDiagnostics)
        {
            allDiagnostics = ToListDiagnosticWriter.Create();
            var usingDeclarations = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if(usingDeclarations.FirstOrDefault() is not {} usingDeclaration)
            {
                allDiagnostics.Write(new TextSpan(0, 0), x => x.UsingDeclarationNotSpecified());
                return null;
            }
            
            if(usingDeclarations.Count() > 1)
            {
                foreach(var declaration in usingDeclarations)
                {
                    allDiagnostics.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
                }
                return null;
            }

            if(!PathHelper.TryGetUsingPath(usingDeclaration, out var bicepFilePath, out var failureBuilder))
            {       
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(usingDeclaration.Path.Span));
                allDiagnostics.Write(diagnostic);
                return null;
            }

            if (!Uri.TryCreate(BicepParamFile.FileUri, bicepFilePath, out var bicepFileUri) || !fileResolver.FileExists(bicepFileUri))
            {
                allDiagnostics.Write(usingDeclaration.Path.Span, x => x.UsingDeclarationReferencesInvalidFile());
                return null; 
            }            

            return bicepFileUri;
        }
    }
}
