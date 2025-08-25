// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.Semantics
{
    public class Binder : IBinder
    {
        private readonly BicepSourceFile bicepFile;
        private readonly ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol;
        private readonly ConcurrentDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> symbolsDirectlyReferencedInDeclarations = new();
        private readonly ConcurrentDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> referencedSymbolClosures = new();
        private readonly Stack<DeclaredSymbol> closureCalculationStack = new();

        public Binder(
            INamespaceProvider namespaceProvider,
            IArtifactFileLookup sourceFileLookup,
            ISemanticModelLookup modelLookup,
            BicepSourceFile sourceFile,
            ISymbolContext symbolContext)
        {
            // TODO use lazy or some other pattern for init
            this.bicepFile = sourceFile;
            this.TargetScope = SyntaxHelper.GetTargetScope(sourceFile);

            var namespaceResults = namespaceProvider
                .GetNamespaces(sourceFileLookup, sourceFile, TargetScope)
                .ToImmutableArray();
            this.NamespaceResolver = NamespaceResolver.Create(namespaceResults);

            var fileScope = DeclarationVisitor.GetDeclarations(namespaceResults, sourceFile, symbolContext);

            // Process extends & synthesize 'base' BEFORE name binding so variable accesses to 'base' bind correctly.
            var extendsDeclarations = sourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>().ToImmutableArray();
            bool hasExtends = extendsDeclarations.Any();
            var parentParameterAssignments = ImmutableArray<ParameterAssignmentSymbol>.Empty;

            if (hasExtends)
            {
                foreach (var extendsDeclaration in extendsDeclarations)
                {
                    if (!(sourceFileLookup.TryGetSourceFile(extendsDeclaration).TryUnwrap() is { } extendedFile &&
                        modelLookup.GetSemanticModel(extendedFile) is SemanticModel extendedModel))
                    {
                        continue;
                    }

                    var allParentAssignments = extendedModel.Root.ParameterAssignments;
                    foreach (var assignment in allParentAssignments)
                    {
                        if (!parentParameterAssignments.Any(a => string.Equals(a.Name, assignment.Name, LanguageConstants.IdentifierComparison)))
                        {
                            parentParameterAssignments = parentParameterAssignments.Add(assignment);
                        }
                    }

                    var parentVariables = extendedModel.Root.VariableDeclarations.OfType<VariableSymbol>().ToImmutableArray();

                    var nonConflicting = allParentAssignments.Where(a => !fileScope.Locals.Any(e => string.Equals(e.Name, a.Name, LanguageConstants.IdentifierComparison)));
                    fileScope = fileScope.ReplaceLocals(fileScope.Locals.AddRange(nonConflicting));

                    var nonConflictingVars = parentVariables.Where(v => !fileScope.Locals.Any(e => string.Equals(e.Name, v.Name, LanguageConstants.IdentifierComparison)));
                    fileScope = fileScope.ReplaceLocals(fileScope.Locals.AddRange(nonConflictingVars));
                }

                if (parentParameterAssignments.Any())
                {
                    var localsWithoutOldBase = fileScope.Locals.Where(l => l is not BaseParametersSymbol).ToImmutableArray();
                    fileScope = fileScope.ReplaceLocals(localsWithoutOldBase.Add(new BaseParametersSymbol(symbolContext, parentParameterAssignments)));
                }
            }

            var baseBindings = NameBindingVisitor.GetBindings(sourceFile.ProgramSyntax, NamespaceResolver, fileScope).ToBuilder();

            if (hasExtends && parentParameterAssignments.Any())
            {
                foreach (var parentAssignment in parentParameterAssignments)
                {
                    var valueSyntax = parentAssignment.DeclaringParameterAssignment.Value;

                    var stack = new Stack<SyntaxBase>();
                    stack.Push(valueSyntax);
                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();

                        if (current is VariableAccessSyntax || current == valueSyntax || current is PropertyAccessSyntax || current is ArrayAccessSyntax)
                        {
                            var parentSymbol = parentAssignment.Context.Binder.GetSymbolInfo(current);
                            if (parentSymbol is not null && !baseBindings.ContainsKey(current))
                            {
                                baseBindings[current] = parentSymbol;
                            }
                        }

                        if (current is ObjectSyntax obj)
                        {
                            foreach (var prop in obj.Properties)
                            {
                                stack.Push(prop.Value);
                            }
                        }
                        else if (current is ArraySyntax arr)
                        {
                            foreach (var item in arr.Items)
                            {
                                stack.Push(item.Value);
                            }
                        }
                        else if (current is PropertyAccessSyntax propAccess)
                        {
                            stack.Push(propAccess.BaseExpression);
                        }
                        else if (current is ArrayAccessSyntax arrayAccess)
                        {
                            stack.Push(arrayAccess.BaseExpression);
                            stack.Push(arrayAccess.IndexExpression);
                        }
                        else if (current is FunctionCallSyntaxBase funcCall)
                        {
                            foreach (var argument in funcCall.Arguments)
                            {
                                stack.Push(argument.Expression);
                            }
                        }
                        else if (current is ParenthesizedExpressionSyntax paren)
                        {
                            stack.Push(paren.Expression);
                        }
                        else if (current is TernaryOperationSyntax ternary)
                        {
                            stack.Push(ternary.FalseExpression);
                            stack.Push(ternary.TrueExpression);
                            stack.Push(ternary.ConditionExpression);
                        }
                        else if (current is BinaryOperationSyntax binary)
                        {
                            stack.Push(binary.RightExpression);
                            stack.Push(binary.LeftExpression);
                        }
                        else if (current is UnaryOperationSyntax unary)
                        {
                            stack.Push(unary.Expression);
                        }
                        else if (current is NonNullAssertionSyntax nonNull)
                        {
                            stack.Push(nonNull.BaseExpression);
                        }
                    }
                }

                var inheritedVariables = fileScope.Locals.OfType<VariableSymbol>()
                    .Where(v => !ReferenceEquals(v.Context.SourceFile, sourceFile))
                    .ToImmutableArray();

                foreach (var inheritedVar in inheritedVariables)
                {
                    var stack = new Stack<SyntaxBase>();
                    stack.Push(inheritedVar.DeclaringVariable.Value);
                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();
                        if (current is VariableAccessSyntax || current == inheritedVar.DeclaringVariable.Value || current is PropertyAccessSyntax || current is ArrayAccessSyntax)
                        {
                            var parentSymbol = inheritedVar.Context.Binder.GetSymbolInfo(current);
                            if (parentSymbol is not null && !baseBindings.ContainsKey(current))
                            {
                                baseBindings[current] = parentSymbol;
                            }
                        }

                        if (current is ObjectSyntax obj)
                        {
                            foreach (var prop in obj.Properties)
                            {
                                stack.Push(prop.Value);
                            }
                        }
                        else if (current is ArraySyntax arr)
                        {
                            foreach (var item in arr.Items)
                            {
                                stack.Push(item.Value);
                            }
                        }
                        else if (current is PropertyAccessSyntax propAccess)
                        {
                            stack.Push(propAccess.BaseExpression);
                        }
                        else if (current is ArrayAccessSyntax arrayAccess)
                        {
                            stack.Push(arrayAccess.BaseExpression);
                            stack.Push(arrayAccess.IndexExpression);
                        }
                        else if (current is FunctionCallSyntaxBase funcCall)
                        {
                            foreach (var argument in funcCall.Arguments)
                            {
                                stack.Push(argument.Expression);
                            }
                        }
                        else if (current is ParenthesizedExpressionSyntax paren)
                        {
                            stack.Push(paren.Expression);
                        }
                        else if (current is TernaryOperationSyntax ternary)
                        {
                            stack.Push(ternary.FalseExpression);
                            stack.Push(ternary.TrueExpression);
                            stack.Push(ternary.ConditionExpression);
                        }
                        else if (current is BinaryOperationSyntax binary)
                        {
                            stack.Push(binary.RightExpression);
                            stack.Push(binary.LeftExpression);
                        }
                        else if (current is UnaryOperationSyntax unary)
                        {
                            stack.Push(unary.Expression);
                        }
                        else if (current is NonNullAssertionSyntax nonNull)
                        {
                            stack.Push(nonNull.BaseExpression);
                        }
                    }
                }
            }

            this.Bindings = baseBindings.ToImmutable();
            this.cyclesBySymbol = CyclicCheckVisitor.FindCycles(sourceFile.ProgramSyntax, this.Bindings);

            this.FileSymbol = new FileSymbol(
                symbolContext,
                sourceFile,
                NamespaceResolver,
                fileScope);
        }

        public ResourceScope TargetScope { get; }

        public FileSymbol FileSymbol { get; }

        public NamespaceResolver NamespaceResolver { get; }

        public ImmutableDictionary<SyntaxBase, Symbol> Bindings { get; }

        public SyntaxBase? GetParent(SyntaxBase syntax)
            => bicepFile.Hierarchy.GetParent(syntax);

        public bool IsDescendant(SyntaxBase node, SyntaxBase potentialAncestor)
            => bicepFile.Hierarchy.IsDescendant(node, potentialAncestor);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.Bindings.TryGetValue(syntax);

        public ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol)
            => this.cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle) ? cycle : null;

        public ImmutableHashSet<DeclaredSymbol> GetSymbolsReferencedInDeclarationOf(DeclaredSymbol symbol)
            => symbolsDirectlyReferencedInDeclarations.GetOrAdd(symbol,
                s => [.. SymbolicReferenceCollector.CollectSymbolsReferenced(this, s.DeclaringSyntax).Keys]);

        public ImmutableHashSet<DeclaredSymbol> GetReferencedSymbolClosureFor(DeclaredSymbol symbol)
            => referencedSymbolClosures.GetOrAdd(symbol, CalculateReferencedSymbolClosure);

        private ImmutableHashSet<DeclaredSymbol> CalculateReferencedSymbolClosure(DeclaredSymbol symbol)
        {
            closureCalculationStack.Push(symbol);

            var builder = ImmutableHashSet.CreateBuilder<DeclaredSymbol>();
            foreach (var symbolReferencedInDeclaration in GetSymbolsReferencedInDeclarationOf(symbol))
            {
                builder.Add(symbolReferencedInDeclaration);
                if (!closureCalculationStack.Contains(symbolReferencedInDeclaration))
                {
                    builder.UnionWith(GetReferencedSymbolClosureFor(symbolReferencedInDeclaration));
                }
            }

            closureCalculationStack.Pop();

            return builder.ToImmutable();
        }
    }
}
