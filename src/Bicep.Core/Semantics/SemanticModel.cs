// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class SemanticModel : ISemanticModel
    {
        private readonly Lazy<EmitLimitationInfo> emitLimitationInfoLazy;
        private readonly Lazy<SymbolHierarchy> symbolHierarchyLazy;
        private readonly Lazy<ResourceAncestorGraph> resourceAncestorsLazy;
        private readonly Lazy<ImmutableDictionary<string, ParameterMetadata>> parametersLazy;
        private readonly Lazy<ImmutableArray<OutputMetadata>> outputsLazy;
        private readonly Lazy<IApiVersionProvider> apiVersionProviderLazy;

        // needed to support param file go to def
        private readonly Lazy<ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>> declarationsByAssignment;
        private readonly Lazy<ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>> assignmentsByDeclaration;

        private readonly Lazy<ImmutableArray<ResourceMetadata>> allResourcesLazy;
        private readonly Lazy<ImmutableArray<DeclaredResourceMetadata>> declaredResourcesLazy;
        private readonly Lazy<ImmutableArray<IDiagnostic>> allDiagnostics;

        public SemanticModel(Compilation compilation, BicepSourceFile sourceFile, IFileResolver fileResolver, IBicepAnalyzer linterAnalyzer, RootConfiguration configuration, IFeatureProvider features)
        {
            Trace.WriteLine($"Building semantic model for {sourceFile.FileUri} ({sourceFile.FileKind})");

            this.Compilation = compilation;
            this.SourceFile = sourceFile;
            this.Configuration = configuration;
            this.Features = features;
            this.FileResolver = fileResolver;

            // create this in locked mode by default
            // this blocks accidental type or binding queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var symbolContext = new SymbolContext(compilation, this);
            this.SymbolContext = symbolContext;
            this.Binder = new Binder(compilation.NamespaceProvider, features, sourceFile, this.SymbolContext);
            this.apiVersionProviderLazy = new Lazy<IApiVersionProvider>(() => new ApiVersionProvider(features, this.Binder.NamespaceResolver.GetAvailableResourceTypes()));
            this.TypeManager = new TypeManager(features, this.Binder, fileResolver, this.ParsingErrorLookup, this.SourceFile.FileKind);

            // name binding is done, allow type queries now
            symbolContext.Unlock();

            this.emitLimitationInfoLazy = new Lazy<EmitLimitationInfo>(() => EmitLimitationCalculator.Calculate(this));
            this.symbolHierarchyLazy = new Lazy<SymbolHierarchy>(() =>
            {
                var hierarchy = new SymbolHierarchy();
                hierarchy.AddRoot(this.Root);

                return hierarchy;
            });
            this.resourceAncestorsLazy = new Lazy<ResourceAncestorGraph>(() => ResourceAncestorGraph.Compute(this));
            this.ResourceMetadata = new ResourceMetadataCache(this);

            LinterAnalyzer = linterAnalyzer;

            this.allResourcesLazy = new Lazy<ImmutableArray<ResourceMetadata>>(() => GetAllResourceMetadata());
            this.declaredResourcesLazy = new Lazy<ImmutableArray<DeclaredResourceMetadata>>(() => this.AllResources.OfType<DeclaredResourceMetadata>().ToImmutableArray());

            this.assignmentsByDeclaration = new Lazy<ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>>(InitializeDeclarationToAssignmentDictionary);
            this.declarationsByAssignment = new Lazy<ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>>(InitializeAssignmentToDeclarationDictionary);

            // lazy load single use diagnostic set
            this.allDiagnostics = new Lazy<ImmutableArray<IDiagnostic>>(() => AssembleDiagnostics());

            this.parametersLazy = new Lazy<ImmutableDictionary<string, ParameterMetadata>>(() =>
            {
                var parameters = ImmutableDictionary.CreateBuilder<string, ParameterMetadata>();

                foreach (var param in this.Root.ParameterDeclarations.DistinctBy(p => p.Name))
                {
                    var description = DescriptionHelper.TryGetFromDecorator(this, param.DeclaringParameter);
                    var isRequired = SyntaxHelper.TryGetDefaultValue(param.DeclaringParameter) == null;
                    if (param.Type is ResourceType resourceType)
                    {
                        // Resource type parameters are a special case, we need to convert to a dedicated
                        // type so we can compare differently for assignment.
                        var type = new UnboundResourceType(resourceType.TypeReference);
                        parameters.Add(param.Name, new ParameterMetadata(param.Name, type, isRequired, description));
                    }
                    else
                    {
                        parameters.Add(param.Name, new ParameterMetadata(param.Name, param.Type, isRequired, description));
                    }
                }

                return parameters.ToImmutable();
            });

            this.outputsLazy = new Lazy<ImmutableArray<OutputMetadata>>(() =>
            {
                var outputs = new List<OutputMetadata>();

                foreach (var output in this.Root.OutputDeclarations.DistinctBy(o => o.Name))
                {
                    var description = DescriptionHelper.TryGetFromDecorator(this, output.DeclaringOutput);
                    if (output.Type is ResourceType resourceType)
                    {
                        // Resource type parameters are a special case, we need to convert to a dedicated
                        // type so we can compare differently for assignment and code generation.
                        var type = new UnboundResourceType(resourceType.TypeReference);
                        outputs.Add(new OutputMetadata(output.Name, type, description));
                    }
                    else
                    {
                        outputs.Add(new OutputMetadata(output.Name, output.Type, description));
                    }
                }

                return outputs.ToImmutableArray();
            });
        }

        public BicepSourceFile SourceFile { get; }

        public BicepSourceFileKind SourceFileKind => this.SourceFile.FileKind;

        public RootConfiguration Configuration { get; }

        public IFeatureProvider Features { get; }

        public IApiVersionProvider ApiVersionProvider =>
            this.apiVersionProviderLazy.Value;

        public IBinder Binder { get; }

        public ISymbolContext SymbolContext { get; }

        public Compilation Compilation { get; }

        public ITypeManager TypeManager { get; }

        public IFileResolver FileResolver { get; }

        public IDiagnosticLookup LexingErrorLookup => this.SourceFile.LexingErrorLookup;

        public IDiagnosticLookup ParsingErrorLookup => this.SourceFile.ParsingErrorLookup;

        public EmitLimitationInfo EmitLimitationInfo => emitLimitationInfoLazy.Value;

        public ResourceAncestorGraph ResourceAncestors => resourceAncestorsLazy.Value;

        public ResourceMetadataCache ResourceMetadata { get; }

        public IBicepAnalyzer LinterAnalyzer { get; }

        public ImmutableDictionary<string, ParameterMetadata> Parameters => this.parametersLazy.Value;

        public ImmutableArray<OutputMetadata> Outputs => this.outputsLazy.Value;

        /// <summary>
        /// Gets the metadata of all resources for the semantic model including parameters and outputs of modules.
        /// </summary>
        public ImmutableArray<ResourceMetadata> AllResources => allResourcesLazy.Value;

        /// <summary>
        /// Gets the metadata of resources declared for the semantic model (using the resource declaration).
        /// Does not include parameters and outputs of modules.
        /// </summary>
        public ImmutableArray<DeclaredResourceMetadata> DeclaredResources => declaredResourcesLazy.Value;

        /// <summary>
        /// Gets all diagnostics raised by loading Bicep config for this template.
        /// </summary>
        private IEnumerable<IDiagnostic> GetConfigDiagnostics()
        {
            foreach (var builderFunc in Configuration.DiagnosticBuilders)
            {
                // This diagnostic does not correspond to any specific location in the template, so just use the first character span.
                yield return builderFunc(DiagnosticBuilder.ForDocumentStart());
            }
        }

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<IDiagnostic> GetSemanticDiagnostics()
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var visitor = new SemanticDiagnosticVisitor(diagnosticWriter);
            visitor.Visit(this.Root);

            foreach (var missingDeclarationSyntax in this.SourceFile.ProgramSyntax.Children.OfType<MissingDeclarationSyntax>())
            {
                // Trigger type checking manually as missing declarations are not bound to any symbol.
                this.TypeManager.GetTypeInfo(missingDeclarationSyntax);
            }

            var typeValidationDiagnostics = TypeManager.GetAllDiagnostics();
            diagnosticWriter.WriteMultiple(typeValidationDiagnostics);
            diagnosticWriter.WriteMultiple(EmitLimitationInfo.Diagnostics);

            return diagnosticWriter.GetDiagnostics();
        }

        /// <summary>
        /// Gets all the analyzer diagnostics unsorted.
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<IDiagnostic> GetAnalyzerDiagnostics()
        {
            var diagnostics = LinterAnalyzer.Analyze(this);

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            diagnosticWriter.WriteMultiple(diagnostics);

            return diagnosticWriter.GetDiagnostics();
        }

        /// <summary>
        /// Cached diagnostics from compilation
        /// </summary>
        public ImmutableArray<IDiagnostic> GetAllDiagnostics() => allDiagnostics.Value;

        private ImmutableArray<IDiagnostic> AssembleDiagnostics()
        {
            var diagnostics = GetConfigDiagnostics()
                .Concat(this.LexingErrorLookup)
                .Concat(this.ParsingErrorLookup)
                .Concat(GetSemanticDiagnostics())
                .Concat(GetAnalyzerDiagnostics())
                // TODO: This could be eliminated if we change the params type checking code to operate more on symbols
                .Concat(GetAdditionalParamsSemanticDiagnostics())
                .OrderBy(diag => diag.Span.Position);
            var filteredDiagnostics = new List<IDiagnostic>();

            var disabledDiagnosticsCache = SourceFile.DisabledDiagnosticsCache;
            foreach (IDiagnostic diagnostic in diagnostics)
            {
                (int diagnosticLine, _) = TextCoordinateConverter.GetPosition(SourceFile.LineStarts, diagnostic.Span.Position);

                if (diagnosticLine == 0 || !diagnostic.CanBeSuppressed())
                {
                    filteredDiagnostics.Add(diagnostic);
                    continue;
                }

                if (disabledDiagnosticsCache.TryGetDisabledNextLineDirective(diagnosticLine - 1) is { } disableNextLineDirectiveEndPositionAndCodes &&
                    disableNextLineDirectiveEndPositionAndCodes.diagnosticCodes.Contains(diagnostic.Code))
                {
                    continue;
                }

                filteredDiagnostics.Add(diagnostic);
            }

            return filteredDiagnostics.ToImmutableArray();
        }

        /// <summary>
        /// Immediately runs diagnostics and returns true if any errors are detected
        /// </summary>
        /// <returns>True if analysis finds errors</returns>
        public bool HasErrors()
            => allDiagnostics.Value.Any(x => x.Level == DiagnosticLevel.Error);

        public bool HasParsingError(SyntaxBase syntax) => this.ParsingErrorLookup.Contains(syntax);

        public TypeSymbol GetTypeInfo(SyntaxBase syntax) => this.TypeManager.GetTypeInfo(syntax);

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.TypeManager.GetDeclaredType(syntax);

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax) => this.TypeManager.GetDeclaredTypeAssignment(syntax);

        public Symbol? GetSymbolParent(Symbol symbol) => this.symbolHierarchyLazy.Value.GetParent(symbol);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax)
            => SymbolHelper.TryGetSymbolInfo(Binder, TypeManager.GetDeclaredType, syntax);

        /// <summary>
        /// Returns all syntax nodes that represent a reference to the specified symbol. This includes the definitions of the symbol as well.
        /// Unused declarations will return 1 result. Unused and undeclared symbols (functions, namespaces, for example) may return an empty list.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol)
            => FindReferences(symbol, this.SourceFile.ProgramSyntax);

        /// <summary>
        /// Returns all syntax nodes that represent a reference to the specified symbol in the given syntax tree.  This includes the definitions
        /// of the symbol as well, if inside the given syntax tree.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="syntaxTree">The syntax tree to traverse</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol, SyntaxBase syntaxTree)
            => SyntaxAggregator.Aggregate(syntaxTree, current =>
            {
                return object.ReferenceEquals(symbol, this.GetSymbolInfo(current));
            });

        private ImmutableArray<ResourceMetadata> GetAllResourceMetadata()
        {
            var resources = ImmutableArray.CreateBuilder<ResourceMetadata>();
            foreach (var resourceSymbol in ResourceSymbolVisitor.GetAllResources(Root))
            {
                if (this.ResourceMetadata.TryLookup(resourceSymbol.DeclaringSyntax) is { } resource)
                {
                    resources.Add(resource);
                }
            }

            foreach (var parameterSymbol in Root.ParameterDeclarations)
            {
                if (this.ResourceMetadata.TryLookup(parameterSymbol.DeclaringSyntax) is { } resource)
                {
                    resources.Add(resource);
                }
            }

            foreach (var moduleSymbol in Root.ModuleDeclarations)
            {
                if (moduleSymbol.TryGetSemanticModel(out var model, out _))
                {
                    foreach (var output in model.Outputs)
                    {
                        this.ResourceMetadata.TryAdd(moduleSymbol, output.Name);
                    }
                }
            }

            return resources.ToImmutable();
        }

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root => this.Binder.FileSymbol;

        public ResourceScope TargetScope => this.Binder.TargetScope;

        public ParameterSymbol? TryGetParameterDeclaration(ParameterAssignmentSymbol parameterAssignmentSymbol) =>
            this.declarationsByAssignment.Value.TryGetValue(parameterAssignmentSymbol, out var parameterSymbol) ? parameterSymbol : null;

        public ParameterAssignmentSymbol? TryGetParameterAssignment(ParameterSymbol parameterSymbol) =>
            this.assignmentsByDeclaration.Value.TryGetValue(parameterSymbol, out var parameterAssignmentSymbol) ? parameterAssignmentSymbol : null;

        private ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?> InitializeDeclarationToAssignmentDictionary()
        {
            if (this.TryGetBicepSemanticModelForParamsFile() is not { } bicepSemanticModel)
            {
                // not a param file or we can't resolve the semantic model via "using"
                return ImmutableDictionary<ParameterSymbol, ParameterAssignmentSymbol?>.Empty;
            }

            var assignmentsByDeclaration = bicepSemanticModel.Root.ParameterDeclarations.ToDictionary(x => x, _ => (ParameterAssignmentSymbol?)null);
            var parameterAssignments = SourceFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.Binder.GetSymbolInfo(x) is not null);
            var assignmentsBySymbolName = parameterAssignments.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in assignmentsByDeclaration.Keys)
            {
                if (assignmentsBySymbolName.TryGetValue(declaration.Name, out var parameterAssignmentSyntax))
                {
                    assignmentsByDeclaration[declaration] = this.Binder.GetSymbolInfo(parameterAssignmentSyntax) as ParameterAssignmentSymbol;
                }
            }
            return assignmentsByDeclaration.ToImmutableDictionary();
        }

        private ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?> InitializeAssignmentToDeclarationDictionary()
        {
            if (this.TryGetBicepSemanticModelForParamsFile() is not { } bicepSemanticModel)
            {
                // not a param file or we can't resolve the semantic model via "using"
                return ImmutableDictionary<ParameterAssignmentSymbol, ParameterSymbol?>.Empty;
            }

            var declarationsByAssignment = this.Binder.FileSymbol.ParameterAssignments.ToDictionary(x => x, _ => (ParameterSymbol?)null);
            var parameterDeclarations = bicepSemanticModel.Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
            var declarationsBySymbolName = parameterDeclarations.ToDictionary(x => x.Name.IdentifierName, LanguageConstants.IdentifierComparer);

            foreach (var declaration in declarationsByAssignment.Keys)
            {
                if (declarationsBySymbolName.TryGetValue(declaration.Name, out var parameterDeclarationSyntax))
                {
                    declarationsByAssignment[declaration] = bicepSemanticModel.GetSymbolInfo(parameterDeclarationSyntax) as ParameterSymbol;
                }
            }
            return declarationsByAssignment.ToImmutableDictionary();
        }

        private SemanticModel? TryGetBicepSemanticModelForParamsFile()
        {
            if (this.SourceFile is BicepParamFile &&
                this.Compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out _))
            {
                return bicepSemanticModel;
            }

            return null;
        }

        /// <summary>
        /// Gets all the params semantic diagnostics unsorted. Does not include params parser and params lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDiagnostic> GetAdditionalParamsSemanticDiagnostics()
        {
            if (this.SourceFile.FileKind != BicepSourceFileKind.ParamsFile)
            {
                // not a param file - no additional diagnostics
                return Enumerable.Empty<IDiagnostic>();
            }

            // try to get the bicep file's semantic model
            if (!this.Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out var failureDiagnostic))
            {
                // failed to resolve using
                return failureDiagnostic.AsEnumerable<IDiagnostic>();
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var parameters = bicepSemanticModel.Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
            var parameterAssignments = SourceFile.ProgramSyntax.Children.OfType<ParameterAssignmentSyntax>().Where(x => this.Binder.GetSymbolInfo(x) is not null);

            // get diagnostics relating to missing parameter assignments or declarations
            WriteParameterMismatchDiagnostics(bicepSemanticModel, diagnosticWriter, parameters, parameterAssignments);

            // get diagnostics relating to type mismatch of params between Bicep and params files
            WriteTypeMismatchDiagnostics(diagnosticWriter, parameterAssignments);

            return diagnosticWriter.GetDiagnostics();
        }

        private void WriteParameterMismatchDiagnostics(SemanticModel bicepSemanticModel, IDiagnosticWriter diagnosticWriter, IEnumerable<ParameterDeclarationSyntax> parameters, IEnumerable<ParameterAssignmentSyntax> parameterAssignments)
        {
            // parameters that are assigned but not declared
            // var missingAssignedParams = new List<ParameterAssignmentSyntax>();
            var missingAssignedParams = parameterAssignments
                .Where(x => this.Binder.GetSymbolInfo(x) is ParameterAssignmentSymbol symbol && this.TryGetParameterDeclaration(symbol) is null);

            // parameters that are declared but not assigned
            var missingRequiredParams = new List<string>();

            foreach (var parameter in parameters)
            {
                if (bicepSemanticModel.Binder.GetSymbolInfo(parameter) is ParameterSymbol symbol && TryGetParameterAssignment(symbol) is null &&
                    bicepSemanticModel.Parameters[parameter.Name.IdentifierName].IsRequired)
                {
                    missingRequiredParams.Add(parameter.Name.IdentifierName);
                }
            }

            // emit diagnostic only if there is a using statement
            var usingDeclarationSyntax = this.Root.UsingDeclarationSyntax;
            if (usingDeclarationSyntax is not null && missingRequiredParams.Any())
            {
                diagnosticWriter.Write(usingDeclarationSyntax.Path, x => x.MissingParameterAssignment(missingRequiredParams));
            }

            foreach (var assignedParam in missingAssignedParams)
            {
                diagnosticWriter.Write(assignedParam.Span, x => x.MissingParameterDeclaration(this.Binder.GetSymbolInfo(assignedParam)?.Name));
            }
        }

        private void WriteTypeMismatchDiagnostics(IDiagnosticWriter diagnosticWriter, IEnumerable<ParameterAssignmentSyntax> parameterAssignments)
        {
            foreach (var syntax in parameterAssignments)
            {
                if (TypeManager.GetTypeInfo(syntax) is not ErrorType &&
                    TypeManager.GetDeclaredType(syntax) is { } declaredType &&
                    !TypeValidator.AreTypesAssignable(TypeManager.GetTypeInfo(syntax), declaredType))
                {
                    diagnosticWriter.Write(syntax.Span, x => x.ParameterTypeMismatch(this.Binder.GetSymbolInfo(syntax)?.Name, declaredType, TypeManager.GetTypeInfo(syntax)));
                }
            }
        }
    }
}
