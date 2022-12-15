// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;
        private readonly ExpressionBuilder expressionBuilder;

        private readonly ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, Expression>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
            this.expressionBuilder = new ExpressionBuilder(context, localReplacements);
        }

        public LanguageExpression ConvertModuleParameterTernaryExpression(TernaryOperationSyntax ternary)
        {
            LanguageExpression ConvertParameter(SyntaxBase syntax)
            {
                return syntax switch
                {
                    TernaryOperationSyntax innerTernary => ConvertModuleParameterTernaryExpression(innerTernary),
                    InstanceFunctionCallSyntax instanceFunctionCall when string.Equals(instanceFunctionCall.Name.IdentifierName, "getSecret", LanguageConstants.IdentifierComparison) => ConvertGetSecretFunctionCall(instanceFunctionCall),
                    _ => GetCreateObjectExpression(new JTokenExpression("value"), ConvertExpression(syntax))
                };
            }

            return CreateFunction(
                "if",
                ConvertExpression(ternary.ConditionExpression),
                ConvertParameter(ternary.TrueExpression),
                ConvertParameter(ternary.FalseExpression));
        }

        private LanguageExpression ConvertGetSecretFunctionCall(InstanceFunctionCallSyntax instanceFunctionCallSyntax)
        {
            var (baseSyntax, _) = SyntaxHelper.UnwrapArrayAccessSyntax(instanceFunctionCallSyntax.BaseExpression);

            if (context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is not { } resource ||
                !StringComparer.OrdinalIgnoreCase.Equals(resource.TypeReference.FormatType(), AzResourceTypeProvider.ResourceTypeKeyVault))
            {
                throw new InvalidOperationException("Cannot emit parameter's KeyVault secret reference.");
            }

            var keyVaultId = instanceFunctionCallSyntax.BaseExpression switch
            {
                ArrayAccessSyntax arrayAccessSyntax when resource is DeclaredResourceMetadata declared =>
                    CreateConverterForIndexReplacement(declared.NameSyntax, arrayAccessSyntax.IndexExpression, instanceFunctionCallSyntax)
                    .GetFullyQualifiedResourceId(resource),
                _ => GetFullyQualifiedResourceId(resource)
            };

            if (instanceFunctionCallSyntax.Arguments.Count() > 1)
            {
                return GetCreateObjectExpression(
                    new JTokenExpression("reference"), GetCreateObjectExpression(
                        new JTokenExpression("keyVault"), GetCreateObjectExpression(new JTokenExpression("id"), keyVaultId),
                        new JTokenExpression("secretName"), ConvertExpression(instanceFunctionCallSyntax.GetArgumentByPosition(0).Expression),
                        new JTokenExpression("secretVersion"), ConvertExpression(instanceFunctionCallSyntax.GetArgumentByPosition(1).Expression)
                    )
                );
            }
            return GetCreateObjectExpression(
                new JTokenExpression("reference"), GetCreateObjectExpression(
                    new JTokenExpression("keyVault"), GetCreateObjectExpression(new JTokenExpression("id"), keyVaultId),
                    new JTokenExpression("secretName"), ConvertExpression(instanceFunctionCallSyntax.GetArgumentByPosition(0).Expression)
                    )
            );
        }

        public LanguageExpression ConvertExpression(SyntaxBase syntax)
        {
            var expression = expressionBuilder.Convert(syntax);
            expression = ExpressionLoweringVisitor.Lower(expression);

            return ConvertExpression(expression);
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        public LanguageExpression ConvertExpression(Expression expression)
        {
            SyntaxBase syntax;
            switch (expression)
            {
                case BooleanLiteralExpression @bool:
                    return CreateFunction(@bool.Value ? "true" : "false");

                case IntegerLiteralExpression @int:
                    return @int.Value switch {
                        // Bicep permits long values, but ARM's parser only permits int jtoken expressions.
                        // We can work around this by using the `json()` function to represent non-integer numerics.
                        > int.MaxValue or < int.MinValue => CreateFunction("json", new JTokenExpression(@int.Value.ToString(CultureInfo.InvariantCulture))),
                        _ => new JTokenExpression((int)@int.Value),
                    };

                case StringLiteralExpression @string:
                    return new JTokenExpression(@string.Value);

                case NullLiteralExpression _:
                    return CreateFunction("null");

                case InterpolatedStringExpression @string:
                    return ConvertString(@string);

                case ObjectExpression @object:
                    return ConvertObject(@object);

                case ArrayExpression array:
                    return ConvertArray(array);

                case SyntaxExpression syntaxExpression:
                    syntax = syntaxExpression.Syntax;
                    break;

                case UnaryExpression unary:
                    return ConvertUnary(unary);

                case BinaryExpression binary:
                    return ConvertBinary(binary);

                case TernaryExpression ternary:
                    return CreateFunction(
                        "if",
                        ConvertExpression(ternary.Condition),
                        ConvertExpression(ternary.True),
                        ConvertExpression(ternary.False));

                case FunctionCallExpression function:
                    return CreateFunction(
                        function.Name,
                        function.Parameters.Select(p => ConvertExpression(p)));

                case ArrayAccessExpression exp:
                    return AppendProperties(
                        ToFunctionExpression(ConvertExpression(exp.Base)),
                        ConvertExpression(exp.Access));

                case PropertyAccessExpression { Base: ResourceReferenceExpression resource } exp:
                    return GetConverter(resource.IndexContext).ConvertResourcePropertyAccess(resource, exp);

                case PropertyAccessExpression { Base: ModuleReferenceExpression module } exp:
                    return GetConverter(module.IndexContext).ConvertModulePropertyAccess(module, exp);

                case PropertyAccessExpression exp:
                    return AppendProperties(
                        ToFunctionExpression(ConvertExpression(exp.Base)),
                        new JTokenExpression(exp.PropertyName));

                case ModuleOutputPropertyAccessExpression exp:
                    return AppendProperties(
                        ToFunctionExpression(ConvertExpression(exp.Base)),
                        new JTokenExpression(exp.PropertyName),
                        new JTokenExpression("value"));

                case ResourceReferenceExpression exp:
                    return GetReferenceExpression(exp.Metadata, exp.IndexContext, true);

                case ModuleReferenceExpression exp:
                    return GetModuleReferenceExpression(exp.Module, exp.IndexContext);

                case VariableReferenceExpression exp:
                    if (context.VariablesToInline.Contains(exp.Variable))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpression(exp.Variable.DeclaringVariable.Value);
                    }
                    return CreateFunction("variables", new JTokenExpression(exp.Variable.Name));

                case ParametersReferenceExpression exp:
                    return CreateFunction("parameters", new JTokenExpression(exp.Parameter.Name));

                case LambdaExpression exp:
                    var variableNames = exp.Parameters.Select(x => new JTokenExpression(x));
                    var body = ConvertExpression(exp.Body);

                    return CreateFunction(
                        "lambda",
                        variableNames.Concat(body));

                case LambdaVariableReferenceExpression exp:
                    return CreateFunction("lambdaVariables", new JTokenExpression(exp.Variable.Name));

                case CopyIndexExpression exp:
                    return exp.Name is null
                        ? CreateFunction("copyIndex")
                        : CreateFunction("copyIndex", new JTokenExpression(exp.Name));

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }

            switch (syntax)
            {
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        private ExpressionConverter GetConverter(IndexReplacementContext? replacementContext)
        {
            if (replacementContext is not null)
            {
                return new(this.context, replacementContext.LocalReplacements);
            }

            return this;
        }

        public ExpressionConverter CreateConverterForIndexReplacement(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var indexContext = expressionBuilder.TryGetReplacementContext(nameSyntax, indexExpression, newContext);

            return GetConverter(indexContext);
        }

        private LanguageExpression ConvertResourcePropertyAccess(ResourceReferenceExpression reference, PropertyAccessExpression propertyAccess)
        {
            var resource = reference.Metadata;
            var indexContext = reference.IndexContext;
            var propertyName = propertyAccess.PropertyName;

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return AppendProperties(
                    GetReferenceExpression(resource, indexContext, false),
                    new JTokenExpression(propertyName));
            }

            // The cases for a parameter resource are much simpler and can be handled up front. These do not
            // support symbolic names they are somewhat different from the declared resource case since we just have an
            // ID and type.
            if (resource is ParameterResourceMetadata parameter)
            {
                switch (propertyName)
                {
                    case "id":
                        return GetFullyQualifiedResourceId(parameter);
                    case "type":
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case "apiVersion":
                        return new JTokenExpression(resource.TypeReference.ApiVersion);
                    case "name":
                        // create an expression like: `last(split(<resource id>, '/'))`
                        return new FunctionExpression(
                                "last",
                                new LanguageExpression[]
                                {
                                    new FunctionExpression(
                                        "split",
                                        new LanguageExpression[]
                                        {
                                            GetFullyQualifiedResourceId(parameter),
                                            new JTokenExpression("/"),
                                        },
                                        Array.Empty<LanguageExpression>())
                                },
                                Array.Empty<LanguageExpression>());
                    case "properties":
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return GetReferenceExpression(resource, indexContext, false);
                    default:
                        return AppendProperties(
                            GetReferenceExpression(resource, indexContext, true),
                            new JTokenExpression(propertyName));
                }
            }
            else if (resource is ModuleOutputResourceMetadata output)
            {
                switch (propertyName)
                {
                    case "id":
                        return GetFullyQualifiedResourceId(output);
                    case "type":
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case "apiVersion":
                        return new JTokenExpression(resource.TypeReference.ApiVersion);
                    case "name":
                        // create an expression like: `last(split(<resource id>, '/'))`
                        return new FunctionExpression(
                                "last",
                                new LanguageExpression[]
                                {
                                    new FunctionExpression(
                                        "split",
                                        new LanguageExpression[]
                                        {
                                            GetFullyQualifiedResourceId(output),
                                            new JTokenExpression("/"),
                                        },
                                        Array.Empty<LanguageExpression>())
                                },
                                Array.Empty<LanguageExpression>());
                    case "properties":
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return GetReferenceExpression(resource, indexContext, false);
                    default:
                        // For a module output we have to handle all possible cases here, because otherwise
                        // this case would be handled like any old property access rather than access to a resource's property.
                        return AppendProperties(GetReferenceExpression(resource, indexContext, true), new JTokenExpression(propertyName));
                }
            }
            else if (resource is DeclaredResourceMetadata declaredResource)
            {
                switch ((propertyName, context.Settings.EnableSymbolicNames))
                {
                    case ("id", true):
                    case ("name", true):
                    case ("type", true):
                    case ("apiVersion", true):
                        var symbolExpression = GenerateSymbolicReference(declaredResource, indexContext);

                        return AppendProperties(
                            CreateFunction("resourceInfo", symbolExpression),
                            new JTokenExpression(propertyName));
                    case ("id", false):
                        // the ID is dependent on the name expression which could involve locals in case of a resource collection
                        return GetFullyQualifiedResourceId(resource);
                    case ("name", false):
                        // the name is dependent on the name expression which could involve locals in case of a resource collection

                        // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                        // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                        return ConvertExpression(declaredResource.NameSyntax);
                    case ("type", false):
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case ("apiVersion", false):
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                        return new JTokenExpression(apiVersion);
                    case ("properties", _):
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return GetReferenceExpression(resource, indexContext, false);
                    default:
                        return AppendProperties(
                            GetReferenceExpression(resource, indexContext, true),
                            new JTokenExpression(propertyName));
                }
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource.GetType()}");
            }
        }

        private LanguageExpression ConvertModulePropertyAccess(ModuleReferenceExpression module, PropertyAccessExpression propertyAccess)
        {
            switch (propertyAccess.PropertyName)
            {
                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection
                    return GetModuleNameExpression(module.Module);
                case "outputs":
                    return AppendProperties(
                        GetModuleReferenceExpression(module.Module, module.IndexContext),
                        new JTokenExpression("outputs"));
            }

            throw new InvalidOperationException($"Unsupported module property: {propertyAccess.PropertyName}");
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

                var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(rewritten, newContext.Symbol.NameIdentifier);
                var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

                switch (inaccessibleLocalLoops.Count)
                {
                    case 0:
                        /*
                         * Hardcoded index expression resulted in no more local vars to replace.
                         * We can just bail out with the result.
                         */
                        return rewritten;

                    case 1 when ancestor.IndexExpression is not null:
                        if (LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.context.SemanticModel, rewritten).SingleOrDefault(s => s.LocalKind == LocalKind.ForExpressionItemVariable) is { } loopItemSymbol)
                        {
                            // rewrite the straggler from previous iteration
                            // TODO: Nested loops will require DFA on the ForSyntax.Expression
                            rewritten = SymbolReplacer.Replace(this.context.SemanticModel, new Dictionary<Symbol, SyntaxBase> { [loopItemSymbol] = SyntaxFactory.CreateArrayAccess(GetEnclosingForExpression(loopItemSymbol).Expression, ancestor.IndexExpression) }, rewritten);
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

        public LanguageExpression GetFullyQualifiedResourceName(DeclaredResourceMetadata resource)
        {
            var nameValueSyntax = resource.NameSyntax;

            // For a nested resource we need to compute the name
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
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

            var nameSegments = GetResourceNameSegments(resource);
            // {0}/{1}/{2}....
            var formatString = string.Join("/", nameSegments.Select((_, i) => $"{{{i}}}"));

            return CreateFunction("format", new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            SyntaxBase nameValueSyntax = GetModuleNameSyntax(moduleSymbol);
            return ConvertExpression(nameValueSyntax);
        }

        public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
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
            if (resource is ParameterResourceMetadata parameter)
            {
                return new FunctionExpression(
                    "parameters",
                    new LanguageExpression[] { new JTokenExpression(parameter.Symbol.Name), },
                    new LanguageExpression[] { });
            }
            else if (resource is ModuleOutputResourceMetadata output)
            {
                return AppendProperties(
                    GetModuleReferenceExpression(output.Module, null),
                    new JTokenExpression("outputs"),
                    new JTokenExpression(output.OutputName),
                    new JTokenExpression("value"));
            }
            else if (resource is DeclaredResourceMetadata declared)
            {
                var nameSegments = GetResourceNameSegments(declared);
                return ScopeHelper.FormatFullyQualifiedResourceId(
                    context,
                    this,
                    context.ResourceScopeData[declared],
                    resource.TypeReference.FormatType(),
                    nameSegments);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }
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

        public FunctionExpression GetModuleReferenceExpression(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            if (context.Settings.EnableSymbolicNames)
            {
                return CreateFunction(
                    "reference",
                    GenerateSymbolicReference(moduleSymbol, indexContext));
            }

            return CreateFunction(
                "reference",
                GetConverter(indexContext).GetFullyQualifiedResourceId(moduleSymbol),
                new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion));
        }

        public FunctionExpression GetReferenceExpression(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
            var referenceExpression = resource switch
            {
                ParameterResourceMetadata parameter => new FunctionExpression(
                    "parameters",
                    new LanguageExpression[] { new JTokenExpression(parameter.Symbol.Name), },
                    Array.Empty<LanguageExpression>()),

                ModuleOutputResourceMetadata output => AppendProperties(
                    GetModuleReferenceExpression(output.Module, null),
                    new JTokenExpression("outputs"),
                    new JTokenExpression(output.OutputName),
                    new JTokenExpression("value")),

                DeclaredResourceMetadata declared when context.Settings.EnableSymbolicNames =>
                    GenerateSymbolicReference(declared, indexContext),
                DeclaredResourceMetadata => GetFullyQualifiedResourceId(resource),

                _ => throw new InvalidOperationException($"Unexpected resource metadata type: {resource.GetType()}"),
            };

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return CreateFunction(
                    "reference",
                    referenceExpression);
            }

            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");

                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(apiVersion),
                    new JTokenExpression("full"));
            }

            if (!context.Settings.EnableSymbolicNames)
            {
                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");

                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(apiVersion));
            }

            return CreateFunction(
                "reference",
                referenceExpression);
        }

        private SyntaxBase GetEnclosingDeclaringSyntax(LocalVariableSymbol localVariable)
        {
            // we're following the symbol hierarchy rather than syntax hierarchy because
            // this guarantees a single hop in all cases
            var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
            if (symbolParent is not LocalScope localScope)
            {
                throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
            }

            return localScope.DeclaringSyntax;
        }

        private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
        {
            var declaringSyntax = GetEnclosingDeclaringSyntax(localVariable);

            if (declaringSyntax is ForSyntax @for)
            {
                return @for;
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{declaringSyntax?.GetType().Name}'.");
        }

        private LanguageExpression ConvertString(InterpolatedStringExpression expression)
        {
            var formatArgs = new LanguageExpression[expression.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(expression.SegmentValues);
            formatArgs[0] = new JTokenExpression(formatString);

            for (var i = 0; i < expression.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpression(expression.Expressions[i]);
            }

            return CreateFunction("format", formatArgs);
        }

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

        private FunctionExpression ConvertArray(ArrayExpression expression)
        {
            // we are using the createArray() function as a proxy for an array literal
            return CreateFunction(
                "createArray",
                expression.Items.Select(ConvertExpression));
        }

        private FunctionExpression ConvertObject(ObjectExpression expression)
        {
            // need keys and values in one array of parameters
            var parameters = new LanguageExpression[expression.Properties.Count() * 2];

            int index = 0;
            foreach (var property in expression.Properties)
            {
                parameters[index] = property.Key switch
                {
                    SyntaxExpression { Syntax: IdentifierSyntax identifier } => new JTokenExpression(identifier.IdentifierName),
                    StringLiteralExpression @string => new JTokenExpression(@string.Value),
                    InterpolatedStringExpression @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{property.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpression(property.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return GetCreateObjectExpression(parameters);
        }

        private static FunctionExpression GetCreateObjectExpression(params LanguageExpression[] parameters)
            => CreateFunction("createObject", parameters);

        private LanguageExpression ConvertBinary(BinaryExpression binary)
        {
            var operand1 = ConvertExpression(binary.Left);
            var operand2 = ConvertExpression(binary.Right);

            return binary.Operator switch
            {
                BinaryOperator.LogicalOr => CreateFunction("or", operand1, operand2),
                BinaryOperator.LogicalAnd => CreateFunction("and", operand1, operand2),
                BinaryOperator.Equals => CreateFunction("equals", operand1, operand2),
                BinaryOperator.NotEquals => CreateFunction("not",
                    CreateFunction("equals", operand1, operand2)),
                BinaryOperator.EqualsInsensitive => CreateFunction("equals",
                    CreateFunction("toLower", operand1),
                    CreateFunction("toLower", operand2)),
                BinaryOperator.NotEqualsInsensitive => CreateFunction("not",
                    CreateFunction("equals",
                        CreateFunction("toLower", operand1),
                        CreateFunction("toLower", operand2))),
                BinaryOperator.LessThan => CreateFunction("less", operand1, operand2),
                BinaryOperator.LessThanOrEqual => CreateFunction("lessOrEquals", operand1, operand2),
                BinaryOperator.GreaterThan => CreateFunction("greater", operand1, operand2),
                BinaryOperator.GreaterThanOrEqual => CreateFunction("greaterOrEquals", operand1, operand2),
                BinaryOperator.Add => CreateFunction("add", operand1, operand2),
                BinaryOperator.Subtract => CreateFunction("sub", operand1, operand2),
                BinaryOperator.Multiply => CreateFunction("mul", operand1, operand2),
                BinaryOperator.Divide => CreateFunction("div", operand1, operand2),
                BinaryOperator.Modulo => CreateFunction("mod", operand1, operand2),
                BinaryOperator.Coalesce => CreateFunction("coalesce", operand1, operand2),
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{binary.Operator}'."),
            };
        }

        private LanguageExpression ConvertUnary(UnaryExpression unary)
        {
            var operand = ConvertExpression(unary.Expression);
            return unary.Operator switch
            {
                UnaryOperator.Not => CreateFunction("not", operand),
                UnaryOperator.Minus => CreateFunction("sub", new JTokenExpression(0), operand),
                _ => throw new NotImplementedException($"Cannot emit unexpected unary operator '{unary.Operator}."),
            };
        }

        public string GetSymbolicName(DeclaredResourceMetadata resource)
        {
            var nestedHierarchy = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource)
                .Reverse()
                .TakeWhile(x => x.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested)
                .Select(x => x.Resource)
                .Reverse()
                .Concat(resource);

            return string.Join("::", nestedHierarchy.Select(x => x.Symbol.Name));
        }

        private LanguageExpression GenerateSymbolicReference(string symbolName, IndexReplacementContext? indexContext)
        {
            if (indexContext is null)
            {
                return new JTokenExpression(symbolName);
            }

            return CreateFunction(
                "format",
                new JTokenExpression($"{symbolName}[{{0}}]"),
                ConvertExpression(indexContext.Index));
        }

        private LanguageExpression GenerateSymbolicReference(string symbolName, SyntaxBase? indexExpression)
        {
            if (indexExpression is null)
            {
                return new JTokenExpression(symbolName);
            }

            return CreateFunction(
                "format",
                new JTokenExpression($"{symbolName}[{{0}}]"),
                ConvertExpression(indexExpression));
        }

        public LanguageExpression GenerateSymbolicReference(DeclaredResourceMetadata resource, IndexReplacementContext? indexContext)
            => GenerateSymbolicReference(GetSymbolicName(resource), indexContext);

        public LanguageExpression GenerateSymbolicReference(ModuleSymbol module, IndexReplacementContext? indexContext)
            => GenerateSymbolicReference(module.Name, indexContext);

        public LanguageExpression GenerateSymbolicReference(DeclaredResourceMetadata resource, SyntaxBase? indexExpression)
            => GenerateSymbolicReference(GetSymbolicName(resource), indexExpression);

        public LanguageExpression GenerateSymbolicReference(ModuleSymbol module, SyntaxBase? indexExpression)
            => GenerateSymbolicReference(module.Name, indexExpression);

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
