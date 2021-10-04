// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class SemanticModel : ISemanticModel
    {
        private readonly Lazy<EmitLimitationInfo> emitLimitationInfoLazy;
        private readonly Lazy<SymbolHierarchy> symbolHierarchyLazy;
        private readonly Lazy<ResourceAncestorGraph> resourceAncestorsLazy;
        private readonly Lazy<LinterAnalyzer> linterAnalyzerLazy;
        private readonly Lazy<ImmutableArray<TypeProperty>> parameterTypePropertiesLazy;
        private readonly Lazy<ImmutableArray<TypeProperty>> outputTypePropertiesLazy;

        private readonly Lazy<ImmutableArray<ResourceMetadata>> allResourcesLazy;
        private readonly Lazy<IEnumerable<IDiagnostic>> allDiagnostics;
        private static readonly Regex DisableDiagnosticComment = new Regex(@"\/\* Disable (?<code>(.*?)) \*\/", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex EnableDiagnosticComment = new Regex(@"\/\* Enable (?<code>(.*?)) \*\/", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public SemanticModel(Compilation compilation, BicepFile sourceFile, IFileResolver fileResolver, RootConfiguration configuration)
        {
            Trace.WriteLine($"Building semantic model for {sourceFile.FileUri}");

            Compilation = compilation;
            SourceFile = sourceFile;
            FileResolver = fileResolver;
            Configuration = configuration;

            // create this in locked mode by default
            // this blocks accidental type or binding queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var symbolContext = new SymbolContext(compilation, this);
            SymbolContext = symbolContext;

            Binder = new Binder(compilation.NamespaceProvider, sourceFile, symbolContext);
            TypeManager = new TypeManager(Binder, fileResolver);

            // name binding is done
            // allow type queries now
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

            // lazy loading the linter will delay linter rule loading
            // and configuration loading until the linter is actually needed
            this.linterAnalyzerLazy = new Lazy<LinterAnalyzer>(() => new LinterAnalyzer(configuration));

            this.allResourcesLazy = new Lazy<ImmutableArray<ResourceMetadata>>(() => GetAllResourceMetadata());

            // lazy load single use diagnostic set
            this.allDiagnostics = new Lazy<IEnumerable<IDiagnostic>>(() => AssembleDiagnostics());

            this.parameterTypePropertiesLazy = new Lazy<ImmutableArray<TypeProperty>>(() =>
            {
                var paramTypeProperties = new List<TypeProperty>();

                foreach (var param in this.Root.ParameterDeclarations.DistinctBy(p => p.Name))
                {
                    var typePropertyFlags = TypePropertyFlags.WriteOnly;
                    if (SyntaxHelper.TryGetDefaultValue(param.DeclaringParameter) == null)
                    {
                        // if there's no default value, it must be specified
                        typePropertyFlags |= TypePropertyFlags.Required;
                    }

                    var description = SemanticModelHelper.TryGetDescription(this, param.DeclaringParameter);
                    paramTypeProperties.Add(new TypeProperty(param.Name, param.Type, typePropertyFlags, description));
                }

                return paramTypeProperties.ToImmutableArray();
            });

            this.outputTypePropertiesLazy = new Lazy<ImmutableArray<TypeProperty>>(() =>
            {
                var outputTypeProperties = new List<TypeProperty>();

                foreach (var output in this.Root.OutputDeclarations.DistinctBy(o => o.Name))
                {
                    var description = SemanticModelHelper.TryGetDescription(this, output.DeclaringOutput);
                    outputTypeProperties.Add(new TypeProperty(output.Name, output.Type, TypePropertyFlags.ReadOnly, description));
                }

                return outputTypeProperties.ToImmutableArray();
            });
        }

        public BicepFile SourceFile { get; }

        public IBinder Binder { get; }

        public ISymbolContext SymbolContext { get; }

        public Compilation Compilation { get; }

        public RootConfiguration Configuration { get; }

        public ITypeManager TypeManager { get; }

        public IFileResolver FileResolver { get; }

        public EmitLimitationInfo EmitLimitationInfo => emitLimitationInfoLazy.Value;

        public ResourceAncestorGraph ResourceAncestors => resourceAncestorsLazy.Value;

        public ResourceMetadataCache ResourceMetadata { get; }

        private LinterAnalyzer LinterAnalyzer => linterAnalyzerLazy.Value;

        public ImmutableArray<TypeProperty> ParameterTypeProperties => this.parameterTypePropertiesLazy.Value;

        public ImmutableArray<TypeProperty> OutputTypeProperties => this.outputTypePropertiesLazy.Value;

        public ImmutableArray<ResourceMetadata> AllResources => allResourcesLazy.Value;

        /// <summary>
        /// Gets all the parser and lexer diagnostics unsorted. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<IDiagnostic> GetParseDiagnostics() => this.Root.Syntax.GetParseDiagnostics();

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IDiagnostic> GetSemanticDiagnostics()
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
        public IReadOnlyList<IDiagnostic> GetAnalyzerDiagnostics()
        {
            var diagnostics = LinterAnalyzer.Analyze(this);

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            diagnosticWriter.WriteMultiple(diagnostics);

            return diagnosticWriter.GetDiagnostics();
        }

        /// <summary>
        /// Cached diagnostics from compilation
        /// </summary>
        public IEnumerable<IDiagnostic> GetAllDiagnostics()
        {
            var spansToIgnore = GetSpansToIgnore();
            List<IDiagnostic> updatedDiagnostics = new List<IDiagnostic>();

            foreach (IDiagnostic diagnostic in AssembleDiagnostics())
            {
                if (spansToIgnore.TryGetValue(diagnostic.Code, out List<TextSpan> spans) &&
                    spans.Any(x => x.ContainsInclusive(diagnostic.Span.Position) &&
                    x.ContainsInclusive(diagnostic.Span.Position + diagnostic.Span.Length)))
                {
                    continue;
                }
                else
                {
                    updatedDiagnostics.Add(diagnostic);
                }
            }

            return updatedDiagnostics;
        }

        protected Dictionary<string, List<TextSpan>> GetSpansToIgnore()
        {
            Dictionary<string, List<int>> disableCommentsMap = new();
            Dictionary<string, List<TextSpan>> spansToIgnore = new();

            foreach (var child in this.SourceFile.ProgramSyntax.Children)
            {
                if (child is Token childToken)
                {
                    if (TokenContainsCommentPattern(DisableDiagnosticComment, childToken, out string? code) && code is not null)
                    {
                        if (disableCommentsMap.TryGetValue(code, out List<int> value))
                        {
                            value.Add(childToken.Span.Position);
                            disableCommentsMap[code] = value;
                        }
                        else
                        {
                            disableCommentsMap.Add(code, new List<int>() { childToken.Span.Position });
                        }
                    }

                    if (TokenContainsCommentPattern(EnableDiagnosticComment, childToken, out code) && code is not null)
                    {
                        if (disableCommentsMap.TryGetValue(code, out List<int> value) && value.Any())
                        {
                            int spanStart = value.First();
                            TextSpan span = new TextSpan(spanStart, childToken.Span.Position - spanStart);
                            value.RemoveAt(0);
                            disableCommentsMap[code] = value;

                            if (spansToIgnore.TryGetValue(code, out List<TextSpan> textSpans))
                            {
                                textSpans.Add(span);
                                spansToIgnore[code] = textSpans;
                            }
                            else
                            {
                                spansToIgnore.Add(code, new List<TextSpan> { span });
                            }
                        }
                    }
                }
            }

            return spansToIgnore;
        }

        private bool TokenContainsCommentPattern(Regex commentPattern, Token token, out string? code)
        {
            code = null;
            foreach (var syntaxTrivia in token.LeadingTrivia)
            {
                if (TryGetCode(commentPattern, syntaxTrivia.Text, out code))
                {
                    return true;
                }
            }

            foreach (var syntaxTrivia in token.TrailingTrivia)
            {
                if (TryGetCode(commentPattern, syntaxTrivia.Text, out code))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetCode(Regex regex, string text, [NotNullWhen(true)] out string? code)
        {
            code = null;
            if (regex.Matches(text).FirstOrDefault() is Match match &&
                match.Groups["code"].Value is string diagnosticCode)
            {
                code = diagnosticCode;
                return true;
            }

            return false;
        }

        private IEnumerable<IDiagnostic> AssembleDiagnostics()
        {
            return GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .Concat(GetAnalyzerDiagnostics())
            .OrderBy(diag => diag.Span.Position);
        }

        /// <summary>
        /// Immediately runs diagnostics and returns true if any errors are detected
        /// </summary>
        /// <returns>True if analysis finds errors</returns>
        public bool HasErrors()
            => allDiagnostics.Value.Any(x => x.Level == DiagnosticLevel.Error);

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
        /// Unusued declarations will return 1 result. Unused and undeclared symbols (functions, namespaces, for example) may return an empty list.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol)
            => SyntaxAggregator.Aggregate(this.SourceFile.ProgramSyntax, new List<SyntaxBase>(), (accumulated, current) =>
                {
                    if (object.ReferenceEquals(symbol, this.GetSymbolInfo(current)))
                    {
                        accumulated.Add(current);
                    }

                    return accumulated;
                },
                accumulated => accumulated);

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

            return resources.ToImmutable();
        }

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root => this.Binder.FileSymbol;

        public ResourceScope TargetScope => this.Binder.TargetScope;
    }
}
