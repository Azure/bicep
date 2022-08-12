// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
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

        private readonly ImmutableDictionary<LocalVariableSymbol, LanguageExpression> localReplacements;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, LanguageExpression>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, LanguageExpression> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
        }

        public ExpressionConverter AppendReplacement(LocalVariableSymbol symbol, LanguageExpression replacement)
        {
            // Allow local variable symbol replacements to be overwritten, as there are scenarios where we recursively generate expressions for the same index symbol
            return new(this.context, this.localReplacements.SetItem(symbol, replacement));
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        public LanguageExpression ConvertExpression(SyntaxBase expression)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return CreateFunction(boolSyntax.Value ? "true" : "false");

                case IntegerLiteralSyntax integerSyntax:
                    return ConvertInteger(integerSyntax, false);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);

                case NullLiteralSyntax _:
                    return CreateFunction("null");

                case ObjectSyntax @object:
                    return ConvertObject(@object);

                case ArraySyntax array:
                    return ConvertArray(array);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return ConvertExpression(parenthesized.Expression);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return CreateFunction(
                        "if",
                        ConvertExpression(ternary.ConditionExpression),
                        ConvertExpression(ternary.TrueExpression),
                        ConvertExpression(ternary.FalseExpression));

                case FunctionCallSyntaxBase functionCall:
                    return ConvertFunction(functionCall);

                case ArrayAccessSyntax arrayAccess:
                    return ConvertArrayAccess(arrayAccess);

                case ResourceAccessSyntax resourceAccess:
                    return ConvertResourceAccess(resourceAccess);

                case PropertyAccessSyntax propertyAccess:
                    return ConvertPropertyAccess(propertyAccess);

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess);

                case LambdaSyntax lambda:
                    var variables = lambda.GetLocalVariables();

                    var variableNames = variables.Select(x => new JTokenExpression(x.Name.IdentifierName));
                    var body = ConvertExpression(lambda.Body);

                    return CreateFunction(
                        "lambda",
                        variableNames.Concat(body));

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private LanguageExpression ConvertFunction(FunctionCallSyntaxBase functionCall)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
            if (symbol is FunctionSymbol &&
                context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: { } } functionOverload)
            {
                return ConvertExpression(functionOverload.Evaluator(functionCall,
                    symbol,
                    context.SemanticModel.GetTypeInfo(functionCall),
                    context.FunctionVariables.GetValueOrDefault(functionCall),
                    context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall)));
            }

            switch (functionCall)
            {
                case FunctionCallSyntax function:
                    return CreateFunction(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpression(a.Expression)));

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    var (baseExpression, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(instanceFunctionCall.BaseExpression);
                    var baseSymbol = context.SemanticModel.GetSymbolInfo(baseExpression);

                    switch (baseSymbol)
                    {
                        case INamespaceSymbol namespaceSymbol:
                            Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                            return CreateFunction(
                                instanceFunctionCall.Name.IdentifierName,
                                instanceFunctionCall.Arguments.Select(a => ConvertExpression(a.Expression)));
                        case DeclaredSymbol declaredSymbol when context.SemanticModel.ResourceMetadata.TryLookup(declaredSymbol.DeclaringSyntax) is DeclaredResourceMetadata resource:
                            if (instanceFunctionCall.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                            {
                                var converter = indexExpression is not null ?
                                    CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, instanceFunctionCall) :
                                    this;

                                // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                                var convertedArgs = instanceFunctionCall.Arguments.SelectArray(a => ConvertExpression(a.Expression));
                                var resourceIdExpression = converter.GetFullyQualifiedResourceId(resource);

                                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                                var apiVersionExpression = new JTokenExpression(apiVersion);

                                var listArgs = convertedArgs.Length switch
                                {
                                    0 => new LanguageExpression[] { resourceIdExpression, apiVersionExpression, },
                                    _ => new LanguageExpression[] { resourceIdExpression, }.Concat(convertedArgs),
                                };

                                return CreateFunction(instanceFunctionCall.Name.IdentifierName, listArgs);
                            }

                            break;
                    }
                    throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
            }
        }

        public ExpressionConverter CreateConverterForIndexReplacement(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
            var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

            switch (inaccessibleLocalLoops.Count)
            {
                case 0:
                    // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                    // regardless if there is an index expression or not, we don't need to append replacements
                    return this;

                case 1 when indexExpression is not null:
                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();
                    var current = this;
                    foreach (var local in inaccessibleLocals)
                    {
                        var replacementValue = GetLoopVariableExpression(local, @for, this.ConvertExpression(indexExpression));
                        current = current.AppendReplacement(local, replacementValue);
                    }

                    return current;

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
            }
        }

        private LanguageExpression ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
        {
            // if there is an array access on a resource/module reference, we have to generate differently
            // when constructing the reference() function call, the resource name expression needs to have its local
            // variable replaced with <loop array expression>[this array access' index expression]
            if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
            {
                if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is DeclaredResourceMetadata resource &&
                    resource.Symbol.IsCollection)
                {
                    var movedSyntax = context.Settings.EnableSymbolicNames ? resource.Symbol.NameSyntax : resource.NameSyntax;

                    return this.CreateConverterForIndexReplacement(movedSyntax, arrayAccess.IndexExpression, arrayAccess)
                        .GetReferenceExpression(resource, arrayAccess.IndexExpression, true);
                }

                switch (this.context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression))
                {
                    case ModuleSymbol { IsCollection: true } moduleSymbol:
                        var moduleConverter = this.CreateConverterForIndexReplacement(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);

                        // TODO: Can this return a language expression?
                        return moduleConverter.ToFunctionExpression(arrayAccess.BaseExpression);
                }
            }

            return AppendProperties(
                ToFunctionExpression(arrayAccess.BaseExpression),
                ConvertExpression(arrayAccess.IndexExpression));
        }

        private LanguageExpression ConvertResourcePropertyAccess(ResourceMetadata resource, SyntaxBase? indexExpression, string propertyName)
        {
            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return AppendProperties(
                    GetReferenceExpression(resource, indexExpression, false),
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
                        return GetReferenceExpression(resource, indexExpression, false);
                    default:
                        return AppendProperties(
                            GetReferenceExpression(resource, indexExpression, true),
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
                        return GetReferenceExpression(resource, indexExpression, false);
                    default:
                        // For a module output we have to handle all possible cases here, because otherwise
                        // this case would be handled like any old property access rather than access to a resource's property.
                        return AppendProperties(GetReferenceExpression(resource, indexExpression, true), new JTokenExpression(propertyName));
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
                        var symbolExpression = GenerateSymbolicReference(declaredResource, indexExpression);

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
                        return GetReferenceExpression(resource, indexExpression, false);
                    default:
                        return AppendProperties(
                            GetReferenceExpression(resource, indexExpression, true),
                            new JTokenExpression(propertyName));
                }
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource.GetType()}");
            }
        }

        private LanguageExpression? ConvertModulePropertyAccess(ModuleSymbol moduleSymbol, string propertyName)
        {
            switch (propertyName)
            {
                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection
                    return GetModuleNameExpression(moduleSymbol);
            }

            return null;
        }

        private LanguageExpression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
            {
                var movedSyntax = context.Settings.EnableSymbolicNames ? resource.Symbol.NameSyntax : resource.NameSyntax;

                // we are doing property access on a single resource
                return CreateConverterForIndexReplacement(movedSyntax, null, propertyAccess)
                    .ConvertResourcePropertyAccess(resource, null, propertyAccess.PropertyName.IdentifierName);
            }

            if ((propertyAccess.BaseExpression is VariableAccessSyntax || propertyAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ParameterResourceMetadata parameter &&
                    this.ConvertResourcePropertyAccess(parameter, null, propertyAccess.PropertyName.IdentifierName) is { } convertedSingleParameter)
            {
                // we are doing property access on a single resource
                // and we are dealing with special case properties
                return convertedSingleParameter;
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
            {
                var movedSyntax = context.Settings.EnableSymbolicNames ? resourceCollection.Symbol.NameSyntax : resourceCollection.NameSyntax;

                // we are doing property access on an array access of a resource collection
                return CreateConverterForIndexReplacement(movedSyntax, propArrayAccess.IndexExpression, propertyAccess)
                    .ConvertResourcePropertyAccess(resourceCollection, propArrayAccess.IndexExpression, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
                !moduleOutput.Module.IsCollection &&
                this.ConvertResourcePropertyAccess(moduleOutput, null, propertyAccess.PropertyName.IdentifierName) is { } convertedSingleModuleOutput)
            {
                // we are doing property access on an output of a non-collection module.
                // and we are dealing with special case properties
                return convertedSingleModuleOutput;
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
                moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
                moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
                moduleCollectionOutputMetadata.Module.IsCollection &&
                CreateConverterForIndexReplacement(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess)
                    .ConvertResourcePropertyAccess(moduleCollectionOutputMetadata, null, propertyAccess.PropertyName.IdentifierName) is { } convertedCollectionModuleOutput)
            {
                // we are doing property access on an output of an array of modules.
                // and we are dealing with special case properties
                return convertedCollectionModuleOutput;
            }

            if (propertyAccess.BaseExpression is VariableAccessSyntax modulePropVariableAccess &&
                context.SemanticModel.GetSymbolInfo(modulePropVariableAccess) is ModuleSymbol moduleSymbol &&
                CreateConverterForIndexReplacement(GetModuleNameSyntax(moduleSymbol), null, propertyAccess)
                    .ConvertModulePropertyAccess(moduleSymbol, propertyAccess.PropertyName.IdentifierName) is { } moduleConvertedSingle)
            {
                // we are doing property access on a single module
                // and we are dealing with special case properties
                return moduleConvertedSingle;
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax modulePropArrayAccess &&
                modulePropArrayAccess.BaseExpression is VariableAccessSyntax moduleArrayVariableAccess &&
                context.SemanticModel.GetSymbolInfo(moduleArrayVariableAccess) is ModuleSymbol moduleCollectionSymbol &&
                CreateConverterForIndexReplacement(GetModuleNameSyntax(moduleCollectionSymbol), modulePropArrayAccess.IndexExpression, propertyAccess)
                    .ConvertModulePropertyAccess(moduleCollectionSymbol, propertyAccess.PropertyName.IdentifierName) is { } moduleConvertedCollection)
            {

                // we are doing property access on an array access of a module collection
                // and we are dealing with special case properties
                return moduleConvertedCollection;
            }

            // is this a (<child>.outputs).<prop> propertyAccess?
            if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess && childPropertyAccess.PropertyName.IdentifierName == LanguageConstants.ModuleOutputsPropertyName)
            {
                switch (childPropertyAccess.BaseExpression)
                {
                    // is <child> a variable which points to a variable that requires in-lining?
                    case VariableAccessSyntax grandChildVariableAccess
                        when context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is VariableSymbol variableSymbol &&
                            context.VariablesToInline.Contains(variableSymbol):
                        {
                            //execute variable in-lining
                            if (ConvertVariableAccess(grandChildVariableAccess) is FunctionExpression moduleReferenceExpression)
                            {
                                // we assume that this will generate a proper reference function to a deployment resource.
                                // If not then the deployment will fail as the template will be malformed but that should have been caught before

                                return AppendProperties(moduleReferenceExpression,
                                    new JTokenExpression(propertyAccess.PropertyName.IdentifierName),
                                    new JTokenExpression("value"));
                            }
                            break;
                        }

                    // is <child> a variable which points to a non-collection module symbol?
                    case VariableAccessSyntax grandChildVariableAccess
                        when context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is ModuleSymbol { IsCollection: false } outputsModuleSymbol:
                        {
                            return AppendProperties(
                                this.GetModuleOutputsReferenceExpression(outputsModuleSymbol, null),
                                new JTokenExpression(propertyAccess.PropertyName.IdentifierName),
                                new JTokenExpression("value"));
                        }

                    // is <child> an array access operating on a module collection
                    case ArrayAccessSyntax { BaseExpression: VariableAccessSyntax grandGrandChildVariableAccess } grandChildArrayAccess
                        when context.SemanticModel.GetSymbolInfo(grandGrandChildVariableAccess) is ModuleSymbol { IsCollection: true } outputsModuleCollectionSymbol:
                        {
                            var updatedConverter = this.CreateConverterForIndexReplacement(GetModuleNameSyntax(outputsModuleCollectionSymbol), grandChildArrayAccess.IndexExpression, propertyAccess);
                            return AppendProperties(
                                updatedConverter.GetModuleOutputsReferenceExpression(outputsModuleCollectionSymbol, grandChildArrayAccess.IndexExpression),
                                new JTokenExpression(propertyAccess.PropertyName.IdentifierName),
                                new JTokenExpression("value"));
                        }
                }
            }

            return AppendProperties(
                ToFunctionExpression(propertyAccess.BaseExpression),
                new JTokenExpression(propertyAccess.PropertyName.IdentifierName));
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
                    GetModuleOutputsReferenceExpression(output.Module, null),
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

        public FunctionExpression GetModuleOutputsReferenceExpression(ModuleSymbol moduleSymbol, SyntaxBase? indexExpression)
        {
            if (context.Settings.EnableSymbolicNames)
            {
                return AppendProperties(
                    CreateFunction(
                        "reference",
                        GenerateSymbolicReference(moduleSymbol.Name, indexExpression)),
                    new JTokenExpression("outputs"));
            }

            if (moduleSymbol.DeclaringModule.HasCondition())
            {
                return AppendProperties(
                    CreateFunction(
                        "reference",
                        GetFullyQualifiedResourceId(moduleSymbol),
                        new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                    new JTokenExpression("outputs"));
            }

            return AppendProperties(
                CreateFunction(
                    "reference",
                    GetFullyQualifiedResourceId(moduleSymbol)),
                new JTokenExpression("outputs"));
        }

        public FunctionExpression GetReferenceExpression(ResourceMetadata resource, SyntaxBase? indexExpression, bool full)
        {
            var referenceExpression = resource switch
            {
                ParameterResourceMetadata parameter => new FunctionExpression(
                    "parameters",
                    new LanguageExpression[] { new JTokenExpression(parameter.Symbol.Name), },
                    Array.Empty<LanguageExpression>()),

                ModuleOutputResourceMetadata output => AppendProperties(
                    GetModuleOutputsReferenceExpression(output.Module, null),
                    new JTokenExpression(output.OutputName),
                    new JTokenExpression("value")),

                DeclaredResourceMetadata declared when context.Settings.EnableSymbolicNames =>
                    GenerateSymbolicReference(declared, indexExpression),
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

            var shouldIncludeApiVersion =
                !context.Settings.EnableSymbolicNames &&
                (resource.IsExistingResource ||
                (resource is DeclaredResourceMetadata { Symbol.DeclaringResource: var declaringResource } && declaringResource.HasCondition()));

            if (shouldIncludeApiVersion)
            {
                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");

                // we must include an API version for an existing resource, because it cannot be inferred from any deployed template resource
                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(apiVersion));
            }

            return CreateFunction(
                "reference",
                referenceExpression);
        }

        private LanguageExpression GetLocalVariableExpression(LocalVariableSymbol localVariableSymbol)
        {
            if (this.localReplacements.TryGetValue(localVariableSymbol, out var replacement))
            {
                // the current context has specified an expression to be used for this local variable symbol
                // to override the regular conversion
                return replacement;
            }

            var enclosingSyntax = GetEnclosingDeclaringSyntax(localVariableSymbol);
            switch (enclosingSyntax) {
                case ForSyntax @for:
                    return GetLoopVariableExpression(localVariableSymbol, @for, CreateCopyIndexFunction(@for));
                case LambdaSyntax lambda:
                    return CreateFunction("lambdaVariables", new JTokenExpression(localVariableSymbol.Name));
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{enclosingSyntax?.GetType().Name}'.");
        }

        private LanguageExpression GetLoopVariableExpression(LocalVariableSymbol localVariableSymbol, ForSyntax @for, LanguageExpression indexExpression)
        {
            return localVariableSymbol.LocalKind switch
            {
                // this is the "item" variable of a for-expression
                // to emit this, we need to index the array expression by the copyIndex() function
                LocalKind.ForExpressionItemVariable => GetLoopItemVariableExpression(@for, indexExpression),

                // this is the "index" variable of a for-expression inside a variable block
                // to emit this, we need to return a copyIndex(...) function
                LocalKind.ForExpressionIndexVariable => indexExpression,

                _ => throw new NotImplementedException($"Unexpected local variable kind '{localVariableSymbol.LocalKind}'."),
            };
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

        private string? GetCopyIndexName(ForSyntax @for)
        {
            return this.context.SemanticModel.Binder.GetParent(@for) switch
            {
                // copyIndex without name resolves to module/resource loop index in the runtime
                ResourceDeclarationSyntax => null,
                ModuleDeclarationSyntax => null,

                // variable copy index has the name of the variable
                VariableDeclarationSyntax variable when variable.Name.IsValid => variable.Name.IdentifierName,

                // output loops are only allowed at the top level and don't have names, either
                OutputDeclarationSyntax => null,

                // the property copy index has the name of the property
                ObjectPropertySyntax property when property.TryGetKeyText() is { } key && ReferenceEquals(property.Value, @for) => key,

                _ => throw new NotImplementedException("Unexpected for-expression grandparent.")
            };
        }

        private FunctionExpression CreateCopyIndexFunction(ForSyntax @for)
        {
            var copyIndexName = GetCopyIndexName(@for);
            return copyIndexName is null
                ? CreateFunction("copyIndex")
                : CreateFunction("copyIndex", new JTokenExpression(copyIndexName));
        }

        private FunctionExpression GetLoopItemVariableExpression(ForSyntax @for, LanguageExpression indexExpression)
        {
            // loop item variable should be replaced with <array expression>[<index expression>]
            var arrayExpression = ToFunctionExpression(@for.Expression);

            return AppendProperties(arrayExpression, indexExpression);
        }

        private LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            var name = variableAccessSyntax.Name.IdentifierName;

            if (variableAccessSyntax is ExplicitVariableAccessSyntax)
            {
                //just return a call to variables.
                return CreateFunction("variables", new JTokenExpression(name));
            }

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
                case ParameterSymbol parameterSymbol when parameterSymbol.Type is ResourceType resourceType:
                    // This is a reference to a pre-existing resource where the resource ID was passed in as a
                    // string. Generate a call to reference().
                    return CreateFunction(
                        "reference",
                        CreateFunction("parameters", new JTokenExpression(name)),
                        new JTokenExpression(resourceType.TypeReference.ApiVersion),
                        new JTokenExpression("full"));

                case ParameterSymbol parameterSymbol when parameterSymbol.Type is ResourceType:
                    return CreateFunction("parameters", new JTokenExpression(name));

                case ParameterSymbol _:
                    return CreateFunction("parameters", new JTokenExpression(name));

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpression(variableSymbol.DeclaringVariable.Value);
                    }
                    return CreateFunction("variables", new JTokenExpression(name));

                case ResourceSymbol when context.SemanticModel.ResourceMetadata.TryLookup(variableAccessSyntax) is { } resource:
                    return GetReferenceExpression(resource, null, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleOutputsReferenceExpression(moduleSymbol, null);

                case LocalVariableSymbol localVariableSymbol:
                    return GetLocalVariableExpression(localVariableSymbol);

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");

            }
        }

        private LanguageExpression ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
            {
                return GetReferenceExpression(resource, null, true);
            }

            throw new NotImplementedException($"Unable to obtain resource metadata when generating a resource access expression.");
        }

        private LanguageExpression ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new JTokenExpression(literalStringValue);
            }

            var formatArgs = new LanguageExpression[syntax.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(syntax);
            formatArgs[0] = new JTokenExpression(formatString);

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpression(syntax.Expressions[i]);
            }

            return CreateFunction("format", formatArgs);
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
        public FunctionExpression ToFunctionExpression(SyntaxBase expression)
        {
            var converted = ConvertExpression(expression);
            return ToFunctionExpression(converted);
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

        private FunctionExpression ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return CreateFunction(
                "createArray",
                syntax.Items.Select(item => ConvertExpression(item.Value)));
        }

        private FunctionExpression ConvertObject(ObjectSyntax syntax)
        {
            // need keys and values in one array of parameters
            var parameters = new LanguageExpression[syntax.Properties.Count() * 2];

            int index = 0;
            foreach (var propertySyntax in syntax.Properties)
            {
                parameters[index] = propertySyntax.Key switch
                {
                    IdentifierSyntax identifier => new JTokenExpression(identifier.IdentifierName),
                    StringSyntax @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{propertySyntax.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpression(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return GetCreateObjectExpression(parameters);
        }

        private static FunctionExpression GetCreateObjectExpression(params LanguageExpression[] parameters)
            => CreateFunction("createObject", parameters);

        private LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = ConvertExpression(syntax.LeftExpression);
            LanguageExpression operand2 = ConvertExpression(syntax.RightExpression);

            return syntax.Operator switch
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
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'."),
            };
        }

        private LanguageExpression ConvertUnary(UnaryOperationSyntax syntax)
        {
            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    LanguageExpression convertedOperand = ConvertExpression(syntax.Expression);
                    return CreateFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (syntax.Expression is IntegerLiteralSyntax integerLiteral)
                    {
                        // shortcutting the integer parsing logic here because we need to return either the literal 32 bit integer or the FunctionExpression of an integer outside the 32 bit range
                        return ConvertInteger(integerLiteral, true);
                    }

                    return CreateFunction(
                        "sub",
                        new JTokenExpression(0),
                        ConvertExpression(syntax.Expression));

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        // the deployment engine can only handle 32 bit integers expressed as literal values, so for 32 bit integers, we return the literal integer value
        // for values outside that signed 32 bit integer range, we return the FunctionExpression
        private LanguageExpression ConvertInteger(IntegerLiteralSyntax integerSyntax, bool minus)
        {
            if (minus)
            {
                // integerSyntax.Value is always positive, so for the most negative signed 32 bit integer -2,147,483,648
                // we would compare its positive token (2,147,483,648) to int.MaxValue (2,147,483,647) + 1
                if (integerSyntax.Value > (ulong)int.MaxValue + 1)
                {
                    return CreateFunction("json", new JTokenExpression($"-{integerSyntax.Value.ToString(CultureInfo.InvariantCulture)}"));
                }
                else
                {
                    // the integerSyntax.Value is a valid negative 32 bit integer.
                    // because integerSyntax.Value is a ulong type, it is always positive. we need to first cast it to a long in order to negate it.
                    // after negating, cast it to a int type because that is what represents a signed 32 bit integer.
                    var longValue = -(long)integerSyntax.Value;
                    return new JTokenExpression((int)longValue);
                }
            }
            else
            {
                if (integerSyntax.Value > int.MaxValue)
                {
                    return CreateFunction("json", new JTokenExpression(integerSyntax.Value.ToString(CultureInfo.InvariantCulture)));
                }
                else
                {
                    return new JTokenExpression((int)integerSyntax.Value);
                }
            }
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

        protected static void Assert(bool predicate, string message)
        {
            if (predicate == false)
            {
                // we have a code defect - use the exception stack to debug
                throw new ArgumentException(message);
            }
        }
    }
}

