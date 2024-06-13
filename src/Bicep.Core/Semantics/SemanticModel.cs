// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class SemanticModel : ISemanticModel
    {
        private readonly Lazy<EmitLimitationInfo> emitLimitationInfoLazy;
        private readonly Lazy<SymbolHierarchy> symbolHierarchyLazy;
        private readonly Lazy<ResourceAncestorGraph> resourceAncestorsLazy;
        private readonly Lazy<ImmutableSortedDictionary<string, ParameterMetadata>> parametersLazy;
        private readonly Lazy<ImmutableSortedDictionary<string, ExportMetadata>> exportsLazy;
        private readonly Lazy<ImmutableArray<OutputMetadata>> outputsLazy;
        private readonly Lazy<IApiVersionProvider> apiVersionProviderLazy;
        private readonly Lazy<EmitterSettings> emitterSettingsLazy;

        // needed to support param file go to def
        private readonly Lazy<ImmutableDictionary<ParameterAssignmentSymbol, ParameterMetadata?>> declarationsByAssignment;
        private readonly Lazy<ImmutableDictionary<ParameterMetadata, ParameterAssignmentSymbol?>> assignmentsByDeclaration;

        private readonly Lazy<ImmutableArray<ResourceMetadata>> allResourcesLazy;
        private readonly Lazy<ImmutableArray<DeclaredResourceMetadata>> declaredResourcesLazy;
        private readonly Lazy<ImmutableArray<IDiagnostic>> allDiagnostics;
        private readonly ConcurrentDictionary<Uri, ResultWithDiagnostic<AuxiliaryFile>> auxiliaryFileCache = new();

        public SemanticModel(Compilation compilation, BicepSourceFile sourceFile)
        {
            this.Compilation = compilation;
            this.SourceFile = sourceFile;
            this.Configuration = compilation.ConfigurationManager.GetConfiguration(sourceFile.FileUri);
            this.Features = compilation.FeatureProviderFactory.GetFeatureProvider(sourceFile.FileUri);
            this.Environment = compilation.Environment;

            TraceBuildOperation(sourceFile, Features, Configuration);

            // create this in locked mode by default
            // this blocks accidental type or binding queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var symbolContext = new SymbolContext(compilation, this);
            // Because import cycles would have been detected and blocked earlier in the compilation, it's fine to allow
            // access to *other* models in the compilation while the symbol context is locked.
            // This allows the binder to create the right kind of symbol for compile-time imports.
            var cycleBlockingModelLookup = ISemanticModelLookup.Excluding(compilation, sourceFile);
            this.SymbolContext = symbolContext;
            this.Binder = new Binder(
                compilation.NamespaceProvider,
                Configuration,
                Features,
                compilation.SourceFileGrouping,
                cycleBlockingModelLookup,
                sourceFile,
                this.SymbolContext);

            // TODO(#13239): ApiVersionProvider is only used by UseRecentApiVersionRule. Coupling the linter with the semantic model is suboptimal. A better approach would be to integrate ApiVersionProvider into IResourceTypeProvider.
            this.apiVersionProviderLazy = new Lazy<IApiVersionProvider>(() => new ApiVersionProvider(Features, this.Binder.NamespaceResolver.GetAvailableAzureResourceTypes()));

            this.TypeManager = new TypeManager(this, this.Binder);

            // name binding is done
            // allow type queries now
            symbolContext.Unlock();

            this.emitterSettingsLazy = new(() => new(this));
            this.emitLimitationInfoLazy = new(() => EmitLimitationCalculator.Calculate(this));
            this.symbolHierarchyLazy = new(() =>
            {
                var hierarchy = new SymbolHierarchy();
                hierarchy.AddRoot(this.Root);

                return hierarchy;
            });
            this.resourceAncestorsLazy = new(() => ResourceAncestorGraph.Compute(this));
            this.ResourceMetadata = new ResourceMetadataCache(this);

            LinterAnalyzer = compilation.LinterAnalyzer;

            this.allResourcesLazy = new(GetAllResourceMetadata);
            this.declaredResourcesLazy = new(() => this.AllResources.OfType<DeclaredResourceMetadata>().ToImmutableArray());

            this.assignmentsByDeclaration = new(InitializeDeclarationToAssignmentDictionary);
            this.declarationsByAssignment = new(InitializeAssignmentToDeclarationDictionary);

            // lazy load single use diagnostic set
            this.allDiagnostics = new(AssembleDiagnostics);

            this.parametersLazy = new(() =>
            {
                var parameters = ImmutableSortedDictionary.CreateBuilder<string, ParameterMetadata>();

                foreach (var param in this.Root.ParameterDeclarations.DistinctBy(p => p.Name))
                {
                    var description = DescriptionHelper.TryGetFromDecorator(this, param.DeclaringParameter);
                    var isRequired = SyntaxHelper.TryGetDefaultValue(param.DeclaringParameter) == null && !TypeHelper.IsNullable(param.Type);
                    if (param.Type is ResourceType resourceType)
                    {
                        // Resource type parameters are a special case, we need to convert to a dedicated
                        // type so we can compare differently for assignment.
                        var type = new UnresolvedResourceType(resourceType.TypeReference);
                        parameters.Add(param.Name, new ParameterMetadata(param.Name, type, isRequired, description));
                    }
                    else
                    {
                        parameters.Add(param.Name, new ParameterMetadata(param.Name, param.Type, isRequired, description));
                    }
                }

                return parameters.ToImmutable();
            });

            this.exportsLazy = new(() => FindExportedTypes().Concat(FindExportedVariables()).Concat(FindExportedFunctions())
                .DistinctBy(export => export.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableSortedDictionary(export => export.Name, export => export, LanguageConstants.IdentifierComparer));

            this.outputsLazy = new(() =>
            {
                var outputs = new List<OutputMetadata>();

                foreach (var output in this.Root.OutputDeclarations.DistinctBy(o => o.Name))
                {
                    var description = DescriptionHelper.TryGetFromDecorator(this, output.DeclaringOutput);
                    if (output.Type is ResourceType resourceType)
                    {
                        // Resource type parameters are a special case, we need to convert to a dedicated
                        // type so we can compare differently for assignment and code generation.
                        var type = new UnresolvedResourceType(resourceType.TypeReference);
                        outputs.Add(new OutputMetadata(output.Name, type, description));
                    }
                    else
                    {
                        outputs.Add(new OutputMetadata(output.Name, output.Type, description));
                    }
                }

                return [.. outputs];
            });
        }

        public ResultWithDiagnostic<AuxiliaryFile> ReadAuxiliaryFile(Uri uri)
            => auxiliaryFileCache.GetOrAdd(uri, Compilation.FileCache.Read);

        public IEnumerable<Uri> GetAuxiliaryFileReferences()
            => auxiliaryFileCache.Keys;

        public bool HasAuxiliaryFileReference(Uri uri)
            => auxiliaryFileCache.ContainsKey(uri);

        private IEnumerable<ExportMetadata> FindExportedTypes() => Root.TypeDeclarations
            .Where(t => t.IsExported())
            .Select(t => new ExportedTypeMetadata(t.Name, t.Type, DescriptionHelper.TryGetFromDecorator(this, t.DeclaringType)));

        private IEnumerable<ExportMetadata> FindExportedVariables() => Root.VariableDeclarations
            .Where(v => v.IsExported())
            .Select(v => new ExportedVariableMetadata(v.Name, v.Type, DescriptionHelper.TryGetFromDecorator(this, v.DeclaringVariable)));

        private IEnumerable<ExportMetadata> FindExportedFunctions() => Root.FunctionDeclarations
            .Where(f => f.IsExported())
            .Select(f => new ExportedFunctionMetadata(f.Name,
                f.Overload.FixedParameters.Select(p => new ExportedFunctionParameterMetadata(p.Name, p.Type, p.Description)).ToImmutableArray(),
                new(f.Overload.TypeSignatureSymbol, null),
                DescriptionHelper.TryGetFromDecorator(this, f.DeclaringFunction)));

        private static void TraceBuildOperation(BicepSourceFile sourceFile, IFeatureProvider features, RootConfiguration configuration)
        {
            var sb = new StringBuilder();

            sb.Append($"Building semantic model for {sourceFile.FileUri} ({sourceFile.FileKind}). ");
            var experimentalFeatures = features.EnabledFeatureMetadata.Select(x => x.name).ToArray();
            if (experimentalFeatures.Any())
            {
                sb.Append($"Experimental features enabled: {string.Join(',', experimentalFeatures)}. ");
            }

            if (configuration.ConfigFileUri is { } configFileUri)
            {
                sb.Append($"Using bicepConfig from {configFileUri}.");
            }
            else
            {
                sb.Append($"Using default built-in bicepconfig.");
            }

            Trace.WriteLine(sb.ToString());
        }

        public BicepSourceFile SourceFile { get; }
        public IEnvironment Environment { get; }

        public BicepSourceFileKind SourceFileKind => this.SourceFile.FileKind;

        public RootConfiguration Configuration { get; }

        public IFeatureProvider Features { get; }

        public IApiVersionProvider ApiVersionProvider =>
            this.apiVersionProviderLazy.Value;

        public IBinder Binder { get; }

        public ISymbolContext SymbolContext { get; }

        public Compilation Compilation { get; }

        public ITypeManager TypeManager { get; }

        public EmitterSettings EmitterSettings => emitterSettingsLazy.Value;

        public IDiagnosticLookup LexingErrorLookup => this.SourceFile.LexingErrorLookup;

        public IDiagnosticLookup ParsingErrorLookup => this.SourceFile.ParsingErrorLookup;

        public EmitLimitationInfo EmitLimitationInfo => emitLimitationInfoLazy.Value;

        public ResourceAncestorGraph ResourceAncestors => resourceAncestorsLazy.Value;

        public ResourceMetadataCache ResourceMetadata { get; }

        public IBicepAnalyzer LinterAnalyzer { get; }

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => this.parametersLazy.Value;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => exportsLazy.Value;

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
                .Distinct()
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

            return [.. filteredDiagnostics];
        }

        /// <summary>
        /// Immediately runs diagnostics and returns true if any errors are detected
        /// </summary>
        /// <returns>True if analysis finds errors</returns>
        public bool HasErrors()
            => allDiagnostics.Value.Any(x => x.Level == DiagnosticLevel.Error);

        public bool HasParsingErrors()
            => this.ParsingErrorLookup.Any(x => x.Level == DiagnosticLevel.Error);

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
                if (moduleSymbol.TryGetSemanticModel().IsSuccess(out var model))
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

        public ParameterMetadata? TryGetParameterMetadata(ParameterAssignmentSymbol parameterAssignmentSymbol) =>
            this.declarationsByAssignment.Value.TryGetValue(parameterAssignmentSymbol, out var parameterMetadata) ? parameterMetadata : null;

        public ParameterAssignmentSymbol? TryGetParameterAssignment(ParameterMetadata parameterMetadata) =>
            this.assignmentsByDeclaration.Value.TryGetValue(parameterMetadata, out var parameterAssignment) ? parameterAssignment : null;

        private ImmutableDictionary<ParameterMetadata, ParameterAssignmentSymbol?> InitializeDeclarationToAssignmentDictionary()
        {
            if (this.TryGetSemanticModelForParamsFile() is not { } usingModel)
            {
                // not a param file or we can't resolve the semantic model via "using"
                return ImmutableDictionary<ParameterMetadata, ParameterAssignmentSymbol?>.Empty;
            }

            var parameterAssignments = Root.ParameterAssignments.ToLookup(x => x.Name, LanguageConstants.IdentifierComparer);

            return usingModel.Parameters.ToImmutableDictionary(
                kvp => kvp.Value,
                kvp => parameterAssignments[kvp.Key].FirstOrDefault());
        }

        private ImmutableDictionary<ParameterAssignmentSymbol, ParameterMetadata?> InitializeAssignmentToDeclarationDictionary()
        {
            if (this.TryGetSemanticModelForParamsFile() is not { } usingModel)
            {
                // not a param file or we can't resolve the semantic model via "using"
                return ImmutableDictionary<ParameterAssignmentSymbol, ParameterMetadata?>.Empty;
            }

            var parameterDeclarations = usingModel.Parameters.ToLookup(x => x.Key, x => x.Value, LanguageConstants.IdentifierComparer);

            return Root.ParameterAssignments.ToImmutableDictionary(
                decl => decl,
                decl => parameterDeclarations[decl.Name].FirstOrDefault());
        }

        private ISemanticModel? TryGetSemanticModelForParamsFile()
        {
            if (this.SourceFile is BicepParamFile && this.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                return usingModel;
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
                return [];
            }

            // try to get the bicep file's semantic model
            if (!this.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var semanticModel, out var failureDiagnostic))
            {
                // failed to resolve using
                return failureDiagnostic.AsEnumerable<IDiagnostic>();
            }

            return
                // get diagnostics relating to missing parameter assignments or declarations
                GatherParameterMismatchDiagnostics(semanticModel)
                // get diagnostics relating to type mismatch of params between Bicep and params files
                .Concat(GatherTypeMismatchDiagnostics());
        }

        private IEnumerable<IDiagnostic> GatherParameterMismatchDiagnostics(ISemanticModel usingModel)
        {
            // parameters that are assigned but not declared
            // var missingAssignedParams = new List<ParameterAssignmentSyntax>();
            var missingAssignedParams = Root.ParameterAssignments.Where(s => TryGetParameterMetadata(s) is null);

            // parameters that are declared but not assigned
            var missingRequiredParams = usingModel.Parameters
                .Where(kvp =>
                {
                    var (parameterName, parameterMetadata) = kvp;

                    if (!usingModel.Parameters.TryGetValue(parameterName, out var md) || !md.IsRequired)
                    {
                        return false;
                    }

                    // consider a parameter to be absent if there was no assignment statement OR if the value `null` was assigned
                    return TryGetParameterAssignment(parameterMetadata) is not { } assignment || assignment.Type is NullType;
                })
                .Select(kvp => kvp.Key)
                .ToImmutableArray();

            // emit diagnostic only if there is a using statement
            var usingDeclarationSyntax = this.Root.UsingDeclarationSyntax;
            if (usingDeclarationSyntax is not null && missingRequiredParams.Any())
            {
                var codeFix = CodeFixHelper.GetCodeFixForMissingBicepParams(Root.Syntax, missingRequiredParams);

                yield return DiagnosticBuilder.ForPosition(usingDeclarationSyntax.Path).MissingParameterAssignment(missingRequiredParams, codeFix);
            }

            foreach (var assignedParam in missingAssignedParams)
            {
                yield return DiagnosticBuilder.ForPosition(assignedParam.DeclaringSyntax).MissingParameterDeclaration(assignedParam.Name);
            }
        }

        private IEnumerable<IDiagnostic> GatherTypeMismatchDiagnostics()
        {
            foreach (var assignmentSymbol in Root.ParameterAssignments)
            {
                if (assignmentSymbol.Type is not ErrorType &&
                    assignmentSymbol.Type is not NullType && // `param x = null` is equivalent to skipping the assignment altogether
                    TypeManager.GetDeclaredType(assignmentSymbol.DeclaringSyntax) is { } declaredType)
                {
                    var diagnostics = ToListDiagnosticWriter.Create();
                    TypeValidator.NarrowTypeAndCollectDiagnostics(TypeManager, Binder, ParsingErrorLookup, diagnostics, assignmentSymbol.DeclaringParameterAssignment.Value, declaredType);
                    foreach (var diagnostic in diagnostics.GetDiagnostics())
                    {
                        yield return diagnostic;
                    }
                }
            }
        }
    }
}
