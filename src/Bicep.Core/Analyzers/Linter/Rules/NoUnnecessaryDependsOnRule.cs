// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnnecessaryDependsOnRule : LinterRuleBase
    {
        public new const string Code = "no-unnecessary-dependson";

        public NoUnnecessaryDependsOnRule() : base(
            code: Code,
            description: CoreResources.NoUnnecessaryDependsOnRuleDescription,
            LinterRuleCategory.BestPractice
        )
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.NoUnnecessaryDependsOnRuleMessage, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var visitor = new ResourceVisitor(this, model, diagnosticLevel);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class ResourceVisitor : AstVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly NoUnnecessaryDependsOnRule parent;
            private readonly Lazy<ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>> inferredDependenciesMap;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public ResourceVisitor(NoUnnecessaryDependsOnRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.inferredDependenciesMap = new(BuildDependencyGraph);
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                if (syntax.TryGetBody() is ObjectSyntax body)
                {
                    VisitResourceOrModuleDeclaration(syntax, body);
                }

                base.VisitModuleDeclarationSyntax(syntax);
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                if (syntax.TryGetBody() is ObjectSyntax body)
                {
                    VisitResourceOrModuleDeclaration(syntax, body);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }

            private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> BuildDependencyGraph()
            {
                var directDependencyGraph = ResourceDependencyVisitor.GetResourceDependencies(
                    model,
                    new() { IgnoreExplicitDependsOn = true });
                var builder = ImmutableDictionary.CreateBuilder<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();

                foreach (var kvp in directDependencyGraph)
                {
                    if (kvp.Key is VariableSymbol)
                    {
                        continue;
                    }

                    var allDependencies = ImmutableHashSet.CreateBuilder<ResourceDependency>();
                    Queue<ResourceDependency> dependenciesToWalk = new(kvp.Value);

                    while (dependenciesToWalk.TryDequeue(out var dependency))
                    {
                        if (allDependencies.Contains(dependency))
                        {
                            continue;
                        }

                        if (directDependencyGraph.TryGetValue(dependency.Resource, out var transitiveDependencies))
                        {
                            dependenciesToWalk.EnqueueRange(transitiveDependencies);
                        }

                        if (!dependency.WeakReference ||
                            dependency.Resource is not ResourceSymbol r ||
                            !r.DeclaringResource.IsExistingResource())
                        {
                            allDependencies.Add(dependency with { WeakReference = false });
                        }
                    }

                    builder[kvp.Key] = allDependencies.ToImmutable();
                }

                return builder.ToImmutable();
            }

            private void VisitResourceOrModuleDeclaration(SyntaxBase declaringSyntax, ObjectSyntax body)
            {
                var dependsOnProperty = body.TryGetPropertyByName(LanguageConstants.ResourceDependsOnPropertyName);
                if (dependsOnProperty?.Value is not ArraySyntax declaredDependencies ||
                    model.GetSymbolInfo(declaringSyntax) is not DeclaredSymbol thisResource ||
                    // If this resource has no implicit dependencies, than all explicit dependsOn entries must be valid, so don't bother checking
                    !inferredDependenciesMap.Value.TryGetValue(thisResource, out var inferredDependencies))
                {
                    return;
                }

                HashSet<Symbol> explicitDependencies = new();

                foreach (ArrayItemSyntax declaredDependency in declaredDependencies.Items)
                {
                    var symbolInfo = model.GetSymbolInfo(declaredDependency.Value);
                    if (symbolInfo is not DeclaredSymbol referent ||
                        // Ignore dependsOn entries pointing to a resource collection - dependency analysis would
                        // be complex and user probably knows what they're doing.
                        (symbolInfo as ResourceSymbol)?.IsCollection is true ||
                        (symbolInfo as ModuleSymbol)?.IsCollection is true)
                    {
                        continue;
                    }

                    if (!inferredDependencies.Any(d => d.Resource == referent) && explicitDependencies.Add(referent))
                    {
                        // There are no inferred dependencies - the dependsOn entry is valid
                        continue;
                    }

                    CodeReplacement codeReplacement = CodeReplacement.Nil;
                    if (declaredDependencies.Items.Count() == 1)
                    {
                        // we only have one entry - remove the whole dependsOn property
                        if (SyntaxModifier.TryRemoveProperty(body, dependsOnProperty, model.ParsingErrorLookup) is { } newObject)
                        {
                            codeReplacement = new CodeReplacement(body.Span, newObject.ToString());
                        }
                    }
                    else
                    {
                        // we have multiple entries - just remove this one
                        if (SyntaxModifier.TryRemoveItem(declaredDependencies, declaredDependency, model.ParsingErrorLookup) is { } newArray)
                        {
                            codeReplacement = new CodeReplacement(declaredDependencies.Span, newArray.ToString());
                        }
                    }

                    // if the syntax is in an invalid state, we may not have a replacement.
                    // just return a diagnostic and leave it up to the user.
                    if (codeReplacement.IsNil)
                    {
                        this.diagnostics.Add(
                            parent.CreateDiagnosticForSpan(
                                diagnosticLevel,
                                declaredDependency.Span,
                                referent.Name));
                    }
                    else
                    {
                        this.diagnostics.Add(
                            parent.CreateFixableDiagnosticForSpan(
                                diagnosticLevel,
                                declaredDependency.Span,
                                new CodeFix(CoreResources.NoUnnecessaryDependsOnRuleCodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
                                referent.Name));
                    }
                }
            }
        }
    }
}
