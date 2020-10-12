// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Arm.Expression.Expressions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        public ExpressionConverter(EmitterContext context)
        {
            this.context = context;
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
                    return CreateParameterlessFunction(boolSyntax.Value ? "true" : "false");
                    
                case NumericLiteralSyntax numericSyntax:
                    return new JTokenExpression(numericSyntax.Value);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);
                    
                case NullLiteralSyntax _:
                    return CreateParameterlessFunction("null");

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
                    return new FunctionExpression(
                        "if",
                        new[]
                        {
                            ConvertExpression(ternary.ConditionExpression),
                            ConvertExpression(ternary.TrueExpression),
                            ConvertExpression(ternary.FalseExpression)
                        },
                        Array.Empty<LanguageExpression>());

                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpression(a.Expression)).ToArray());

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    var namespaceSymbol = context.SemanticModel.GetSymbolInfo(instanceFunctionCall.BaseExpression);
                    
                    Assert(namespaceSymbol is NamespaceSymbol, $"BaseExpression must be a NamespaceSymbol, instead got: '{namespaceSymbol?.Kind}'");

                    return ConvertFunction(
                        instanceFunctionCall.Name.IdentifierName,
                        instanceFunctionCall.Arguments.Select(a => ConvertExpression(a.Expression)).ToArray());

                case ArrayAccessSyntax arrayAccess:
                    return AppendProperty(
                        ToFunctionExpression(arrayAccess.BaseExpression),
                        ConvertExpression(arrayAccess.IndexExpression));

                case PropertyAccessSyntax propertyAccess:
                    if (propertyAccess.BaseExpression is VariableAccessSyntax propVariableAccess &&
                        context.SemanticModel.GetSymbolInfo(propVariableAccess) is ResourceSymbol resourceSymbol)
                    {
                        // special cases for certain resource property access. if we recurse normally, we'll end up
                        // generating statements like reference(resourceId(...)).id which are not accepted by ARM

                        var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                        switch (propertyAccess.PropertyName.IdentifierName)
                        {
                            case "id":
                                return GetResourceIdExpression(resourceSymbol.DeclaringResource, typeReference);
                            case "name":
                                return GetResourceNameExpression(resourceSymbol.DeclaringResource);
                            case "type":
                                return new JTokenExpression(typeReference.FullyQualifiedType);
                            case "apiVersion":
                                return new JTokenExpression(typeReference.ApiVersion);
                            case "properties":
                                // use the reference() overload without "full" to generate a shorter expression
                                return GetReferenceExpression(resourceSymbol.DeclaringResource, typeReference, false);
                        }
                    }

                    if (propertyAccess.BaseExpression is VariableAccessSyntax propVariableAccessModule && 
                        context.SemanticModel.GetSymbolInfo(propVariableAccessModule) is ModuleSymbol moduleSymbol)
                    {
                        return AppendProperty(
                            AppendProperty(
                                GetModuleReferenceExpression(moduleSymbol),
                                new JTokenExpression(propertyAccess.PropertyName.IdentifierName)),
                            new JTokenExpression("value"));
                    }

                    return AppendProperty(
                        ToFunctionExpression(propertyAccess.BaseExpression),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName));

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private LanguageExpression GetResourceNameExpression(ResourceDeclarationSyntax resourceSyntax)
        {
            if (!(resourceSyntax.Body is ObjectSyntax objectSyntax))
            {
                // this condition should have already been validated by the type checker
                throw new ArgumentException($"Expected resource syntax to have type {typeof(ObjectSyntax)}, but found {resourceSyntax.Body.GetType()}");
            }

            var namePropertySyntax = objectSyntax.Properties.FirstOrDefault(p => LanguageConstants.IdentifierComparer.Equals(p.TryGetKeyText(), "name"));
            if (namePropertySyntax == null)
            {
                // this condition should have already been validated by the type checker
                throw new ArgumentException($"Expected resource syntax body to contain property 'name'");
            }

            return ConvertExpression(namePropertySyntax.Value);
        }

        public FunctionExpression GetResourceIdExpression(ResourceDeclarationSyntax resourceSyntax, ResourceTypeReference typeReference)
        {
            if (typeReference.Types.Length == 1)
            {
                return new FunctionExpression(
                    "resourceId",
                    new LanguageExpression[]
                    {
                        new JTokenExpression(typeReference.FullyQualifiedType),
                        GetResourceNameExpression(resourceSyntax),
                    },
                    Array.Empty<LanguageExpression>());
            }

            var nameSegments = typeReference.Types.Select(
                (type, i) => new FunctionExpression(
                    "split",
                    new LanguageExpression[] { GetResourceNameExpression(resourceSyntax), new JTokenExpression("/") },
                    new LanguageExpression[] { new JTokenExpression(i) }));

            return new FunctionExpression(
                "resourceId",
                new LanguageExpression[]
                {
                    new JTokenExpression(typeReference.FullyQualifiedType),
                }.Concat(nameSegments).ToArray(),
                Array.Empty<LanguageExpression>());
        }

        public FunctionExpression GetModuleResourceIdExpression(ModuleSymbol moduleSymbol)
        {
            return new FunctionExpression(
                "resourceId",
                new LanguageExpression[]
                {
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceType),
                    new JTokenExpression(moduleSymbol.Name),
                },
                Array.Empty<LanguageExpression>());
        }
        
        public FunctionExpression GetModuleReferenceExpression(ModuleSymbol moduleSymbol)
        {
            return new FunctionExpression(
                "reference",
                new LanguageExpression[]
                {
                    GetModuleResourceIdExpression(moduleSymbol),
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion),
                },
                new LanguageExpression[]
                {
                    new JTokenExpression("outputs"),
                });
        }

        public FunctionExpression GetReferenceExpression(ResourceDeclarationSyntax resourceSyntax, ResourceTypeReference typeReference, bool full)
        {
            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                return new FunctionExpression(
                    "reference",
                    new LanguageExpression[]
                    {
                        GetResourceIdExpression(resourceSyntax, typeReference),
                        new JTokenExpression(typeReference.ApiVersion),
                        new JTokenExpression("full"),
                    },
                    Array.Empty<LanguageExpression>());
            }

            return new FunctionExpression(
                "reference",
                new LanguageExpression[]
                {
                    GetResourceIdExpression(resourceSyntax, typeReference),
                },
                Array.Empty<LanguageExpression>());
        }

        private LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            // TODO: This will change to support inlined functions like reference() or list*()
            switch (symbol)
            {
                case ParameterSymbol _:
                    return CreateUnaryFunction("parameters", new JTokenExpression(name));

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpression(variableSymbol.DeclaringVariable.Value);
                    }
                    return CreateUnaryFunction("variables", new JTokenExpression(name));

                case ResourceSymbol resourceSymbol:
                    var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                    return GetReferenceExpression(resourceSymbol.DeclaringResource, typeReference, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleReferenceExpression(moduleSymbol);

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private LanguageExpression ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new JTokenExpression(literalStringValue);;
            }

            if (syntax.Expressions.Length == 1)
            {
                const string emptyStringOpen = LanguageConstants.StringDelimiter + LanguageConstants.StringHoleOpen; // '${
                const string emptyStringClose = LanguageConstants.StringHoleClose + LanguageConstants.StringDelimiter; // }'

                // Special-case interpolation of format '${myValue}' because it's a common pattern for userAssignedIdentities.
                // There's no need for a 'format' function because we just have a single expression with no outer formatting.
                if (syntax.StringTokens[0].Text == emptyStringOpen && syntax.StringTokens[1].Text == emptyStringClose)
                {
                    return ConvertExpression(syntax.Expressions[0]);
                }
            }

            var formatArgs = new LanguageExpression[syntax.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(syntax);
            formatArgs[0] = new JTokenExpression(formatString);

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpression(syntax.Expressions[i]);
            }

            return new FunctionExpression("format", formatArgs, Array.Empty<LanguageExpression>());
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
                            return CreateUnaryFunction("int", valueExpression);

                        case JTokenType.String:
                            // convert string literal to function call via string() function
                            return CreateUnaryFunction("string", valueExpression);
                    }

                    break;
            }

            throw new NotImplementedException($"Unexpected expression type '{converted.GetType().Name}'.");
        }

        private static LanguageExpression ConvertFunction(string functionName, LanguageExpression[] arguments)
        {
            if (string.Equals("any", functionName, LanguageConstants.IdentifierComparison))
            {
                // this is the any function - don't generate a function call for it
                return arguments.Single();
            }

            return new FunctionExpression(functionName, arguments, Array.Empty<LanguageExpression>());
        }

        private FunctionExpression ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return new FunctionExpression(
                "createArray",
                syntax.Items.Select(item => ConvertExpression(item.Value)).ToArray(),
                Array.Empty<LanguageExpression>());
        }

        private FunctionExpression ConvertObject(ObjectSyntax syntax)
        {
            // need keys and values in one array of parameters
            var parameters = new LanguageExpression[syntax.Properties.Count() * 2];

            int index = 0;
            foreach (var propertySyntax in syntax.Properties)
            {
                parameters[index] = new JTokenExpression(propertySyntax.TryGetKeyText());
                index++;

                parameters[index] = ConvertExpression(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() funciton as a proy for an object literal
            return new FunctionExpression(
                "createObject",
                parameters,
                Array.Empty<LanguageExpression>());
        }

        private LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = ConvertExpression(syntax.LeftExpression);
            LanguageExpression operand2 = ConvertExpression(syntax.RightExpression);

            switch (syntax.Operator)
            {
                case BinaryOperator.LogicalOr:
                    return CreateBinaryFunction("or", operand1, operand2);

                case BinaryOperator.LogicalAnd:
                    return CreateBinaryFunction("and", operand1, operand2);

                case BinaryOperator.Equals:
                    return CreateBinaryFunction("equals", operand1, operand2);

                case BinaryOperator.NotEquals:
                    return CreateUnaryFunction("not", 
                        CreateBinaryFunction("equals", operand1, operand2));

                case BinaryOperator.EqualsInsensitive:
                    return CreateBinaryFunction("equals",
                        CreateUnaryFunction("toLower", operand1),
                        CreateUnaryFunction("toLower", operand2));

                case BinaryOperator.NotEqualsInsensitive:
                    return CreateUnaryFunction("not",
                        CreateBinaryFunction("equals",
                            CreateUnaryFunction("toLower", operand1),
                            CreateUnaryFunction("toLower", operand2)));

                case BinaryOperator.LessThan:
                    return CreateBinaryFunction("less", operand1, operand2);

                case BinaryOperator.LessThanOrEqual:
                    return CreateBinaryFunction("lessOrEquals", operand1, operand2);

                case BinaryOperator.GreaterThan:
                    return CreateBinaryFunction("greater", operand1, operand2);

                case BinaryOperator.GreaterThanOrEqual:
                    return CreateBinaryFunction("greaterOrEquals", operand1, operand2);

                case BinaryOperator.Add:
                    return CreateBinaryFunction("add", operand1, operand2);

                case BinaryOperator.Subtract:
                    return CreateBinaryFunction("sub", operand1, operand2);

                case BinaryOperator.Multiply:
                    return CreateBinaryFunction("mul", operand1, operand2);

                case BinaryOperator.Divide:
                    return CreateBinaryFunction("div", operand1, operand2);

                case BinaryOperator.Modulo:
                    return CreateBinaryFunction("mod", operand1, operand2);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'.");
            }
        }

        private LanguageExpression ConvertUnary(UnaryOperationSyntax syntax)
        {
            LanguageExpression convertedOperand = ConvertExpression(syntax.Expression);

            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return CreateUnaryFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is JTokenExpression literal && literal.Value.Type == JTokenType.Integer)
                    {
                        // invert the integer literal
                        int literalValue = literal.Value.Value<int>();
                        return new JTokenExpression(-literalValue);
                    }

                    return new FunctionExpression(
                        "sub",
                        new[] {new JTokenExpression(0), convertedOperand},
                        Array.Empty<LanguageExpression>());

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        private static FunctionExpression AppendProperty(FunctionExpression function, LanguageExpression newProperty) => 
            // technically we could just mutate the provided function object, but let's hold off on that optimization
            // until we have evidence that it is needed
            new FunctionExpression(function.Function, function.Parameters, function.Properties.Append(newProperty).ToArray());

        private static FunctionExpression CreateParameterlessFunction(string function) =>
            new FunctionExpression(function, Array.Empty<LanguageExpression>(), Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateUnaryFunction(string function, LanguageExpression operand) => 
            new FunctionExpression(function, new[] {operand}, Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateBinaryFunction(string name, LanguageExpression operand1, LanguageExpression operand2) =>
            new FunctionExpression(name, new[] {operand1, operand2}, Array.Empty<LanguageExpression>());

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

