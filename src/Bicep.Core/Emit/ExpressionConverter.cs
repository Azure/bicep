// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        private readonly ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements;

        private readonly OperationConverter operationConverter;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, Operation>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
            this.operationConverter = new OperationConverter(context, localReplacements);
        }

        public LanguageExpression ConvertExpression(SyntaxBase syntax)
            => ConvertOperation(operationConverter.ConvertSyntax(syntax));

        private ExpressionConverter GetConverter(IndexReplacementContext? replacementContext)
        {
            if (replacementContext is not null)
            {
                return new(this.context, replacementContext.LocalReplacements);
            }

            return this;
        }

        public ExpressionConverter CreateConverterForIndexReplacement(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
            => GetConverter(
                TryGetReplacementContext(nameSyntax, indexExpression, newContext));

        public IndexReplacementContext? TryGetReplacementContext(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
            => operationConverter.TryGetReplacementContext(resource, indexExpression, newContext);

        public IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
            => operationConverter.TryGetReplacementContext(nameSyntax, indexExpression, newContext);

        public Operation GetExpressionOperation(SyntaxBase syntax)
            => operationConverter.ConvertSyntax(syntax);

        public ProgramOperation ConvertProgram(FileSymbol fileSymbol)
            => operationConverter.ConvertProgram(fileSymbol);

        public IEnumerable<ObjectPropertyOperation> GetDecorators(StatementSyntax statement, TypeSymbol targetType)
            => operationConverter.GetDecorators(statement, targetType);

        public LanguageExpression ConvertOperation(Operation operation)
        {
            switch (operation)
            {
                case ConstantValueOperation op:
                    {
                        return op.Value switch
                        {
                            string value => new JTokenExpression(value),
                            long value when value <= int.MaxValue && value >= int.MinValue => new JTokenExpression((int)value),
                            long value => CreateFunction("json", new JTokenExpression(value.ToInvariantString())),
                            bool value => CreateFunction(value ? "true" : "false"),
                            _ => throw new NotImplementedException($"Cannot convert constant type {op.Value?.GetType()}"),
                        };
                    }
                case ArrayOperation op:
                    {
                        return CreateFunction(
                            "createArray",
                            op.Items.SelectArray(ConvertOperation));
                    }
                case ObjectOperation op:
                    {
                        return CreateFunction(
                            "createObject",
                            op.Properties.SelectMany(x => new [] { ConvertOperation(x.Key), ConvertOperation(x.Value) }).ToArray());
                    }
                case NullValueOperation op:
                    {
                        return CreateFunction("null");
                    }
                case PropertyAccessOperation op:
                    {
                        return AppendProperties(
                            ToFunctionExpression(ConvertOperation(op.Base)),
                            new JTokenExpression(op.PropertyName));
                    }
                case ArrayAccessOperation op:
                    {
                        return AppendProperties(
                            ToFunctionExpression(ConvertOperation(op.Base)),
                            ConvertOperation(op.Access));
                    }
                case ResourceIdOperation op:
                    {
                        return GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Metadata);
                    }
                case ResourceNameOperation op:
                    {
                        return op.FullyQualified switch {
                            true => GetConverter(op.IndexContext).GetFullyQualifiedResourceName(op.Metadata),
                            false => GetConverter(op.IndexContext).GetUnqualifiedResourceName(op.Metadata),
                        };
                    }
                case ResourceTypeOperation op:
                    {
                        return new JTokenExpression(op.Metadata.TypeReference.FormatType());
                    }
                case ResourceApiVersionOperation op:
                    {
                        return op.Metadata.TypeReference.ApiVersion switch
                        {
                            { } apiVersion => new JTokenExpression(apiVersion),
                            _ => throw new NotImplementedException(""),
                        };
                    }
                case ResourceInfoOperation op:
                    {
                        return CreateFunction(
                            "resourceInfo",
                            GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext));
                    }
                case ResourceReferenceOperation op:
                    {
                        return (op.Full, op.ShouldIncludeApiVersion) switch
                        {
                            (true, _) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!),
                                new JTokenExpression("full")),
                            (false, false) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId)),
                            (false, true) => CreateFunction(
                                "reference",
                                ConvertOperation(op.ResourceId),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!)),
                        };
                    }
                case SymbolicResourceReferenceOperation op:
                    {
                        return (op.Full, op.Metadata.IsAzResource) switch
                        {
                            (true, true) => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext),
                                new JTokenExpression(op.Metadata.TypeReference.ApiVersion!),
                                new JTokenExpression("full")),
                            _ => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext)),
                        };
                    }
                case ModuleNameOperation op:
                    {
                        return GetConverter(op.IndexContext).GetModuleNameExpression(op.Symbol);
                    }
                case VariableAccessOperation op:
                    {
                        return CreateFunction(
                            "variables",
                            new JTokenExpression(op.Symbol.Name));
                    }
                case ExplicitVariableAccessOperation op:
                    {
                        return CreateFunction(
                            "variables",
                            new JTokenExpression(op.Name));
                    }
                case ParameterAccessOperation op:
                    {
                        return CreateFunction(
                            "parameters",
                            new JTokenExpression(op.Symbol.Name));
                    }
                case ModuleOutputOperation op:
                    {
                        var reference = ConvertOperation(new ModuleReferenceOperation(op.Symbol, op.IndexContext));

                        return AppendProperties(
                            ToFunctionExpression(reference),
                            new JTokenExpression("outputs"),
                            ConvertOperation(op.PropertyName),
                            new JTokenExpression("value"));
                    }
                case ModuleReferenceOperation op:
                    {
                        // See https://github.com/Azure/bicep/issues/6008 for more info
                        var shouldIncludeApiVersion = op.Symbol.DeclaringModule.HasCondition();

                        return (context.Settings.EnableSymbolicNames, shouldIncludeApiVersion) switch
                        {
                            (true, _) => CreateFunction(
                                "reference",
                                GenerateSymbolicReference(op.Symbol.Name, op.IndexContext)),
                            (false, false) => CreateFunction(
                                "reference",
                                GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol)),
                            (false, true) => CreateFunction(
                                "reference",
                                GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol),
                                new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                        };
                    }
                case FunctionCallOperation op:
                    {
                        return CreateFunction(
                            op.Name,
                            op.Parameters.Select(p => ConvertOperation(p)));
                    }
                default:
                    throw new NotImplementedException($"Cannot convert operation of type {operation.GetType().Name}");
            };
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(DeclaredResourceMetadata resource)
        {
            // TODO move this into az extension
            var typeReference = resource.TypeReference;
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameExpression = ConvertExpression(resource.NameSyntax);

            var typesAfterProvider = typeReference.TypeSegments.Skip(1).ToImmutableArray();

            if (ancestors.Length > 0)
            {
                var firstAncestorNameLength = typesAfterProvider.Length - ancestors.Length;

                var parentNames = ancestors.SelectMany((x, i) =>
                {
                    var expression = GetResourceNameAncestorSyntaxSegment(resource, i);
                    var nameExpression = this.ConvertExpression(expression);

                    if (i == 0 && firstAncestorNameLength > 1)
                    {
                        return Enumerable.Range(0, firstAncestorNameLength).Select(
                            (_, i) => AppendProperties(
                                CreateFunction("split", nameExpression, new JTokenExpression("/")),
                                new JTokenExpression(i)));
                    }

                    return nameExpression.AsEnumerable();
                });

                return parentNames.Concat(nameExpression.AsEnumerable());
            }

            if (typesAfterProvider.Length == 1)
            {
                return nameExpression.AsEnumerable();
            }

            return typesAfterProvider.Select(
                (type, i) => AppendProperties(
                    CreateFunction("split", nameExpression, new JTokenExpression("/")),
                    new JTokenExpression(i)));
        }

        /// <summary>
        /// Returns a collection of name segment expressions for the specified resource. Local variable replacements
        /// are performed so the expressions are valid in the language/binding scope of the specified resource.
        /// </summary>
        /// <param name="resource">The resource</param>
        public IEnumerable<SyntaxBase> GetResourceNameSyntaxSegments(DeclaredResourceMetadata resource)
        {
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameExpression = resource.NameSyntax;

            return ancestors
                .Select((x, i) => GetResourceNameAncestorSyntaxSegment(resource, i))
                .Concat(nameExpression);
        }

        /// <summary>
        /// Calculates the expression that represents the parent name corresponding to the specified ancestor of the specified resource.
        /// The expressions returned are modified by performing the necessary local variable replacements.
        /// </summary>
        /// <param name="resource">The declared resource metadata</param>
        /// <param name="startingAncestorIndex">the index of the ancestor (0 means the ancestor closest to the root)</param>
        private SyntaxBase GetResourceNameAncestorSyntaxSegment(DeclaredResourceMetadata resource, int startingAncestorIndex)
        {
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            if (startingAncestorIndex >= ancestors.Length)
            {
                // not enough ancestors
                throw new ArgumentException($"Resource type has {ancestors.Length} ancestor types but name expression was requested for ancestor type at index {startingAncestorIndex}.");
            }

            /*
             * Consider the following example:
             *
             * resource one 'MS.Example/ones@...' = [for (_, i) in range(0, ...) : {
             *   name: name_exp1(i)
             * }]
             *
             * resource two 'MS.Example/ones/twos@...' = [for (_, j) in range(0, ...) : {
             *   parent: one[index_exp2(j)]
             *   name: name_exp2(j)
             * }]
             *
             * resource three 'MS.Example/ones/twos/threes@...' = [for (_, k) in range(0, ...) : {
             *   parent: two[index_exp3(k)]
             *   name: name_exp3(k)
             * }]
             *
             * name_exp* and index_exp* are expressions represented here as functions
             *
             * The name segment expressions for "three" are the following:
             * 0. name_exp1(index_exp2(index_exp3(k)))
             * 1. name_exp2(index_exp3(k))
             * 2. name_exp3(k)
             *
             * (The formula can be generalized to more levels of nesting.)
             *
             * This function can be used to get 0 and 1 above by passing 0 or 1 respectively as the startingAncestorIndex.
             * The name segment 2 above must be obtained from the resource directly.
             *
             * Given that we don't have proper functions in our runtime AND that our expressions don't have side effects,
             * the formula is implemented via local variable replacement.
             */

            // the initial ancestor gives us the base expression
            SyntaxBase? rewritten = ancestors[startingAncestorIndex].Resource.NameSyntax;

            for (int i = startingAncestorIndex; i < ancestors.Length; i++)
            {
                var ancestor = ancestors[i];

                // local variable replacement will be done in context of the next ancestor
                // or the resource itself if we're on the last ancestor
                var newContext = i < ancestors.Length - 1 ? ancestors[i + 1].Resource : resource;

                var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(rewritten, newContext.Symbol.NameSyntax);
                var inaccessibleLocalLoops = inaccessibleLocals.Select(local => operationConverter.GetEnclosingForExpression(local)).Distinct().ToList();

                switch (inaccessibleLocalLoops.Count)
                {
                    case 0 when i == startingAncestorIndex:
                        /*
                         * There are no local vars to replace. It is impossible for a local var to be introduced at the next level
                         * so we can just bail out with the result.
                         *
                         * This path is followed by non-loop resources.
                         *
                         * Case 0 is not possible for non-starting ancestor index because
                         * once we have a local variable replacement, it will propagate to the next levels
                         */
                        return ancestor.Resource.NameSyntax;

                    case 1 when ancestor.IndexExpression is not null:
                        if (LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.context.SemanticModel, rewritten).SingleOrDefault(s => s.LocalKind == LocalKind.ForExpressionItemVariable) is { } loopItemSymbol)
                        {
                            // rewrite the straggler from previous iteration
                            // TODO: Nested loops will require DFA on the ForSyntax.Expression
                            rewritten = SymbolReplacer.Replace(this.context.SemanticModel, new Dictionary<Symbol, SyntaxBase> { [loopItemSymbol] = SyntaxFactory.CreateArrayAccess(operationConverter.GetEnclosingForExpression(loopItemSymbol).Expression, ancestor.IndexExpression) }, rewritten);
                        }

                        // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                        var @for = inaccessibleLocalLoops.Single();

                        var replacements = inaccessibleLocals.ToDictionary(local => (Symbol)local, local => local.LocalKind switch
                              {
                                  LocalKind.ForExpressionIndexVariable => ancestor.IndexExpression,
                                  LocalKind.ForExpressionItemVariable => SyntaxFactory.CreateArrayAccess(@for.Expression, ancestor.IndexExpression),
                                  _ => throw new NotImplementedException($"Unexpected local kind '{local.LocalKind}'.")
                              });

                        rewritten = SymbolReplacer.Replace(this.context.SemanticModel, replacements, rewritten);

                        break;

                    default:
                        throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index expression rewriting.");
                }
            }

            return rewritten;
        }

        public LanguageExpression GetFullyQualifiedResourceName(ResourceMetadata resource)
        {
            switch (resource)
            {
                case DeclaredResourceMetadata declaredResource:
                    var nameValueSyntax = declaredResource.NameSyntax;

                    // For a nested resource we need to compute the name
                    var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(declaredResource);
                    if (ancestors.Length == 0)
                    {
                        return ConvertExpression(nameValueSyntax);
                    }

                    // Build an expression like '${parent.name}/${child.name}'
                    //
                    // This is a call to the `format` function with the first arg as a format string
                    // and the remaining args the actual name segments.
                    //
                    // args.Length = 1 (format string) + N (ancestor names) + 1 (resource name)

                    var nameSegments = GetResourceNameSegments(declaredResource);
                    // {0}/{1}/{2}....
                    var formatString = string.Join("/", nameSegments.Select((_, i) => $"{{{i}}}"));

                    return CreateFunction("format", new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
                case ModuleOutputResourceMetadata:
                case ParameterResourceMetadata:
                    // TODO(antmarti): Can we come up with an expression to get the fully-qualified name here?
                    return GetUnqualifiedResourceName(resource);
                default:
                    throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }

        }

        public LanguageExpression GetUnqualifiedResourceName(ResourceMetadata resource)
        {
            switch (resource)
            {
                case DeclaredResourceMetadata declaredResource:
                    return ConvertExpression(declaredResource.NameSyntax);
                case ModuleOutputResourceMetadata:
                case ParameterResourceMetadata:
                    // create an expression like: `last(split(<resource id>, '/'))`
                    return CreateFunction(
                        "last",
                        CreateFunction(
                            "split",
                            GetFullyQualifiedResourceId(resource),
                            new JTokenExpression("/")));
                default:
                    throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            SyntaxBase nameValueSyntax = OperationConverter.GetModuleNameSyntax(moduleSymbol);
            return ConvertExpression(nameValueSyntax);
        }

        public LanguageExpression GetUnqualifiedResourceId(DeclaredResourceMetadata resource)
        {
            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
                resource.TypeReference.FormatType(),
                GetResourceNameSegments(resource));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ResourceMetadata resource)
        {
            return resource switch {
                ParameterResourceMetadata parameter => ConvertOperation(new ParameterAccessOperation(parameter.Symbol)),
                ModuleOutputResourceMetadata output => ConvertOperation(new ModuleOutputOperation(output.Module, null, new ConstantValueOperation(output.OutputName))),
                DeclaredResourceMetadata declared => ScopeHelper.FormatFullyQualifiedResourceId(
                    context,
                    this,
                    context.ResourceScopeData[declared],
                    resource.TypeReference.FormatType(),
                    GetResourceNameSegments(declared)),
                _ => throw new InvalidOperationException($"Unsupported resource metadata type: {resource}"),
            };
        }

        public LanguageExpression GetFullyQualifiedResourceId(ModuleSymbol moduleSymbol)
        {
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ModuleScopeData[moduleSymbol],
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleSymbol).AsEnumerable());
        }

        /// <summary>
        /// Converts a given language expression into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        public static FunctionExpression ToFunctionExpression(LanguageExpression converted)
        {
            switch (converted)
            {
                case FunctionExpression functionExpression:
                    return functionExpression;

                case JTokenExpression valueExpression:
                    JToken value = valueExpression.Value;

                    switch (value.Type)
                    {
                        case JTokenType.Integer:
                            // convert integer literal to a function call via int() function
                            return CreateFunction("int", valueExpression);

                        case JTokenType.String:
                            // convert string literal to function call via string() function
                            return CreateFunction("string", valueExpression);
                    }

                    break;
            }

            throw new NotImplementedException($"Unexpected expression type '{converted.GetType().Name}'.");
        }

        public LanguageExpression GenerateSymbolicReference(string symbolName, IndexReplacementContext? indexContext)
        {
            if (indexContext is null)
            {
                return new JTokenExpression(symbolName);
            }

            return CreateFunction(
                "format",
                new JTokenExpression($"{symbolName}[{{0}}]"),
                ConvertOperation(indexContext.Index));
        }

        public static LanguageExpression GenerateUnqualifiedResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var typeSegments = fullyQualifiedType.Split("/");

            // Generate a format string that looks like: My.Rp/type1/{0}/type2/{1}
            var formatString = $"{typeSegments[0]}/" + string.Join('/', typeSegments.Skip(1).Select((type, i) => $"{type}/{{{i}}}"));

            return CreateFunction(
                "format",
                new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
        }

        public static LanguageExpression GenerateExtensionResourceId(LanguageExpression scope, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "extensionResourceId",
                new[] { scope, new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupScope(LanguageExpression subscriptionId, LanguageExpression resourceGroup)
            => CreateFunction(
                "format",
                new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                subscriptionId,
                resourceGroup);

        public static LanguageExpression GenerateTenantResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "tenantResourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "resourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public LanguageExpression GenerateManagementGroupResourceId(SyntaxBase managementGroupNameProperty, bool fullyQualified)
        {
            const string managementGroupType = "Microsoft.Management/managementGroups";
            var managementGroupName = ConvertExpression(managementGroupNameProperty);

            if (fullyQualified)
            {
                return GenerateTenantResourceId(managementGroupType, new[] { managementGroupName });
            }
            else
            {
                return GenerateUnqualifiedResourceId(managementGroupType, new[] { managementGroupName });
            }
        }

        /// <summary>
        /// Generates a management group id, using the managementGroup() function. Only suitable for use if the template being generated is targeting the management group scope.
        /// </summary>
        public static LanguageExpression GenerateCurrentManagementGroupId()
            => AppendProperties(
                CreateFunction("managementGroup"),
                new JTokenExpression("id"));

        private static FunctionExpression CreateFunction(string name, params LanguageExpression[] parameters)
            => CreateFunction(name, parameters as IEnumerable<LanguageExpression>);

        private static FunctionExpression CreateFunction(string name, IEnumerable<LanguageExpression> parameters)
            => new(name, parameters.ToArray(), Array.Empty<LanguageExpression>());

        public static FunctionExpression AppendProperties(FunctionExpression function, params LanguageExpression[] properties)
            => AppendProperties(function, properties as IEnumerable<LanguageExpression>);

        public static FunctionExpression AppendProperties(FunctionExpression function, IEnumerable<LanguageExpression> properties)
            => new(function.Function, function.Parameters, function.Properties.Concat(properties).ToArray());
    }
}

