// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
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
                    return integerSyntax.Value > int.MaxValue || integerSyntax.Value < int.MinValue ? CreateFunction("json", new JTokenExpression(integerSyntax.Value.ToInvariantString())) : new JTokenExpression((int)integerSyntax.Value);

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

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private LanguageExpression ConvertFunction(FunctionCallSyntaxBase functionCall)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
            if (symbol is FunctionSymbol functionSymbol &&
                context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is FunctionOverload functionOverload &&
                functionOverload.Evaluator is not null)
            {
                return ConvertExpression(functionOverload.Evaluator(functionCall, symbol, context.SemanticModel.GetTypeInfo(functionCall)));
            }

            switch (functionCall)
            {
                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpression(a.Expression)));

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    var (baseSymbol, indexExpression) = instanceFunctionCall.BaseExpression switch
                    {
                        ArrayAccessSyntax arrayAccessSyntax => (context.SemanticModel.GetSymbolInfo(arrayAccessSyntax.BaseExpression), arrayAccessSyntax.IndexExpression),
                        _ => (context.SemanticModel.GetSymbolInfo(instanceFunctionCall.BaseExpression), null),
                    };

                    switch (baseSymbol)
                    {
                        case INamespaceSymbol namespaceSymbol:
                            Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                            return ConvertFunction(
                                instanceFunctionCall.Name.IdentifierName,
                                instanceFunctionCall.Arguments.Select(a => ConvertExpression(a.Expression)));
                        case ResourceSymbol resourceSymbol when context.SemanticModel.ResourceMetadata.TryLookup(resourceSymbol.DeclaringSyntax) is { } resource:
                            if (instanceFunctionCall.Name.IdentifierName.StartsWithOrdinalInsensitively("list"))
                            {
                                var converter = indexExpression is not null ?
                                    CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, instanceFunctionCall) :
                                    this;

                                // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                                var convertedArgs = instanceFunctionCall.Arguments.SelectArray(a => ConvertExpression(a.Expression));
                                var resourceIdExpression = converter.GetFullyQualifiedResourceId(resource);
                                var apiVersionExpression = new JTokenExpression(resource.TypeReference.ApiVersion);

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

                case 1 when indexExpression != null:
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
                if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is { } resource &&
                    resource.Symbol.IsCollection)
                {
                    var resourceConverter = this.CreateConverterForIndexReplacement(resource.NameSyntax, arrayAccess.IndexExpression, arrayAccess);

                    // TODO: Can this return a language expression?
                    return resourceConverter.ToFunctionExpression(arrayAccess.BaseExpression);
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

        private LanguageExpression? ConvertResourcePropertyAccess(ResourceMetadata resource, SyntaxBase? indexExpression, string propertyName)
        {
            // special cases for certain resource property access. if we recurse normally, we'll end up
            // generating statements like reference(resourceId(...)).id which are not accepted by ARM

            switch ((propertyName, context.Settings.EnableSymbolicNames))
            {
                case ("id", true):
                case ("name", true):
                case ("type", true):
                case ("apiVersion", true):
                    var symbolExpression = GenerateSymbolicReference(resource.Symbol.Name, indexExpression);

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
                    return ConvertExpression(resource.NameSyntax);
                case ("type", false):
                    return new JTokenExpression(resource.TypeReference.FullyQualifiedType);
                case ("apiVersion", false):
                    return new JTokenExpression(resource.TypeReference.ApiVersion);
                case ("properties", _):
                    // use the reference() overload without "full" to generate a shorter expression
                    // this is dependent on the name expression which could involve locals in case of a resource collection
                    return GetReferenceExpression(resource, indexExpression, false);
                default:
                    return null;
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
            if ((propertyAccess.BaseExpression is VariableAccessSyntax || propertyAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is { } resource &&
                CreateConverterForIndexReplacement(resource.NameSyntax, null, propertyAccess)
                    .ConvertResourcePropertyAccess(resource, null, propertyAccess.PropertyName.IdentifierName) is { } convertedSingle)
            {
                // we are doing property access on a single resource
                // and we are dealing with special case properties
                return convertedSingle;
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                (propArrayAccess.BaseExpression is VariableAccessSyntax || propArrayAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is { } resourceCollection &&
                CreateConverterForIndexReplacement(resourceCollection.NameSyntax, propArrayAccess.IndexExpression, propertyAccess)
                    .ConvertResourcePropertyAccess(resourceCollection, propArrayAccess.IndexExpression, propertyAccess.PropertyName.IdentifierName) is { } convertedCollection)
            {

                // we are doing property access on an array access of a resource collection
                // and we are dealing with special case properties
                return convertedCollection;
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

        public IEnumerable<LanguageExpression> GetResourceNameSegments(ResourceMetadata resource)
        {
            var typeReference = resource.TypeReference;
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameSyntax = resource.NameSyntax;
            var nameExpression = ConvertExpression(nameSyntax);

            if (ancestors.Length > 0)
            {
                var firstAncestorNameLength = typeReference.Types.Length - ancestors.Length;

                var resourceName = ConvertExpression(resource.NameSyntax);

                var parentNames = ancestors.SelectMany((x, i) =>
                {
                    var nameSyntax = x.Resource.NameSyntax;
                    var nameExpression = CreateConverterForIndexReplacement(nameSyntax, x.IndexExpression, x.Resource.Symbol.NameSyntax)
                        .ConvertExpression(nameSyntax);

                    if (i == 0 && firstAncestorNameLength > 1)
                    {
                        return Enumerable.Range(0, firstAncestorNameLength).Select(
                            (_, i) => AppendProperties(
                                CreateFunction("split", nameExpression, new JTokenExpression("/")),
                                new JTokenExpression(i)));
                    }

                    return nameExpression.AsEnumerable();
                });

                return parentNames.Concat(resourceName.AsEnumerable());
            }

            if (typeReference.Types.Length == 1)
            {
                return nameExpression.AsEnumerable();
            }

            return typeReference.Types.Select(
                (type, i) => AppendProperties(
                    CreateFunction("split", nameExpression, new JTokenExpression("/")),
                    new JTokenExpression(i)));
        }

        public LanguageExpression GetFullyQualifiedResourceName(ResourceMetadata resource)
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
            return moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        public LanguageExpression GetUnqualifiedResourceId(ResourceMetadata resource)
        {
            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
                resource.TypeReference.FullyQualifiedType,
                GetResourceNameSegments(resource));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ResourceMetadata resource)
        {
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
                resource.TypeReference.FullyQualifiedType,
                GetResourceNameSegments(resource));
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

            return AppendProperties(
                CreateFunction(
                    "reference",
                    GetFullyQualifiedResourceId(moduleSymbol),
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                new JTokenExpression("outputs"));
        }

        public FunctionExpression GetReferenceExpression(ResourceMetadata resource, SyntaxBase? indexExpression, bool full)
        {
            var referenceExpression = context.Settings.EnableSymbolicNames ?
                GenerateSymbolicReference(resource.Symbol.Name, indexExpression) :
                GetFullyQualifiedResourceId(resource);

            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(resource.TypeReference.ApiVersion),
                    new JTokenExpression("full"));
            }

            if (resource.IsExistingResource && !context.Settings.EnableSymbolicNames)
            {
                // we must include an API version for an existing resource, because it cannot be inferred from any deployed template resource
                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(resource.TypeReference.ApiVersion));
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

            var @for = GetEnclosingForExpression(localVariableSymbol);
            return GetLoopVariableExpression(localVariableSymbol, @for, CreateCopyIndexFunction(@for));
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

        private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
        {
            // we're following the symbol hierarchy rather than syntax hierarchy because
            // this guarantees a single hop in all cases
            var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
            if (symbolParent is not LocalScope localScope)
            {
                throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
            }

            if (localScope.DeclaringSyntax is ForSyntax @for)
            {
                return @for;
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{localScope.DeclaringSyntax?.GetType().Name}'.");
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
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
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

        private static LanguageExpression ConvertFunction(string functionName, IEnumerable<LanguageExpression> arguments)
        {
            if (ShouldReplaceUnsupportedFunction(functionName, arguments, out var replacementExpression))
            {
                return replacementExpression;
            }

            return CreateFunction(functionName, arguments);
        }

        private static bool ShouldReplaceUnsupportedFunction(string functionName, IEnumerable<LanguageExpression> arguments, [NotNullWhen(true)] out LanguageExpression? replacementExpression)
        {
            switch (functionName)
            {
                // These functions have not yet been implemented in ARM. For now, we will just return an empty object if they are accessed directly.
                case "tenant":
                case "managementGroup":
                case "subscription" when arguments.Any():
                case "resourceGroup" when arguments.Any():
                    replacementExpression = GetCreateObjectExpression();
                    return true;
            }

            replacementExpression = null;
            return false;
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
            LanguageExpression convertedOperand = ConvertExpression(syntax.Expression);

            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return CreateFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is JTokenExpression literal && literal.Value.Type == JTokenType.Integer)
                    {
                        // invert the integer literal
                        int literalValue = literal.Value.Value<int>();
                        return new JTokenExpression(-literalValue);
                    }

                    return CreateFunction(
                        "sub",
                        new JTokenExpression(0),
                        convertedOperand);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        public LanguageExpression GenerateSymbolicReference(string symbolName, SyntaxBase? indexExpression)
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

