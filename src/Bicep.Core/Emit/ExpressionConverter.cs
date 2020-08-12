using System;
using System.IO;
using System.Linq;
using System.Text;
using Azure.ResourceManager.Deployments.Expression.Expressions;
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
        public static LanguageExpression ToTemplateExpression(this SyntaxBase expression)
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
                    return ConvertString(stringSyntax);
                    
                case NullLiteralSyntax _:
                    return CreateJsonFunctionCall(JValue.CreateNull());

                case ObjectSyntax _:
                case ArraySyntax _:
                    return ConvertComplexLiteral(expression);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return parenthesized.Expression.ToTemplateExpression();

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return new FunctionExpression(
                        "if",
                        new[]
                        {
                            ternary.ConditionExpression.ToTemplateExpression(),
                            ternary.TrueExpression.ToTemplateExpression(),
                            ternary.FalseExpression.ToTemplateExpression()
                        },
                        Array.Empty<LanguageExpression>());

                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.FunctionName.IdentifierName,
                        function.Arguments.Select(a => a.Expression.ToTemplateExpression()).ToArray());

                case ArrayAccessSyntax arrayAccess:
                    return AppendProperty(
                        arrayAccess.BaseExpression.ToFunctionExpression(),
                        arrayAccess.IndexExpression.ToTemplateExpression());

                case PropertyAccessSyntax propertyAccess:
                    return AppendProperty(
                        propertyAccess.BaseExpression.ToFunctionExpression(),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName));

                case VariableAccessSyntax _:
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private static LanguageExpression ConvertString(StringSyntax syntax)
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
                formatArgs[i + 1] = syntax.Expressions[i].ToTemplateExpression();
            }

            return new FunctionExpression("format", formatArgs, Array.Empty<LanguageExpression>());
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
        public static FunctionExpression ToFunctionExpression(this SyntaxBase expression)
        {
            var converted = expression.ToTemplateExpression();
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

        private static FunctionExpression ConvertComplexLiteral(SyntaxBase syntax)
        {
            // the tree node here should not contain any expressions inside
            // if it does, the emitted json expressions will not evaluate as expected due to IL limitations
            // there is a check elsewhere that will generate an error before we get this far in those cases
            var buffer = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(buffer)) {Formatting = Formatting.None})
            {
                ExpressionEmitter.EmitExpression(writer, syntax);
            }

            return CreateJsonFunctionCall(JToken.Parse(buffer.ToString()));
        }

        private static LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = syntax.LeftExpression.ToTemplateExpression();
            LanguageExpression operand2 = syntax.RightExpression.ToTemplateExpression();

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

        private static LanguageExpression ConvertUnary(UnaryOperationSyntax syntax)
        {
            LanguageExpression convertedOperand = syntax.Expression.ToTemplateExpression();

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
