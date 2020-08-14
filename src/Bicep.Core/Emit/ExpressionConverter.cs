using System;
using System.IO;
using System.Linq;
using System.Text;
using Azure.ResourceManager.Deployments.Expression.Expressions;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public static class ExpressionConverter
    {
        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="model">The semantic model</param>
        public static LanguageExpression ToTemplateExpression(this SyntaxBase expression, SemanticModel.SemanticModel model)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return CreateJsonFunctionCall(boolSyntax.Value);
                    
                case NumericLiteralSyntax numericSyntax:
                    return new JTokenExpression(numericSyntax.Value);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax, model);
                    
                case NullLiteralSyntax _:
                    return CreateJsonFunctionCall(JValue.CreateNull());

                case ObjectSyntax _:
                case ArraySyntax _:
                    return ConvertComplexLiteral(expression, model);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return parenthesized.Expression.ToTemplateExpression(model);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary, model);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary, model);

                case TernaryOperationSyntax ternary:
                    return new FunctionExpression(
                        "if",
                        new[]
                        {
                            ternary.ConditionExpression.ToTemplateExpression(model),
                            ternary.TrueExpression.ToTemplateExpression(model),
                            ternary.FalseExpression.ToTemplateExpression(model)
                        },
                        Array.Empty<LanguageExpression>());

                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.FunctionName.IdentifierName,
                        function.Arguments.Select(a => a.Expression.ToTemplateExpression(model)).ToArray());

                case ArrayAccessSyntax arrayAccess:
                    return AppendProperty(
                        arrayAccess.BaseExpression.ToFunctionExpression(model),
                        arrayAccess.IndexExpression.ToTemplateExpression(model));

                case PropertyAccessSyntax propertyAccess:
                    return AppendProperty(
                        propertyAccess.BaseExpression.ToFunctionExpression(model),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName));

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess, model);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private static LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax, SemanticModel.SemanticModel model)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = model.GetSymbolInfo(variableAccessSyntax);

            // TODO: This will change to support inlined functions like reference() or list*()
            switch (symbol)
            {
                case ParameterSymbol _:
                    return CreateUnaryFunction("parameters", new JTokenExpression(name));

                case VariableSymbol _:
                    return CreateUnaryFunction("variables", new JTokenExpression(name));

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private static LanguageExpression ConvertString(StringSyntax syntax, SemanticModel.SemanticModel model)
        {
            var stringExpression = new JTokenExpression(syntax.GetFormatString());

            if (!syntax.IsInterpolated())
            {
                // no need to build a format string
                return stringExpression;
            }

            var formatArgs = new LanguageExpression[syntax.Expressions.Length + 1];
            formatArgs[0] = stringExpression;
            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = syntax.Expressions[i].ToTemplateExpression(model);
            }

            return new FunctionExpression("format", formatArgs, Array.Empty<LanguageExpression>());
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="model"></param>
        public static FunctionExpression ToFunctionExpression(this SyntaxBase expression, SemanticModel.SemanticModel model)
        {
            var converted = expression.ToTemplateExpression(model);
            switch (converted)
            {
                case FunctionExpression functionExpression:
                    return functionExpression;

                case JTokenExpression valueExpression:
                    JToken value = valueExpression.EvaluateExpression(null);

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

        private static FunctionExpression ConvertComplexLiteral(SyntaxBase syntax, SemanticModel.SemanticModel model)
        {
            // the tree node here should not contain any expressions inside
            // if it does, the emitted json expressions will not evaluate as expected due to IL limitations
            // there is a check elsewhere that will generate an error before we get this far in those cases
            var buffer = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(buffer)) {Formatting = Formatting.None})
            {
                ExpressionEmitter.EmitExpression(writer, syntax, model);
            }

            return CreateJsonFunctionCall(JToken.Parse(buffer.ToString()));
        }

        private static LanguageExpression ConvertBinary(BinaryOperationSyntax syntax, SemanticModel.SemanticModel model)
        {
            LanguageExpression operand1 = syntax.LeftExpression.ToTemplateExpression(model);
            LanguageExpression operand2 = syntax.RightExpression.ToTemplateExpression(model);

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

        private static LanguageExpression ConvertUnary(UnaryOperationSyntax syntax, SemanticModel.SemanticModel model)
        {
            LanguageExpression convertedOperand = syntax.Expression.ToTemplateExpression(model);

            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return CreateUnaryFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is JTokenExpression literal && literal.EvaluateExpression(null).Type == JTokenType.Integer)
                    {
                        // invert the integer literal
                        int literalValue = literal.EvaluateExpression(null).Value<int>();
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

        private static FunctionExpression CreateUnaryFunction(string function, LanguageExpression operand) => 
            new FunctionExpression(function, new[] {operand}, Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateBinaryFunction(string name, LanguageExpression operand1, LanguageExpression operand2) =>
            new FunctionExpression(name, new[] {operand1, operand2}, Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateJsonFunctionCall(JToken value) =>
            CreateUnaryFunction("json", new JTokenExpression(value.ToString(Formatting.None)));
    }
}
