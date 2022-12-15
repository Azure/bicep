// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnnecessaryDependsOnRule : LinterRuleBase
    {
        public new const string Code = "no-unnecessary-dependson";

        public NoUnnecessaryDependsOnRule() : base(
            code: Code,
            description: CoreResources.NoUnnecessaryDependsOnRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}")
        )
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.NoUnnecessaryDependsOnRuleMessage, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            Lazy<ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>> inferredDependenciesMap =
                new(
                    () => ResourceDependencyVisitor.GetResourceDependencies(
                        model,
                        new ResourceDependencyVisitor.Options { IgnoreExplicitDependsOn = true }));
            var visitor = new ResourceVisitor(this, inferredDependenciesMap, model, diagnosticLevel);
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

            public ResourceVisitor(NoUnnecessaryDependsOnRule parent, Lazy<ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>> inferredDependenciesMap, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.inferredDependenciesMap = inferredDependenciesMap;
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

                foreach (ArrayItemSyntax declaredDependency in declaredDependencies.Items)
                {
                    if (model.GetSymbolInfo(declaredDependency.Value) is not ResourceSymbol referencedResource ||
                        // Ignore dependsOn entries pointing to a resource collection - dependency analyis would
                        // be complex and user probably knows what they're doing.
                        referencedResource.IsCollection)
                    {
                        continue;
                    }

                    if (!inferredDependencies.Any(d => d.Resource == referencedResource))
                    {
                        // There are no inferred dependencies - the dependsOn entry is valid
                        continue;
                    }

                    CodeReplacement? codeReplacement = null;
                    if (declaredDependencies.Items.Count() == 1)
                    {
                        // we only have one entry - remove the whole dependsOn property
                        if (SyntaxModifier.TryRemoveProperty(body, dependsOnProperty) is {} newObject)
                        {
                            codeReplacement = new CodeReplacement(body.Span, newObject.ToTextPreserveFormatting());
                        }
                    }
                    else
                    {
                        // we have multiple entries - just remove this one
                        if (SyntaxModifier.TryRemoveItem(declaredDependencies, declaredDependency) is {} newArray)
                        {
                            codeReplacement = new CodeReplacement(declaredDependencies.Span, newArray.ToTextPreserveFormatting());
                        }
                    }

                    // if the syntax is in an invald state, we may not have a replacement.
                    // just return a diagnostic and leave it up to the user.
                    if (codeReplacement is null)
                    {
                        this.diagnostics.Add(
                            parent.CreateDiagnosticForSpan(
                                diagnosticLevel,
                                declaredDependency.Span,
                                referencedResource.Name));
                    }
                    else
                    {
                        this.diagnostics.Add(
                            parent.CreateFixableDiagnosticForSpan(
                                diagnosticLevel,
                                declaredDependency.Span,
                                new CodeFix(CoreResources.NoUnnecessaryDependsOnRuleCodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
                                referencedResource.Name));
                    }
                }
            }
        }
    }
}
