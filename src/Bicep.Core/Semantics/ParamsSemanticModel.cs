// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
        private Lazy<ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>> declarationsByAssignment;
        private Lazy<ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>> assignmentsByDeclaration;
        public BicepParamFile BicepParamFile { get; }
        public ParamBinder ParamBinder { get; }
        public Compilation? BicepCompilation { get; }
        public ParamsTypeManager ParamsTypeManager { get; }
        public ParamsSymbolContext ParamsSymbolContext { get; }
        public Lazy<ImmutableArray<IDiagnostic>> AllDiagnostics { get; }
        private readonly ImmutableArray<IDiagnostic> CompilationLoadDiagnostics;


        public ParamsSemanticModel(BicepParamFile bicepParamFile, ImmutableArray<IDiagnostic> compilationLoadDiagnostics, Compilation? bicepCompilation = null)
        {
            this.BicepParamFile = bicepParamFile;
            this.BicepCompilation = bicepCompilation;
            this.CompilationLoadDiagnostics = compilationLoadDiagnostics;

            //binder logic
            var paramsSymbolContext = new ParamsSymbolContext(this);
            ParamsSymbolContext = paramsSymbolContext;
            this.BicepParamFile = bicepParamFile;
            this.ParamBinder = new(bicepParamFile, paramsSymbolContext);
            this.ParamsTypeManager = new(this, ParamBinder);
            // name binding is done
            // allow type queries now
            paramsSymbolContext.Unlock();
            this.assignmentsByDeclaration = new Lazy<ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>>(() => InitializeDeclarationToAssignmentDictionary());
            this.declarationsByAssignment = new Lazy<ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>>(() => InitializeAssignmentToDeclarationDictionary());
            // lazy load single use diagnostic set
            this.AllDiagnostics = new Lazy<ImmutableArray<IDiagnostic>>(() => GetAllDiagnostics());
        }

        public ImmutableArray<IDiagnostic> GetAllDiagnostics()
            => BicepParamFile.ProgramSyntax.GetParseDiagnostics()
                .Concat(ParamsTypeManager.GetAllDiagnostics())
                .Concat(this.CompilationLoadDiagnostics)
                .Concat(GetAdditionalSemanticDiagnostics()).ToImmutableArray();

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public ParamFileSymbol Root => this.ParamBinder.ParamFileSymbol;

        public ParameterSymbol? TryGetParameterDeclaration(ParameterAssignmentSymbol parameterAssignmentSymbol) =>
            this.declarationsByAssignment.Value.TryGetValue(parameterAssignmentSymbol, out var parameterSymbol) ? parameterSymbol : null;

        public ParameterAssignmentSymbol? TryGetParameterAssignment(ParameterSymbol parameterSymbol) =>
            this.assignmentsByDeclaration.Value.TryGetValue(parameterSymbol, out var parameterAssignmentSymbol) ? parameterAssignmentSymbol : null;

        private ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?> InitializeDeclarationToAssignmentDictionary()
        {
            if (this.BicepCompilation is null)
            {
                return ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>.Empty;
            }

            var assignmentsByDeclaration = this.BicepCompilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.ToDictionary(x => x, _ => (ParameterAssignmentSymbol?)null);
            var parameterAssignments = BicepParamFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.ParamBinder.GetSymbolInfo(x) is not null);
            var assignmentsBySymbolName = parameterAssignments.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in assignmentsByDeclaration.Keys)
            {
                if (assignmentsBySymbolName.TryGetValue(declaration.Name, out var parameterAssignmentSyntax))
                {
                    assignmentsByDeclaration[declaration] = this.ParamBinder.GetSymbolInfo(parameterAssignmentSyntax) as ParameterAssignmentSymbol;
                }
            }
            return assignmentsByDeclaration.ToImmutableDictionary();
        }

        private ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?> InitializeAssignmentToDeclarationDictionary()
        {
            if (this.BicepCompilation is null)
            {
                return ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>.Empty;
            }

            var declarationsByAssignment = this.ParamBinder.ParamFileSymbol.ParameterAssignmentSymbols.ToDictionary(x => x, _ => (ParameterSymbol?)null);
            var parameterDeclarations = this.BicepCompilation.GetEntrypointSemanticModel().Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
            var declarationsBySymbolName = parameterDeclarations.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in declarationsByAssignment.Keys)
            {
                if (declarationsBySymbolName.TryGetValue(declaration.Name, out var parameterDeclarationSyntax))
                {
                    declarationsByAssignment[declaration] = this.BicepCompilation.GetEntrypointSemanticModel().GetSymbolInfo(parameterDeclarationSyntax) as ParameterSymbol;
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
            // var missingAssignedParams = new List<ParameterAssignmentSyntax>();
            var missingAssignedParams = parameterAssignments
                .Where(x => this.ParamBinder.GetSymbolInfo(x) is ParameterAssignmentSymbol symbol && this.TryGetParameterDeclaration(symbol) is null);

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
                if (this.BicepCompilation.GetEntrypointSemanticModel().Binder.GetSymbolInfo(parameter) is ParameterSymbol symbol && TryGetParameterAssignment(symbol) is null &&
                    metadataByName[parameter.Name.IdentifierName].IsRequired)
                {
                    missingRequiredParams.Add(parameter.Name.IdentifierName);
                }
            }

            // emit diagnostic only if there is a using statement
            var usingDeclarationSyntax = BicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>().SingleOrDefault();
            if (usingDeclarationSyntax is not null && missingRequiredParams.Any())
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

        public static Uri? TryGetBicepFileUri(out ImmutableArray<IDiagnostic> compilationLoadDiagnostics, IFileResolver fileResolver, BicepParamFile bicepParamFile)
        {
            var diagnosticsWriter = ToListDiagnosticWriter.Create();
            var usingDeclarations = bicepParamFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>();

            if (usingDeclarations.FirstOrDefault() is not { } usingDeclaration)
            {
                diagnosticsWriter.Write(new TextSpan(0, 0), x => x.UsingDeclarationNotSpecified());
                compilationLoadDiagnostics = diagnosticsWriter.GetDiagnostics().ToImmutableArray();
                return null;
            }

            if (usingDeclarations.Count() > 1)
            {
                foreach (var declaration in usingDeclarations)
                {
                    diagnosticsWriter.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
                }
                compilationLoadDiagnostics = diagnosticsWriter.GetDiagnostics().ToImmutableArray();
                return null;
            }

            if (!ParamsSemanticModel.TryGetUsingPath(usingDeclaration, out var bicepFilePath, out var failureBuilder))
            {
                var diagnostic = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(usingDeclaration.Path.Span));
                diagnosticsWriter.Write(diagnostic);
                compilationLoadDiagnostics = diagnosticsWriter.GetDiagnostics().ToImmutableArray();
                return null;
            }

            if (!Uri.TryCreate(bicepParamFile.FileUri, bicepFilePath, out var bicepFileUri) || !fileResolver.FileExists(bicepFileUri))
            {
                diagnosticsWriter.Write(usingDeclaration.Path.Span, x => x.UsingDeclarationReferencesInvalidFile());
                compilationLoadDiagnostics = diagnosticsWriter.GetDiagnostics().ToImmutableArray();
                return null;
            }

            compilationLoadDiagnostics = diagnosticsWriter.GetDiagnostics().ToImmutableArray();
            return bicepFileUri;
        }

        private static bool TryGetUsingPath(UsingDeclarationSyntax usingDeclarationSyntax, [NotNullWhen(true)] out string? bicepPath, [NotNullWhen(false)] out DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder)
        {
            var pathSyntax = usingDeclarationSyntax.TryGetPath();
            if (pathSyntax == null)
            {
                bicepPath = null;
                failureBuilder = x => x.TemplatePathHasNotBeenSpecified();
                return false;
            }
            var pathValue = pathSyntax.TryGetLiteralValue();
            if (pathValue == null)
            {
                bicepPath = null;
                failureBuilder = x => x.FilePathInterpolationUnsupported();
                return false;
            }
            var trimedPathValue = pathValue.Trim();
            if (trimedPathValue == string.Empty)
            {
                bicepPath = null;
                failureBuilder = x => x.UsingDeclarationReferencesInvalidFile();
                return false;
            }

            bicepPath = trimedPathValue;
            failureBuilder = null;

            return true;
        }
    }
}
