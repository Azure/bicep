// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Semantics;

// for each bicep file:
// - for each param, determine whether it's used in a 'runtime' place
// - for each output, determine parameter dependencies
// - for each output, determine whether it's a DTC

// for each module:
// warn if passing a runtime value to a param which is used in a runtime place
// -> to facilitate this, need to be able to determine whether a particular piece of syntax (the parameter value) is "runtime":
//    module mod 'foo' = {
//      params: { runtime: isThisValueRuntime }
//    }
// warn if using a non-DTC output in a runtime place if:
// - it's a non-DTC
// - it's computed from non-DTC parameters
// -> to facilitate this, need to be able to find usages of e.g. mod.outputs.foo and determine whether it's being used in a module runtime param
//
// plan:
// - for each module param:
//   - determine if it's used in a 'runtime' place inside the module
//   - if so, find which module/resource outputs it references, determine if it'll introduce a runtime dependency
//   - will need to do this analysis in reverse dependency order
public static class StrictModeAnalyzer
{
    public record Result(
        ImmutableDictionary<OutputSymbol, ImmutableArray<ParameterSymbol>> OutputParamReferences,
        ImmutableDictionary<OutputSymbol, bool> OutputHasRuntimeReference,
        ImmutableHashSet<ParameterSymbol> RuntimeParameterUsages);

    public static Result Create(SemanticModel model)
    {
        var outputParamReferences = new Dictionary<OutputSymbol, ImmutableArray<ParameterSymbol>>();
        var outputHasRuntimeReference = new Dictionary<OutputSymbol, bool>();
        foreach (var output in model.Root.OutputDeclarations)
        {
            var references = ReferencesFinder.Analyze(model, output.DeclaringOutput.Value);

            outputParamReferences[output] = references.ParameterDependencies.ToImmutableArray();
            outputHasRuntimeReference[output] = references.ResourceRuntimeDependencies.Any() || references.ModuleRuntimeDependencies.Any();
        }

        var usages = UsagesFinder.Analyze(model);

        return new(
            outputParamReferences.ToImmutableDictionary(),
            outputHasRuntimeReference.ToImmutableDictionary(),
            usages.RuntimeParameterUsages);
    }

    public class ReferencesFinder : AstVisitor
    {
        public record Result(
            ImmutableHashSet<DeclaredResourceMetadata> ResourceRuntimeDependencies,
            ImmutableHashSet<ModuleSymbol> ModuleRuntimeDependencies,
            ImmutableHashSet<ParameterSymbol> ParameterDependencies);

        private readonly SemanticModel model;
        private HashSet<DeclaredResourceMetadata> resourceRuntimeDependencies = new();
        private HashSet<ModuleSymbol> moduleRuntimeDependencies = new();
        private HashSet<ParameterSymbol> parameterDependencies = new();

        public static Result Analyze(SemanticModel model, SyntaxBase syntax)
        {
            var visitor = new ReferencesFinder(model);
            visitor.Visit(syntax);

            return new(
                visitor.resourceRuntimeDependencies.ToImmutableHashSet(),
                visitor.moduleRuntimeDependencies.ToImmutableHashSet(),
                visitor.parameterDependencies.ToImmutableHashSet());
        }

        private ReferencesFinder(SemanticModel model)
        {
            this.model = model;
        }

        private void VisitAccess(SyntaxBase baseExpression, string propertyName, Action visitBase)
        {
            if (model.GetSymbolInfo(baseExpression) is DeclaredSymbol symbol &&
                model.Binder.TryGetCycle(symbol) is not null)
            {
                // cycle - exit early
                return;
            }

            if (model.ResourceMetadata.TryLookup(baseExpression) is DeclaredResourceMetadata resource)
            {
                switch (propertyName)
                {
                    case "id":
                    case "name":
                    case "type":
                    case "apiVersion":
                        break;
                    default:
                        resourceRuntimeDependencies.Add(resource);
                        break;
                }
                
                return;
            }

            if (baseExpression is PropertyAccessSyntax childPropertyAccess &&
                childPropertyAccess.PropertyName.NameEquals("outputs") &&                
                model.GetSymbolInfo(childPropertyAccess.BaseExpression) is ModuleSymbol module &&
                module.TryGetSemanticModel(out var moduleGenericModel, out _) &&
                moduleGenericModel is SemanticModel moduleModel &&
                moduleModel.Root.OutputDeclarations.FirstOrDefault(x => LanguageConstants.IdentifierComparer.Equals(x.Name, propertyName)) is {} moduleOutputSymbol)
            {
                var hasRuntimeReference = moduleModel.StrictModeAnalysis.OutputHasRuntimeReference[moduleOutputSymbol];
                var paramReferences = moduleModel.StrictModeAnalysis.OutputParamReferences[moduleOutputSymbol];
                var paramsObject = module.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName) as ObjectSyntax;

                // For each param the module uses to generate this output, analyze where the param value is generated from
                foreach (var moduleParam in paramReferences)
                {
                    if (paramsObject?.TryGetPropertyByName(moduleParam.Name) is {} paramProperty)
                    {
                        Visit(paramProperty.Value);
                    }
                    else
                    {
                        // TODO set to true if default value is newGuid/utcNow
                        hasRuntimeReference |= false;
                    }
                }

                if (hasRuntimeReference)
                {
                    moduleRuntimeDependencies.Add(module);
                }

                return;
            }

            visitBase();
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            VisitAccess(
                syntax.BaseExpression,
                syntax.PropertyName.IdentifierName,
                () => base.VisitPropertyAccessSyntax(syntax));
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (syntax.IndexExpression is StringSyntax stringIndex &&
                stringIndex.TryGetLiteralValue() is {} propertyName)
            {
                VisitAccess(
                    syntax.BaseExpression,
                    propertyName,
                    () => base.VisitArrayAccessSyntax(syntax));
                return;
            }

            base.VisitArrayAccessSyntax(syntax);
        }

        void VisitVarOrResourceAccess(SyntaxBase syntax)
        {
            if (model.GetSymbolInfo(syntax) is DeclaredSymbol symbol &&
                model.Binder.TryGetCycle(symbol) is not null)
            {
                // cycle - exit early
                return;
            }

            if (model.ResourceMetadata.TryLookup(syntax) is DeclaredResourceMetadata resource)
            {
                resourceRuntimeDependencies.Add(resource);
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case ModuleSymbol module:
                    moduleRuntimeDependencies.Add(module);
                    break;
                case VariableSymbol variable:
                    Visit(variable.Value);
                    break;
                case ParameterSymbol parameter:
                    parameterDependencies.Add(parameter);
                    break;
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            VisitVarOrResourceAccess(syntax);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            VisitVarOrResourceAccess(syntax);
        }
    }

    private class UsagesFinder
    {
        public record Result(
            ImmutableHashSet<ParameterSymbol> RuntimeParameterUsages);

        private readonly SemanticModel model;

        public UsagesFinder(SemanticModel model)
        {
            this.model = model;
        }

        public static Result Analyze(SemanticModel model)
        {
            var finder = new UsagesFinder(model);
            var usages = finder.GetRuntimeParameterUsages();

            return new(
                usages.ToImmutableHashSet());
        }

        private IEnumerable<ParameterSymbol> GetRuntimeParameterUsages()
        {
            foreach (var module in model.Root.ModuleDeclarations)
            {
                foreach (var dependency in GetParameterDependencies(module))
                {
                    yield return dependency;
                }
            }

            foreach (var resource in model.Root.ResourceDeclarations)
            {
                foreach (var dependency in GetParameterDependencies(resource))
                {
                    yield return dependency;
                }
            }
        }

        private IEnumerable<ParameterSymbol> GetParameterDependencies(SyntaxBase? syntax)
            => GetDirectSymbolDependencies(syntax).SelectMany(GetParameterDependencies).Distinct();

        private IEnumerable<ParameterSymbol> GetParameterDependencies(params SyntaxBase?[] syntaxes)
            => syntaxes.SelectMany(GetParameterDependencies).Distinct();

        private ImmutableArray<DeclaredSymbol> GetDirectSymbolDependencies(SyntaxBase? syntax)
        {
            if (syntax is null)
            {
                return ImmutableArray<DeclaredSymbol>.Empty;
            }

            return SyntaxAggregator
                .Aggregate(syntax, x => x is VariableAccessSyntax or ResourceAccessSyntax)
                .Select(x => model.GetSymbolInfo(x) as DeclaredSymbol)
                .WhereNotNull()
                .Distinct()
                .ToImmutableArray();
        }

        private IEnumerable<ParameterSymbol> GetParameterDependencies(DeclaredSymbol symbol)
        {
            // TODO implement proper recursion with caching
            if (model.Binder.TryGetCycle(symbol) is not null)
            {
                return Enumerable.Empty<ParameterSymbol>();
            }

            switch (symbol)
            {
                case ParameterSymbol parameter:
                {
                    return parameter.AsEnumerable();
                }
                case VariableSymbol variable:
                {
                    return GetParameterDependencies(variable.DeclaringVariable.Value);
                }
                case ModuleSymbol module:
                {
                    var dependencies = GetParameterDependencies(
                        module.TryGetBodyPropertyValue("name"),
                        module.TryGetBodyPropertyValue("scope"));

                    if (module.TryGetBodyPropertyValue("params") is ObjectSyntax paramsObject &&
                        module.TryGetSemanticModel(out var moduleGenericModel, out _) &&
                        moduleGenericModel is SemanticModel moduleModel)
                    {
                        foreach (var moduleParam in moduleModel.Root.ParameterDeclarations)
                        {
                            if (moduleModel.StrictModeAnalysis.RuntimeParameterUsages.Contains(moduleParam) &&
                                paramsObject.TryGetPropertyByName(moduleParam.Name) is {} paramProperty)
                            {
                                dependencies = dependencies.Concat(GetParameterDependencies(paramProperty.Value));
                            }
                        }
                    }

                    return dependencies;
                }
                case ResourceSymbol resource:
                {
                    var dependencies = GetParameterDependencies(
                        resource.TryGetBodyPropertyValue("name"),
                        resource.TryGetBodyPropertyValue("scope"),
                        resource.TryGetBodyPropertyValue("parent"));

                    // lexically nested
                    if (model.Binder.GetParent(resource.DeclaringResource) is ResourceDeclarationSyntax parentResourceSyntax &&
                        model.GetSymbolInfo(parentResourceSyntax) is ResourceSymbol parentResource)
                    {
                        dependencies = dependencies.Concat(GetParameterDependencies(parentResource));
                    }

                    dependencies = dependencies.Concat(resource.DeclaringResource.Value switch {
                        IfConditionSyntax @if => GetParameterDependencies(@if.ConditionExpression),
                        ForSyntax { Expression: IfConditionSyntax @if } @for => GetParameterDependencies(@for.Expression, @if.ConditionExpression),
                        ForSyntax @for => GetParameterDependencies(@for.Expression),
                        _ => Enumerable.Empty<ParameterSymbol>(),
                    });

                    return dependencies;
                }
                case OutputSymbol output:
                {
                    return GetParameterDependencies(output.DeclaringOutput.Value);
                }
                case LocalVariableSymbol localVariable:
                {
                    return model.Binder.GetParent(localVariable.DeclaringLocalVariable) switch {
                        ForSyntax @for => GetParameterDependencies(@for.Expression),
                        VariableBlockSyntax block when model.Binder.GetParent(block) is ForSyntax @for => GetParameterDependencies(@for.Expression),
                        // TODO handle lambda syntax?
                        _ => Enumerable.Empty<ParameterSymbol>(),
                    };
                }
            }

            return Enumerable.Empty<ParameterSymbol>();
        }
    }
}