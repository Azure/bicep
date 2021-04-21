// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class SemanticModel
    {
        private readonly Lazy<EmitLimitationInfo> emitLimitationInfoLazy;
        private readonly Lazy<SymbolHierarchy> symbolHierarchyLazy;
        private readonly Lazy<ResourceAncestorGraph> resourceAncestorsLazy;

        public SemanticModel(Compilation compilation, SyntaxTree syntaxTree)
        {
            Compilation = compilation;
            SyntaxTree = syntaxTree;

            // create this in locked mode by default
            // this blocks accidental type or binding queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var symbolContext = new SymbolContext(compilation, this);
            SymbolContext = symbolContext;

            Binder = new Binder(syntaxTree, symbolContext);
            TypeManager = new TypeManager(compilation.ResourceTypeProvider, Binder);

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
            this.resourceAncestorsLazy = new Lazy<ResourceAncestorGraph>(() => ResourceAncestorGraph.Compute(syntaxTree, Binder));

        }

        public SyntaxTree SyntaxTree { get; }

        public IBinder Binder { get; }

        public ISymbolContext SymbolContext { get; }

        public Compilation Compilation { get; }

        public ITypeManager TypeManager { get; }

        public EmitLimitationInfo EmitLimitationInfo => emitLimitationInfoLazy.Value;

        public ResourceAncestorGraph ResourceAncestors => resourceAncestorsLazy.Value;

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

            foreach (var missingDeclarationSyntax in this.SyntaxTree.ProgramSyntax.Children.OfType<MissingDeclarationSyntax>())
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
            var linter = new LinterAnalyzer();

            var diagnostics = linter.Analyze(this);
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            diagnosticWriter.WriteMultiple(diagnostics);

            return diagnosticWriter.GetDiagnostics();
        }

        /// <summary>
        /// Gets all the diagnostics sorted by span position ascending. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<IDiagnostic> GetAllDiagnostics() => GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .Concat(GetAnalyzerDiagnostics())
            .OrderBy(diag => diag.Span.Position);

        public bool HasErrors()
            => GetAllDiagnostics().Any(x => x.Level == DiagnosticLevel.Error);

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
        {
            static PropertySymbol? GetPropertySymbol(TypeSymbol? baseType, string property)
            {
                if (baseType is null)
                {
                    return null;
                }

                var typeProperty = TypeAssignmentVisitor.UnwrapType(baseType) switch {
                    ObjectType x => x.Properties.TryGetValue(property, out var tp) ? tp : null,
                    DiscriminatedObjectType x => x.TryGetDiscriminatorProperty(property),
                    _ => null
                };

                if (typeProperty is null)
                {
                    return null;
                }

                return new PropertySymbol(property, typeProperty.Description, typeProperty.TypeReference.Type);
            }

            switch (syntax)
            {
                case InstanceFunctionCallSyntax ifc:
                {
                    var baseType = GetTypeInfo(ifc.BaseExpression);
                    switch (baseType)
                    {
                        case NamespaceType namespaceType when SyntaxTree.Hierarchy.GetParent(ifc) is DecoratorSyntax:
                            return namespaceType.DecoratorResolver.TryGetSymbol(ifc.Name);
                        case ObjectType objectType:
                            return objectType.MethodResolver.TryGetSymbol(ifc.Name);
                    }

                    return null;
                }
                case PropertyAccessSyntax propertyAccess:
                {
                    var baseType = GetDeclaredType(propertyAccess.BaseExpression);
                    var property = propertyAccess.PropertyName.IdentifierName;

                    return GetPropertySymbol(baseType, property);
                }
                case ObjectPropertySyntax objectProperty:
                {
                    if (Binder.GetParent(objectProperty) is not {} parentSyntax)
                    {
                        return null;
                    }
                    
                    var baseType = GetDeclaredType(parentSyntax);
                    if (objectProperty.TryGetKeyText() is not {} property)
                    {
                        return null;
                    }

                    return GetPropertySymbol(baseType, property);
                }
            }

            return this.Binder.GetSymbolInfo(syntax);
        }

        /// <summary>
        /// Returns all syntax nodes that represent a reference to the specified symbol. This includes the definitions of the symbol as well.
        /// Unusued declarations will return 1 result. Unused and undeclared symbols (functions, namespaces, for example) may return an empty list.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol)
            => SyntaxAggregator.Aggregate(this.SyntaxTree.ProgramSyntax, new List<SyntaxBase>(), (accumulated, current) =>
                {
                    if (object.ReferenceEquals(symbol, this.GetSymbolInfo(current)))
                    {
                        accumulated.Add(current);
                    }

                    return accumulated;
                },
                accumulated => accumulated);

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root => this.Binder.FileSymbol;

        public ResourceScope TargetScope => this.Binder.TargetScope;
    }
}
