// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit
{
    public class OperationConverter
    {
        private readonly EmitterContext context;
        private readonly ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements;

        public OperationConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Operation> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
        }

        public ProgramOperation ConvertProgram(FileSymbol fileSymbol)
        {
            var parameters = fileSymbol.ParameterDeclarations.Select(ConvertParameter);
            var outputs = fileSymbol.OutputDeclarations.Select(ConvertOutput);
            var imports = fileSymbol.ImportDeclarations.Select(ConvertImport);
            var variables = ConvertVariables(fileSymbol);

            return new ProgramOperation(
                parameters.ToImmutableArray(),
                outputs.ToImmutableArray(),
                imports.ToImmutableArray(),
                variables.ToImmutableArray());
        }

        private IEnumerable<VariableOperation> ConvertVariables(FileSymbol fileSymbol)
        {
            //emit internal variables
            foreach (var functionVariable in context.FunctionVariables.Values.OrderBy(x => x.Name, LanguageConstants.IdentifierComparer))
            {
                yield return new VariableOperation(
                    functionVariable.Name,
                    ConvertSyntax(functionVariable.Value));
            }

            var nonInlinedVariables = fileSymbol.VariableDeclarations.Where(x => !context.VariablesToInline.Contains(x));
            foreach (var variable in nonInlinedVariables)
            {
                yield return new VariableOperation(
                    variable.Name,
                    ConvertSyntax(variable.Value));
            }
        }

        private ParameterOperation ConvertParameter(ParameterSymbol parameterSymbol)
        {
            TypeSymbol targetType;

            var properties = new List<ObjectPropertyOperation>();
            if (parameterSymbol.Type is ResourceType resourceType)
            {
                // Encode a resource type as a string parameter with a metadata for the resource type.
                targetType = resourceType;
                properties.Add(new ObjectPropertyOperation(
                    new ConstantValueOperation("type"),
                    new ConstantValueOperation(LanguageConstants.String.Name)));
                properties.Add(new ObjectPropertyOperation(
                    new ConstantValueOperation(LanguageConstants.ParameterMetadataPropertyName),
                    new ObjectOperation(new [] {
                        new ObjectPropertyOperation(
                            new ConstantValueOperation(LanguageConstants.MetadataResourceTypePropertyName),
                            new ConstantValueOperation(resourceType.TypeReference.FormatName())),
                    }.ToImmutableArray())));
            }
            else if (SyntaxHelper.TryGetPrimitiveType(parameterSymbol.DeclaringParameter) is TypeSymbol primitiveType)
            {
                targetType = primitiveType;
                properties.Add(new ObjectPropertyOperation(
                    new ConstantValueOperation("type"),
                    new ConstantValueOperation(primitiveType.Name)));
            }
            else
            {
                // this should have been caught by the type checker long ago
                throw new ArgumentException($"Unable to find primitive type for parameter {parameterSymbol.Name}");
            }

            if (parameterSymbol.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
            {
                properties.Add(new ObjectPropertyOperation(
                    new ConstantValueOperation("defaultValue"),
                    ConvertSyntax(defaultValueSyntax.DefaultValue)));
            }

            properties.AddRange(GetDecorators(parameterSymbol.DeclaringParameter, targetType));

            return new ParameterOperation(
                parameterSymbol.Name,
                properties.ToImmutableArray());
        }

        private OutputOperation ConvertOutput(OutputSymbol outputSymbol)
        {
            TypeSymbol targetType;

            ObjectSyntax objectWithDecorators;
            Operation value;
            var properties = new List<ObjectPropertyOperation>();
            if (outputSymbol.Type is ResourceType resourceType)
            {
                // TODO(antmarti): Handle array access here
                if (context.SemanticModel.ResourceMetadata.TryLookup(outputSymbol.Value) is not {} resourceMetadata)
                {
                    throw new InvalidOperationException($"Failed to find resource metadata for resource output {outputSymbol.Name}");
                }

                // Resource-typed outputs are encoded as strings
                targetType = LanguageConstants.String;
                properties.Add(new ObjectPropertyOperation(
                    new ConstantValueOperation(LanguageConstants.ParameterMetadataPropertyName),
                    new ObjectOperation(new [] {
                        new ObjectPropertyOperation(
                            new ConstantValueOperation(LanguageConstants.MetadataResourceTypePropertyName),
                            new ConstantValueOperation(resourceType.TypeReference.FormatName())),
                    }.ToImmutableArray())));

                // Resource-typed outputs are serialized using the resource id.
                value = new ResourceIdOperation(resourceMetadata, null);
            }
            else
            {
                targetType = outputSymbol.Type;
                objectWithDecorators = SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>());
                value = ConvertSyntax(outputSymbol.Value);
            }

            properties.AddRange(GetDecorators(outputSymbol.DeclaringOutput, targetType));

            return new OutputOperation(
                outputSymbol.Name,
                targetType.Name,
                value,
                properties.ToImmutableArray());
        }

        private ImportOperation ConvertImport(ImportedNamespaceSymbol import)
        {
            var namespaceType = context.SemanticModel.GetTypeInfo(import.DeclaringSyntax) as NamespaceType
                ?? throw new ArgumentException("Imported namespace does not have namespace type");

            return new ImportOperation(
                import.DeclaringImport.AliasName.IdentifierName,
                namespaceType,
                import.DeclaringImport.Config is null ? null : ConvertSyntax(import.DeclaringImport.Config));
        }

        public IEnumerable<ObjectPropertyOperation> GetDecorators(StatementSyntax statement, TypeSymbol targetType)
        {
            var result = SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>());
            foreach (var decoratorSyntax in statement.Decorators.Reverse())
            {
                var symbol = this.context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

                if (symbol is FunctionSymbol decoratorSymbol &&
                    decoratorSymbol.DeclaringObject is NamespaceType namespaceType &&
                    TemplateWriter.DecoratorsToEmitAsResourceProperties.Contains(decoratorSymbol.Name))
                {
                    var argumentTypes = decoratorSyntax.Arguments
                        .Select(argument => this.context.SemanticModel.TypeManager.GetTypeInfo(argument))
                        .ToArray();

                    // There should be exact one matching decorator since there's no errors.
                    var decorator = namespaceType.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes).Single();

                    var evaluated = decorator.Evaluate(decoratorSyntax, targetType, result);
                    if (evaluated is not null)
                    {
                        result = evaluated;
                    }
                }
            }

            foreach (var property in result.Properties)
            {
                yield return new ObjectPropertyOperation(
                    property.TryGetKeyText() is { } keyName ? new ConstantValueOperation(keyName) : ConvertSyntax(property.Key),
                    ConvertSyntax(property.Value));
            }
        }

        public Operation ConvertSyntax(SyntaxBase syntax)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(syntax);
            if (symbol is VariableSymbol variableSymbol && context.VariablesToInline.Contains(variableSymbol))
            {
                return ConvertSyntax(variableSymbol.Value);
            }

            if (syntax is FunctionCallSyntax functionCall &&
                symbol is FunctionSymbol functionSymbol &&
                string.Equals(functionSymbol.Name, LanguageConstants.AnyFunction, LanguageConstants.IdentifierComparison))
            {
                // the outermost function in the current syntax node is the "any" function
                // we should emit its argument directly
                // otherwise, they'd get wrapped in a json() template function call in the converted expression

                // we have checks for function parameter count mismatch, which should prevent an exception from being thrown
                return ConvertSyntax(functionCall.Arguments.Single().Expression);
            }

            switch (syntax)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return new ConstantValueOperation(boolSyntax.Value);

                case IntegerLiteralSyntax integerSyntax:
                    var longValue = integerSyntax.Value switch {
                        <= long.MaxValue => (long)integerSyntax.Value,
                        _ => throw new InvalidOperationException($"Integer syntax hs value {integerSyntax.Value} which will overflow"),
                    };

                    return new ConstantValueOperation(longValue);

                case NullLiteralSyntax _:
                    return new NullValueOperation();

                case ObjectSyntax objectSyntax:
                    var properties = objectSyntax.Properties.Select(prop => new ObjectPropertyOperation(
                        prop.TryGetKeyText() is { } keyName ? new ConstantValueOperation(keyName) : ConvertSyntax(prop.Key),
                        ConvertSyntax(prop.Value)));
                    return new ObjectOperation(properties.ToImmutableArray());

                case ObjectPropertySyntax prop:
                    return new ObjectPropertyOperation(
                        prop.TryGetKeyText() is { } keyName ? new ConstantValueOperation(keyName) : ConvertSyntax(prop.Key),
                        ConvertSyntax(prop.Value));

                case ArraySyntax arraySyntax:
                    var items = arraySyntax.Items.Select(x => ConvertSyntax(x.Value));
                    return new ArrayOperation(items.ToImmutableArray());

                case ArrayItemSyntax arrayItemSyntax:
                    return ConvertSyntax(arrayItemSyntax.Value);

                case ForSyntax forSyntax:
                    return new ForLoopOperation(
                        ConvertSyntax(forSyntax.Expression),
                        ConvertSyntax(forSyntax.Body));

                case ParenthesizedExpressionSyntax _:
                case UnaryOperationSyntax _:
                case BinaryOperationSyntax _:
                case TernaryOperationSyntax _:
                case StringSyntax _:
                case InstanceFunctionCallSyntax _:
                case FunctionCallSyntax _:
                case ArrayAccessSyntax _:
                case PropertyAccessSyntax _:
                case ResourceAccessSyntax _:
                case VariableAccessSyntax _:
                    return ConvertExpressionSyntax(syntax);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        public Operation ConvertExpressionSyntax(SyntaxBase expression)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return new ConstantValueOperation(boolSyntax.Value);

                case IntegerLiteralSyntax integerSyntax:
                    var longValue = integerSyntax.Value switch {
                        <= long.MaxValue => (long)integerSyntax.Value,
                        _ => throw new InvalidOperationException($"Integer syntax hs value {integerSyntax.Value} which will overflow"),
                    };

                    return new ConstantValueOperation(longValue);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);

                case NullLiteralSyntax _:
                    return new NullValueOperation();

                case ObjectSyntax @object:
                    return ConvertObject(@object);

                case ArraySyntax array:
                    return ConvertArray(array);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return ConvertExpressionSyntax(parenthesized.Expression);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return new FunctionCallOperation(
                        "if",
                        ConvertExpressionSyntax(ternary.ConditionExpression),
                        ConvertExpressionSyntax(ternary.TrueExpression),
                        ConvertExpressionSyntax(ternary.FalseExpression));

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

        private Operation ConvertArray(ArraySyntax syntax)
        {
            var items = syntax.Items.Select(ConvertSyntax);

            return new ArrayOperation(items.ToImmutableArray());
        }

        private Operation ConvertObject(ObjectSyntax syntax)
        {
            var properties = syntax.Properties.Select(prop => new ObjectPropertyOperation(
                prop.TryGetKeyText() is { } keyName ? new ConstantValueOperation(keyName) : ConvertSyntax(prop.Key),
                ConvertSyntax(prop.Value)));

            return new ObjectOperation(properties.ToImmutableArray());
        }

        private Operation ConvertBinary(BinaryOperationSyntax syntax)
        {
            var operand1 = ConvertExpressionSyntax(syntax.LeftExpression);
            var operand2 = ConvertExpressionSyntax(syntax.RightExpression);

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
            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return new FunctionCallOperation("not", ConvertExpressionSyntax(syntax.Expression));

                case UnaryOperator.Minus:
                    if (syntax.Expression is IntegerLiteralSyntax integerLiteral)
                    {
                        var integerValue = integerLiteral.Value switch {
                            <= long.MaxValue => -(long)integerLiteral.Value,
                            (ulong)long.MaxValue + 1 => long.MinValue,
                            _ => throw new InvalidOperationException($"Integer syntax hs value {integerLiteral.Value} which will overflow"),
                        };

                        return new ConstantValueOperation(integerValue);
                    }

                    return new FunctionCallOperation(
                        "sub",
                        new[] {
                            new ConstantValueOperation(0),
                            ConvertExpressionSyntax(syntax.Expression),
                        });

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        private Operation ConvertFunction(FunctionCallSyntaxBase functionCall)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
            if (symbol is FunctionSymbol &&
                context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: { } } functionOverload)
            {
                return ConvertExpressionSyntax(functionOverload.Evaluator(
                    functionCall,
                    symbol,
                    context.SemanticModel.GetTypeInfo(functionCall),
                    context.FunctionVariables.GetValueOrDefault(functionCall),
                    context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall)));
            }

            switch (functionCall)
            {
                case FunctionCallSyntax function:
                    return new FunctionCallOperation(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpressionSyntax(a.Expression)).ToImmutableArray());

                case InstanceFunctionCallSyntax method:
                    var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(method.BaseExpression);
                    var baseSymbol = context.SemanticModel.GetSymbolInfo(baseSyntax);

                    switch (baseSymbol)
                    {
                        case INamespaceSymbol namespaceSymbol:
                            Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                            return new FunctionCallOperation(
                                method.Name.IdentifierName,
                                method.Arguments.Select(a => ConvertExpressionSyntax(a.Expression)).ToImmutableArray());
                        case { } _ when context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is DeclaredResourceMetadata resource:
                            if (method.Name.IdentifierName.StartsWithOrdinalInsensitively("list"))
                            {
                                // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                                var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);
                                var resourceIdOperation = new ResourceIdOperation(resource, indexContext);

                                var convertedArgs = method.Arguments.SelectArray(a => ConvertExpressionSyntax(a.Expression));

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

                            if (LanguageConstants.IdentifierComparer.Equals(method.Name.IdentifierName, "getSecret"))
                            {
                                var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);
                                var resourceIdOperation = new ResourceIdOperation(resource, indexContext);

                                var convertedArgs = method.Arguments.SelectArray(a => ConvertExpressionSyntax(a.Expression));

                                return new GetKeyVaultSecretOperation(
                                    resourceIdOperation,
                                    convertedArgs[0],
                                    convertedArgs.Length > 1 ? convertedArgs[1] : null);
                            }

                            break;
                    }
                    throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
            }
        }

        private Operation ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
        {
            // if there is an array access on a resource/module reference, we have to generate differently
            // when constructing the reference() function call, the resource name expression needs to have its local
            // variable replaced with <loop array expression>[this array access' index expression]
            if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
            {
                if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is DeclaredResourceMetadata resource &&
                    resource.Symbol.IsCollection)
                {
                    var indexContent = TryGetReplacementContext(resource, arrayAccess.IndexExpression, arrayAccess);
                    return GetResourceReference(resource, indexContent, full: true);
                }

                if (context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
                {
                    var indexContent = TryGetReplacementContext(GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);
                    return GetModuleOutputsReference(moduleSymbol, indexContent);
                }
            }

            return new ArrayAccessOperation(
                ConvertExpressionSyntax(arrayAccess.BaseExpression),
                ConvertExpressionSyntax(arrayAccess.IndexExpression));
        }

        private Operation ConvertResourcePropertyAccess(ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName)
        {
            if (context.Settings.EnableSymbolicNames &&
                resource is DeclaredResourceMetadata declaredResource)
            {
                if (!resource.IsAzResource)
                {
                    // For an extensible resource, always generate a 'reference' statement.
                    // User-defined properties appear inside "properties", so use a non-full reference.
                    return new PropertyAccessOperation(
                        new SymbolicResourceReferenceOperation(declaredResource, indexContext, false),
                        propertyName);
                }

                if (context.Settings.EnableSymbolicNames)
                {
                    switch (propertyName)
                    {
                        case "id":
                        case "name":
                        case "type":
                        case "apiVersion":
                            return new PropertyAccessOperation(
                                new ResourceInfoOperation(declaredResource, indexContext),
                                propertyName);
                        case "properties":
                            return new SymbolicResourceReferenceOperation(declaredResource, indexContext, false);
                        default:
                            return new PropertyAccessOperation(
                                new SymbolicResourceReferenceOperation(declaredResource, indexContext, true),
                                propertyName);
                    }
                }
            }

            switch (propertyName)
            {
                case "id":
                    // the ID is dependent on the name expression which could involve locals in case of a resource collection
                    return new ResourceIdOperation(resource, indexContext);
                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection

                    // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                    // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                    return new ResourceNameOperation(resource, indexContext, FullyQualified: false);
                case "type":
                    return new ResourceTypeOperation(resource);
                case "apiVersion":
                    return new ResourceApiVersionOperation(resource);
                case "properties":
                    var shouldIncludeApiVersion = resource.IsExistingResource ||
                        (resource is DeclaredResourceMetadata { Symbol.DeclaringResource: var declaringResource } && declaringResource.HasCondition());

                    return new ResourceReferenceOperation(
                        resource,
                        new ResourceIdOperation(resource, indexContext),
                        Full: false,
                        ShouldIncludeApiVersion: shouldIncludeApiVersion);
                default:
                    return new PropertyAccessOperation(
                        new ResourceReferenceOperation(
                            resource,
                            new ResourceIdOperation(resource, indexContext),
                            Full: true,
                            ShouldIncludeApiVersion: true),
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
            if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
            {
                // we are doing property access on a single resource
                var indexContext = TryGetReplacementContext(resource, null, propertyAccess);
                return ConvertResourcePropertyAccess(resource, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if ((propertyAccess.BaseExpression is VariableAccessSyntax || propertyAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ResourceMetadata parameter)
            {
                // we are doing property access on a single resource
                // and we are dealing with special case properties
                return ConvertResourcePropertyAccess(parameter, null, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
            {
                // we are doing property access on an array access of a resource collection
                var indexContext = TryGetReplacementContext(resourceCollection, propArrayAccess.IndexExpression, propertyAccess);
                return ConvertResourcePropertyAccess(resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
                !moduleOutput.Module.IsCollection)
            {
                // we are doing property access on an output of a non-collection module.
                // and we are dealing with special case properties
                return this.ConvertResourcePropertyAccess(moduleOutput, null, propertyAccess.PropertyName.IdentifierName);
            }

            if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
                moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
                moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
                context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
                moduleCollectionOutputMetadata.Module.IsCollection)
            {
                // we are doing property access on an output of an array of modules.
                // and we are dealing with special case properties
                var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess);
                return ConvertResourcePropertyAccess(moduleCollectionOutputMetadata, indexContext, propertyAccess.PropertyName.IdentifierName);
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
                if (childPropertyAccess.BaseExpression is VariableAccessSyntax grandChildVariableAccess &&
                    context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is VariableSymbol variableSymbol &&
                    context.VariablesToInline.Contains(variableSymbol))
                {
                    // This is imprecise as we don't check that that variable being accessed is solely composed of modules. We'd end up generating incorrect code for:
                    // var foo = false ? mod1 : varWithOutputs
                    // var bar = foo.outputs.someProp
                    return new PropertyAccessOperation(
                        new PropertyAccessOperation(
                            ConvertVariableAccess(grandChildVariableAccess),
                            propertyAccess.PropertyName.IdentifierName),
                        "value");
                }

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
                ConvertExpressionSyntax(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName);
        }

        private Operation ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            var name = variableAccessSyntax.Name.IdentifierName;

            if (variableAccessSyntax is ExplicitVariableAccessSyntax)
            {
                //just return a call to variables.
                return new ExplicitVariableAccessOperation(name);
            }

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
                case ParameterSymbol parameterSymbol when context.SemanticModel.ResourceMetadata.TryLookup(parameterSymbol.DeclaringSyntax) is {} resource:
                    // This is a reference to a pre-existing resource where the resource ID was passed in as a
                    // string. Generate a call to reference().
                    return GetResourceReference(resource, null, true);
                case ParameterSymbol parameterSymbol:
                    return new ParameterAccessOperation(parameterSymbol);

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpressionSyntax(variableSymbol.DeclaringVariable.Value);
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
                formatArgs[i + 1] = ConvertExpressionSyntax(syntax.Expressions[i]);
            }

            return new FunctionCallOperation("format", formatArgs);
        }

        private Operation GetModuleOutputsReference(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            return new PropertyAccessOperation(
                new ModuleReferenceOperation(
                    moduleSymbol,
                    indexContext),
                "outputs");
        }

        private Operation GetResourceReference(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
            return (context.Settings.EnableSymbolicNames, resource) switch {
                (true, DeclaredResourceMetadata declaredResource) => new SymbolicResourceReferenceOperation(declaredResource, indexContext, true),
                _ => new ResourceReferenceOperation(
                    resource,
                    new ResourceIdOperation(resource, indexContext),
                    Full: true,
                    ShouldIncludeApiVersion: true),
            };
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

        public ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
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
            var forOperation = ConvertExpressionSyntax(@for.Expression);

            return new ArrayAccessOperation(forOperation, indexOperation);
        }

        public IndexReplacementContext? TryGetReplacementContext(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var movedSyntax = context.Settings.EnableSymbolicNames ? resource.Symbol.NameSyntax : resource.NameSyntax;

            return TryGetReplacementContext(movedSyntax, indexExpression, newContext);
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

                    return new(this.localReplacements, ConvertExpressionSyntax(indexExpression));

                case 1 when indexExpression is not null:
                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();
                    var localReplacements = this.localReplacements;
                    var converter = new OperationConverter(this.context, localReplacements);
                    foreach (var local in inaccessibleLocals)
                    {
                        // Allow local variable symbol replacements to be overwritten, as there are scenarios where we recursively generate expressions for the same index symbol
                        var replacementValue = GetLoopVariable(local, @for, converter.ConvertExpressionSyntax(indexExpression));
                        localReplacements = localReplacements.SetItem(local, replacementValue);
                    }

                    return new(localReplacements, converter.ConvertExpressionSyntax(indexExpression));

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
            }
        }

        public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }
    }
}

