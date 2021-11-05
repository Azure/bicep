// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.CodeAnalysis;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        private readonly ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, Operation>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        private Operation ConvertExpressionOperation(SyntaxBase expression)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return new FunctionCallOperation(boolSyntax.Value ? "true" : "false", ImmutableArray<Operation>.Empty);

                case IntegerLiteralSyntax integerSyntax:
                    if (integerSyntax.Value >= int.MinValue && integerSyntax.Value <= int.MaxValue)
                    {
                        return new ConstantValueOperation((int)integerSyntax.Value);
                    }

                    return new FunctionCallOperation(
                        "json",
                        new [] { new ConstantValueOperation(integerSyntax.Value.ToInvariantString()) });

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);

                case NullLiteralSyntax _:
                    return new FunctionCallOperation("null", ImmutableArray<Operation>.Empty);

                case ObjectSyntax @object:
                    return ConvertObject(@object);

                case ArraySyntax array:
                    return ConvertArray(array);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return ConvertExpressionOperation(parenthesized.Expression);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return new FunctionCallOperation(
                        "if",
                        ConvertExpressionOperation(ternary.ConditionExpression),
                        ConvertExpressionOperation(ternary.TrueExpression),
                        ConvertExpressionOperation(ternary.FalseExpression));

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

        public LanguageExpression ConvertExpression(SyntaxBase syntax)
            => ConvertOperation(ConvertExpressionOperation(syntax));

        private Operation ConvertFunction(FunctionCallSyntaxBase functionCall)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
            if (symbol is FunctionSymbol functionSymbol &&
                context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is FunctionOverload functionOverload &&
                functionOverload.Evaluator is not null)
            {
                return ConvertExpressionOperation(functionOverload.Evaluator(functionCall, symbol, context.SemanticModel.GetTypeInfo(functionCall)));
            }

            switch (functionCall)
            {
                case FunctionCallSyntax function:
                    return new FunctionCallOperation(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpressionOperation(a.Expression)).ToImmutableArray());

                case InstanceFunctionCallSyntax method:
                    var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(method.BaseExpression);
                    var baseSymbol = context.SemanticModel.GetSymbolInfo(method.BaseExpression);

                    switch (baseSymbol)
                    {
                        case INamespaceSymbol namespaceSymbol:
                            Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                            return new FunctionCallOperation(
                                method.Name.IdentifierName,
                                method.Arguments.Select(a => ConvertExpressionOperation(a.Expression)).ToImmutableArray());
                        case {} _ when context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is { } resource:
                            if (method.Name.IdentifierName.StartsWithOrdinalInsensitively("list"))
                            {
                                // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                                var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);
                                var resourceIdOperation = new ResourceIdOperation(resource, indexContext);

                                var convertedArgs = method.Arguments.SelectArray(a => ConvertExpressionOperation(a.Expression));

                                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                                var apiVersionOperation = new ConstantValueOperation(apiVersion);

                                var listArgs = convertedArgs.Length switch
                                {
                                    0 => new Operation[] { resourceIdOperation, apiVersionOperation, },
                                    _ => new Operation[] { resourceIdOperation, }.Concat(convertedArgs),
                                };

                                return new FunctionCallOperation(
                                    method.Name.IdentifierName,
                                    listArgs.ToImmutableArray());
                            }

                            break;
                    }
                    throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
            }
        }

        public IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
            var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

            switch (inaccessibleLocalLoops.Count)
            {
                case 0:
                    // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                    // regardless if there is an index expression or not, we don't need to append replacements
                    if (indexExpression is null)
                    {
                        return null;
                    }

                    return new(this.localReplacements, ConvertExpressionOperation(indexExpression));

                case 1 when indexExpression is not null:
                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();
                    var localReplacements = this.localReplacements;
                    var converter = new ExpressionConverter(this.context, localReplacements);
                    foreach (var local in inaccessibleLocals)
                    {
                        // Allow local variable symbol replacements to be overwritten, as there are scenarios where we recursively generate expressions for the same index symbol
                        var replacementValue = GetLoopVariable(local, @for, converter.ConvertExpressionOperation(indexExpression));
                        localReplacements = localReplacements.SetItem(local, replacementValue);
                    }

                    return new(localReplacements, converter.ConvertExpressionOperation(indexExpression));

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
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
            => GetConverter(
                TryGetReplacementContext(nameSyntax, indexExpression, newContext));

        private Operation ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
        {
            // if there is an array access on a resource/module reference, we have to generate differently
            // when constructing the reference() function call, the resource name expression needs to have its local
            // variable replaced with <loop array expression>[this array access' index expression]
            if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
            {
                if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is { } resource &&
                    resource.Symbol.IsCollection)
                {
                    var indexContent = TryGetReplacementContext(resource.NameSyntax, arrayAccess.IndexExpression, arrayAccess);
                    return GetResourceReference(resource, indexContent, full: true);
                }

                if (context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
                {
                    var indexContent = TryGetReplacementContext(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);
                    return GetModuleOutputsReference(moduleSymbol, indexContent);
                }
            }

            return new ArrayAccessOperation(
                ConvertExpressionOperation(arrayAccess.BaseExpression),
                ConvertExpressionOperation(arrayAccess.IndexExpression));
        }

        public LanguageExpression ConvertOperation(Operation operation)
        {
            return operation switch {
                ConstantValueOperation op => op.Value switch {
                    string value => new JTokenExpression(value),
                    int value => new JTokenExpression(value),
                    _ => throw new NotImplementedException($"Cannot convert constant type {op.Value?.GetType()}"),
                },
                PropertyAccessOperation op => AppendProperties(
                    ToFunctionExpression(ConvertOperation(op.Base)),
                    new JTokenExpression(op.PropertyName)),
                ArrayAccessOperation op => AppendProperties(
                    ToFunctionExpression(ConvertOperation(op.Base)),
                    ConvertOperation(op.Access)),
                ResourceIdOperation op => GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Metadata),
                ResourceNameOperation op => op.FullyQualified switch {
                    true => GetConverter(op.IndexContext).GetFullyQualifiedResourceName(op.Metadata),
                    false => GetConverter(op.IndexContext).ConvertExpression(op.Metadata.NameSyntax),
                },
                ResourceTypeOperation op => new JTokenExpression(op.Metadata.TypeReference.FormatType()),
                ResourceApiVersionOperation op => op.Metadata.TypeReference.ApiVersion switch {
                    {} apiVersion => new JTokenExpression(apiVersion),
                    _ => throw new NotImplementedException(""),
                },
                ResourceInfoOperation op => CreateFunction(
                    "resourceInfo",
                    GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext)),
                ResourceReferenceOperation op => (op.Full, op.Metadata.IsExistingResource) switch {
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
                },
                SymbolicResourceReferenceOperation op => (op.Full, op.Metadata.IsAzResource) switch {
                    (true, true) => CreateFunction(
                        "reference",
                        GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext),
                        new JTokenExpression(op.Metadata.TypeReference.ApiVersion!),
                        new JTokenExpression("full")),
                    (true, _) => throw new NotImplementedException(""),
                    (false, _) => CreateFunction(
                        "reference",
                        GenerateSymbolicReference(op.Metadata.Symbol.Name, op.IndexContext)),
                },
                ModuleNameOperation op => GetConverter(op.IndexContext).GetModuleNameExpression(op.Symbol),
                ModuleOutputOperation op => context.Settings.EnableSymbolicNames switch {
                    true => AppendProperties(
                        CreateFunction(
                            "reference",
                            GenerateSymbolicReference(op.Symbol.Name, op.IndexContext)),
                        new JTokenExpression("outputs"),
                        ConvertOperation(op.PropertyName),
                        new JTokenExpression("value")),
                    false => AppendProperties(
                        CreateFunction(
                            "reference",
                            GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol),
                            // TODO remove this - it's not necessary to emit the API version
                            new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                        new JTokenExpression("outputs"),
                        ConvertOperation(op.PropertyName),
                        new JTokenExpression("value")),
                },
                VariableAccessOperation op => CreateFunction(
                    "variables",
                    new JTokenExpression(op.Symbol.Name)),
                ParameterAccessOperation op => CreateFunction(
                    "parameters",
                    new JTokenExpression(op.Symbol.Name)),
                ModuleReferenceOperation op => context.Settings.EnableSymbolicNames switch {
                    true => CreateFunction(
                        "reference",
                        GenerateSymbolicReference(op.Symbol.Name, op.IndexContext)),
                    false => CreateFunction(
                        "reference",
                        GetConverter(op.IndexContext).GetFullyQualifiedResourceId(op.Symbol),
                        new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                },
                FunctionCallOperation op => CreateFunction(
                    op.Name,
                    op.Parameters.Select(p => ConvertOperation(p))),
                _ => throw new NotImplementedException(""),
            };
        }

        private Operation ConvertResourcePropertyAccess(ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName)
        {
            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return new PropertyAccessOperation(
                    new SymbolicResourceReferenceOperation(resource, indexContext, false),
                    propertyName);
            }

            // special cases for certain resource property access. if we recurse normally, we'll end up
            // generating statements like reference(resourceId(...)).id which are not accepted by ARM
            switch ((propertyName, context.Settings.EnableSymbolicNames))
            {
                case ("id", true):
                case ("name", true):
                case ("type", true):
                case ("apiVersion", true):
                    return new PropertyAccessOperation(
                        new ResourceInfoOperation(resource, indexContext),
                        propertyName);
                case ("id", false):
                    // the ID is dependent on the name expression which could involve locals in case of a resource collection
                    return new ResourceIdOperation(resource, indexContext);
                case ("name", false):
                    // the name is dependent on the name expression which could involve locals in case of a resource collection

                    // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                    // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                    return new ResourceNameOperation(resource, indexContext, fullyQualified: false);
                case ("type", false):
                    return new ResourceTypeOperation(resource);
                case ("apiVersion", false):
                    return new ResourceApiVersionOperation(resource);
                case ("properties", _):
                    // use the reference() overload without "full" to generate a shorter expression
                    // this is dependent on the name expression which could involve locals in case of a resource collection
                    return context.Settings.EnableSymbolicNames ?
                        new SymbolicResourceReferenceOperation(resource, indexContext, false) :
                        new ResourceReferenceOperation(
                            resource,
                            new ResourceIdOperation(resource, indexContext),
                            false);
                default:
                    return new PropertyAccessOperation(
                        context.Settings.EnableSymbolicNames ?
                            new SymbolicResourceReferenceOperation(resource, indexContext, true) :
                            new ResourceReferenceOperation(
                                resource,
                                new ResourceIdOperation(resource, indexContext),
                                true),
                        propertyName);
            }
        }

        private Operation ConvertModuleOutput(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext, string propertyName)
        {
            return new ModuleOutputOperation(
                moduleSymbol,
                indexContext,
                new ConstantValueOperation(propertyName));
        }

        private Operation ConvertModulePropertyAccess(ModuleSymbol moduleSymbol, string propertyName, IndexReplacementContext? indexContext)
        {
            switch (propertyName)
            {
                case LanguageConstants.ModuleNamePropertyName:
                    // the name is dependent on the name expression which could involve locals in case of a resource collection
                    return new ModuleNameOperation(moduleSymbol, indexContext);
                default:
                    throw new NotImplementedException("Property access is only implemented for module name");
            }
        }

        private Operation ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is { } resource)
            {
                var indexContext = TryGetReplacementContext(resource.NameSyntax, null, propertyAccess);
                return ConvertResourcePropertyAccess(resource, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is { } resourceCollection)
            {
                // we are doing property access on an array access of a resource collection
                var indexContext = TryGetReplacementContext(resourceCollection.NameSyntax, propArrayAccess.IndexExpression, propertyAccess);
                return ConvertResourcePropertyAccess(resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if (context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is ModuleSymbol moduleSymbol)
            {
                // we are doing property access on a single module
                var indexContext = TryGetReplacementContext(GetModuleNameSyntax(moduleSymbol), null, propertyAccess);
                return ConvertModulePropertyAccess(moduleSymbol, propertyAccess.PropertyName.IdentifierName, indexContext);
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax modulePropArrayAccess &&
                context.SemanticModel.GetSymbolInfo(modulePropArrayAccess.BaseExpression) is ModuleSymbol moduleCollectionSymbol)
            {
                // we are doing property access on an array access of a module collection
                var indexContext = TryGetReplacementContext(GetModuleNameSyntax(moduleCollectionSymbol), modulePropArrayAccess.IndexExpression, propertyAccess);
                return ConvertModulePropertyAccess(moduleCollectionSymbol, propertyAccess.PropertyName.IdentifierName, indexContext);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
                childPropertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName))
            {
                if (context.SemanticModel.GetSymbolInfo(childPropertyAccess.BaseExpression) is ModuleSymbol outputModuleSymbol)
                {
                    var indexContext = TryGetReplacementContext(GetModuleNameSyntax(outputModuleSymbol), null, propertyAccess);
                    return ConvertModuleOutput(outputModuleSymbol, indexContext, propertyAccess.PropertyName.IdentifierName);
                }

                if (childPropertyAccess.BaseExpression is ArrayAccessSyntax outputModulePropArrayAccess &&
                    context.SemanticModel.GetSymbolInfo(outputModulePropArrayAccess.BaseExpression) is ModuleSymbol outputArrayModuleSymbol)
                {
                    var indexContext = TryGetReplacementContext(GetModuleNameSyntax(outputArrayModuleSymbol), outputModulePropArrayAccess.IndexExpression, propertyAccess);
                    return ConvertModuleOutput(outputArrayModuleSymbol, indexContext, propertyAccess.PropertyName.IdentifierName);
                }
            }

            return new PropertyAccessOperation(
                ConvertExpressionOperation(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName);
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(ResourceMetadata resource)
        {
            // TODO move this into az extension
            var typeReference = resource.TypeReference;
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameSyntax = resource.NameSyntax;
            var nameExpression = ConvertExpression(nameSyntax);

            var typesAfterProvider = typeReference.TypeSegments.Skip(1).ToImmutableArray();

            if (ancestors.Length > 0)
            {
                var firstAncestorNameLength = typesAfterProvider.Length - ancestors.Length;

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

            if (typesAfterProvider.Length == 1)
            {
                return nameExpression.AsEnumerable();
            }

            return typesAfterProvider.Select(
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
            return moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        public LanguageExpression GetUnqualifiedResourceId(ResourceMetadata resource)
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
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
                resource.TypeReference.FormatType(),
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

        private Operation GetModuleOutputsReference(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            return new PropertyAccessOperation(
                new ModuleReferenceOperation(moduleSymbol, indexContext),
                "outputs");
        }

        private Operation GetResourceReference(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
            return context.Settings.EnableSymbolicNames ?
                new SymbolicResourceReferenceOperation(resource, indexContext, true) :
                new ResourceReferenceOperation(
                    resource,
                    new ResourceIdOperation(resource, indexContext),
                    true);
        }

        private Operation GetLocalVariable(LocalVariableSymbol localVariableSymbol)
        {
            if (this.localReplacements.TryGetValue(localVariableSymbol, out var replacement))
            {
                // the current context has specified an expression to be used for this local variable symbol
                // to override the regular conversion
                return replacement;
            }

            var @for = GetEnclosingForExpression(localVariableSymbol);
            return GetLoopVariable(localVariableSymbol, @for, CreateCopyIndexFunction(@for));
        }

        private Operation GetLoopVariable(LocalVariableSymbol localVariableSymbol, ForSyntax @for, Operation indexOperation)
        {
            return localVariableSymbol.LocalKind switch
            {
                // this is the "item" variable of a for-expression
                // to emit this, we need to index the array expression by the copyIndex() function
                LocalKind.ForExpressionItemVariable => GetLoopItemVariable(@for, indexOperation),

                // this is the "index" variable of a for-expression inside a variable block
                // to emit this, we need to return a copyIndex(...) function
                LocalKind.ForExpressionIndexVariable => indexOperation,

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

        private Operation CreateCopyIndexFunction(ForSyntax @for)
        {
            var copyIndexName = GetCopyIndexName(@for);
            return copyIndexName is null
                ? new FunctionCallOperation("copyIndex")
                : new FunctionCallOperation("copyIndex", new ConstantValueOperation(copyIndexName));
        }

        private Operation GetLoopItemVariable(ForSyntax @for, Operation indexOperation)
        {
            // loop item variable should be replaced with <array expression>[<index expression>]
            var forOperation = ConvertExpressionOperation(@for.Expression);

            return new ArrayAccessOperation(forOperation, indexOperation);
        }

        private Operation ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
                case ParameterSymbol parameterSymbol:
                    return new ParameterAccessOperation(parameterSymbol);

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpressionOperation(variableSymbol.DeclaringVariable.Value);
                    }

                    return new VariableAccessOperation(variableSymbol);

                case ResourceSymbol when context.SemanticModel.ResourceMetadata.TryLookup(variableAccessSyntax) is { } resource:
                    return GetResourceReference(resource, null, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleOutputsReference(moduleSymbol, null);

                case LocalVariableSymbol localVariableSymbol:
                    return GetLocalVariable(localVariableSymbol);

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private Operation ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
        {
            if (context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
            {
                return GetResourceReference(resource, null, true);
            }

            throw new NotImplementedException($"Unable to obtain resource metadata when generating a resource access expression.");
        }

        private Operation ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new ConstantValueOperation(literalStringValue);
            }

            var formatArgs = new Operation[syntax.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(syntax);
            formatArgs[0] = new ConstantValueOperation(formatString);

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpressionOperation(syntax.Expressions[i]);
            }

            return new FunctionCallOperation("format", formatArgs);
        }

        /// <summary>
        /// Converts a given language expression into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
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

        private Operation ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return new FunctionCallOperation(
                "createArray",
                syntax.Items.Select(item => ConvertExpressionOperation(item.Value)).ToImmutableArray());
        }

        private Operation ConvertObject(ObjectSyntax syntax)
        {
            // need keys and values in one array of parameters
            var parameters = new Operation[syntax.Properties.Count() * 2];

            int index = 0;
            foreach (var propertySyntax in syntax.Properties)
            {
                parameters[index] = propertySyntax.Key switch
                {
                    IdentifierSyntax identifier => new ConstantValueOperation(identifier.IdentifierName),
                    StringSyntax @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{propertySyntax.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpressionOperation(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return new FunctionCallOperation("createObject", parameters);
        }

        private Operation ConvertBinary(BinaryOperationSyntax syntax)
        {
            var operand1 = ConvertExpressionOperation(syntax.LeftExpression);
            var operand2 = ConvertExpressionOperation(syntax.RightExpression);

            return syntax.Operator switch
            {
                BinaryOperator.LogicalOr => new FunctionCallOperation("or", operand1, operand2),
                BinaryOperator.LogicalAnd => new FunctionCallOperation("and", operand1, operand2),
                BinaryOperator.Equals => new FunctionCallOperation("equals", operand1, operand2),
                BinaryOperator.NotEquals => new FunctionCallOperation("not",
                    new FunctionCallOperation("equals", operand1, operand2)),
                BinaryOperator.EqualsInsensitive => new FunctionCallOperation("equals",
                    new FunctionCallOperation("toLower", operand1),
                    new FunctionCallOperation("toLower", operand2)),
                BinaryOperator.NotEqualsInsensitive => new FunctionCallOperation("not",
                    new FunctionCallOperation("equals",
                        new FunctionCallOperation("toLower", operand1),
                        new FunctionCallOperation("toLower", operand2))),
                BinaryOperator.LessThan => new FunctionCallOperation("less", operand1, operand2),
                BinaryOperator.LessThanOrEqual => new FunctionCallOperation("lessOrEquals", operand1, operand2),
                BinaryOperator.GreaterThan => new FunctionCallOperation("greater", operand1, operand2),
                BinaryOperator.GreaterThanOrEqual => new FunctionCallOperation("greaterOrEquals", operand1, operand2),
                BinaryOperator.Add => new FunctionCallOperation("add", operand1, operand2),
                BinaryOperator.Subtract => new FunctionCallOperation("sub", operand1, operand2),
                BinaryOperator.Multiply => new FunctionCallOperation("mul", operand1, operand2),
                BinaryOperator.Divide => new FunctionCallOperation("div", operand1, operand2),
                BinaryOperator.Modulo => new FunctionCallOperation("mod", operand1, operand2),
                BinaryOperator.Coalesce => new FunctionCallOperation("coalesce", operand1, operand2),
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'."),
            };
        }

        private Operation ConvertUnary(UnaryOperationSyntax syntax)
        {
            var convertedOperand = ConvertExpressionOperation(syntax.Expression);

            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return new FunctionCallOperation("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is ConstantValueOperation literal && literal.Value is int intValue)
                    {
                        // invert the integer literal
                        return new ConstantValueOperation(-intValue);
                    }

                    return new FunctionCallOperation(
                        "sub",
                        new [] {
                            new ConstantValueOperation(0),
                            convertedOperand,
                        });

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
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

