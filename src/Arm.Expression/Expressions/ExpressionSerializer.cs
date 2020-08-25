using Arm.Expression.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;

namespace Arm.Expression.Expressions
{
    /// <summary>
    /// Serializes language expressions into strings.
    /// </summary>
    public class ExpressionSerializer
    {
        /// <summary>
        /// The serialization settings.
        /// </summary>
        private readonly ExpressionSerializerSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionSerializer"/> class.
        /// </summary>
        /// <param name="settings">The optional serializer settings</param>
        public ExpressionSerializer(ExpressionSerializerSettings settings = null)
        {
            // Note(majastrz): Use default settings if not specified.
            this.settings = settings ?? new ExpressionSerializerSettings();
        }

        /// <summary>
        /// Serializes a language expression into a string.
        /// </summary>
        /// <param name="expression">The expression</param>
        public string SerializeExpression(LanguageExpression expression)
        {
            var buffer = new StringBuilder();

            if (!ExpressionSerializer.TryWriteSingleStringToBuffer(buffer: buffer, expression: expression, settings: this.settings))
            {
                // Note(majastrz): The expression is not a single string expression or the single string serialization is disabled

                // Note(majastrz): Include outer brackets when serializing the expression if feature is enabled
                ExpressionSerializer.WriteExpressionToBuffer(
                    buffer: buffer,
                    expression: expression,
                    prefix: this.settings.IncludeOuterSquareBrackets ? "[" : null,
                    suffix: this.settings.IncludeOuterSquareBrackets ? "]" : null);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// If the expression is a single string JTokenExpression and serialization as string is enabled, it performs the serialization.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="expression">The expression</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if expression has been written out to the buffer or false otherwise.</returns>
        private static bool TryWriteSingleStringToBuffer(StringBuilder buffer, LanguageExpression expression, ExpressionSerializerSettings settings)
        {
            if (settings.SingleStringHandling == ExpressionSerializerSingleStringHandling.SerializeAsString && expression is JTokenExpression valueExpression)
            {
                var value = ExpressionSerializer.GetValueFromValueExpression(valueExpression: valueExpression);
                if (value.Type == JTokenType.String)
                {
                    var valueStr = value.ToString();

                    // Note(majastrz): Add escape bracket if needed.
                    if (valueStr.Length > 0 && valueStr[0] == '[')
                    {
                        buffer.Append(value: '[');
                    }

                    buffer.Append(value: valueStr);

                    return true;
                }
            }

            // Note(majastrz): Returning false REQUIRES that buffer not be modified in any way.
            return false;
        }

        /// <summary>
        /// Writes the serialized expression to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="expression">The expression</param>
        /// <param name="prefix">The optional prefix</param>
        /// <param name="suffix">The optional suffix</param>
        private static void WriteExpressionToBuffer(StringBuilder buffer, LanguageExpression expression, string prefix = null, string suffix = null)
        {
            ExpressionSerializer.ValidateExpression(expression: expression);

            if (prefix != null)
            {
                buffer.Append(value: prefix);
            }

            switch (expression)
            {
                case FunctionExpression functionExpression:
                    ExpressionSerializer.WriteFunctionExpressionToBuffer(buffer: buffer, functionExpression: functionExpression);
                    break;

                case JTokenExpression valueExpression:
                    ExpressionSerializer.WriteValueExpressionToBuffer(buffer: buffer, valueExpression: valueExpression);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected expression type '{expression.GetType().Name}'");
            }

            if (suffix != null)
            {
                buffer.Append(value: suffix);
            }
        }

        /// <summary>
        /// Writes the function expression to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="functionExpression">The function expression</param>
        private static void WriteFunctionExpressionToBuffer(StringBuilder buffer, FunctionExpression functionExpression)
        {
            ExpressionSerializer.ValidateFunctionExpression(functionExpression: functionExpression);

            // Note(majastrz): Add function name
            buffer.Append(value: functionExpression.Function);

            // Note(majastrz): Opening paren
            buffer.Append(value: '(');

            // Note(majastrz): Add parameter expressions and delimit with comma
            const string ParameterDelimiter = ", ";
            foreach (var parameterExpression in functionExpression.Parameters)
            {
                ExpressionSerializer.WriteExpressionToBuffer(buffer: buffer, expression: parameterExpression, suffix: ParameterDelimiter);
            }

            // Note(majastrz): Remove last param delimiter
            if (functionExpression.Parameters.Any())
            {
                buffer.Remove(startIndex: buffer.Length - ParameterDelimiter.Length, length: ParameterDelimiter.Length);
            }

            // Note(majastrz): Closing paren
            buffer.Append(value: ')');

            // Note(majastrz): Add the property expressions with correct enclosing characters
            foreach (var propertyExpression in functionExpression.Properties)
            {
                ExpressionSerializer.WritePropertyExpressionToBuffer(buffer: buffer, propertyExpression: propertyExpression);
            }
        }

        /// <summary>
        /// Writes the specified property expression to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="propertyExpression">The property expression</param>
        private static void WritePropertyExpressionToBuffer(StringBuilder buffer, LanguageExpression propertyExpression)
        {
            ExpressionSerializer.ValidateExpression(expression: propertyExpression);

            switch (propertyExpression)
            {
                case FunctionExpression functionExpression:
                    // Note(majastrz): Functions in properties should be enclosed in brackets
                    ExpressionSerializer.WriteExpressionToBuffer(buffer: buffer, expression: functionExpression, prefix: "[", suffix: "]");

                    return;

                case JTokenExpression valueExpression:
                    var value = ExpressionSerializer.GetValueFromValueExpression(valueExpression: valueExpression);

                    switch (value.Type)
                    {
                        case JTokenType.String:
                            var valueStr = value.ToString();

                            if (ExpressionSerializer.IsIdentifier(value: valueStr))
                            {
                                // Note(majastrz): The property expression is an identifier. We can serialize it with a leading dot and without any enclosing quotes.
                                buffer.Append(value: '.');
                                buffer.Append(value: valueStr);
                            }
                            else
                            {
                                // Note(majastrz): The property expression has to be enclosed in brackets because it's not an identifier.
                                ExpressionSerializer.WriteExpressionToBuffer(buffer: buffer, expression: valueExpression, prefix: "[", suffix: "]");
                            }

                            return;

                        case JTokenType.Integer:
                            // Note(majastrz): Indexes should be enclosed in brackets.
                            buffer.Append(value: '[');
                            buffer.Append(value: value);
                            buffer.Append(value: ']');

                            return;

                        default:
                            // Note(majastrz): JTokenValue can only be created with string or int value.
                            throw new InvalidOperationException(message: $"JTokenExpression has a value of unexpected type '{value.Type}'.");
                    }

                default:
                    throw new InvalidOperationException($"Unexpected expression type '{propertyExpression.GetType().Name}'");
            }
        }

        /// <summary>
        /// Writes the value expression to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="valueExpression">The value expression</param>
        private static void WriteValueExpressionToBuffer(StringBuilder buffer, JTokenExpression valueExpression)
        {
            ExpressionSerializer.ValidateValueExpression(valueExpression: valueExpression);

            var value = ExpressionSerializer.GetValueFromValueExpression(valueExpression: valueExpression);

            switch (value.Type)
            {
                case JTokenType.Integer:
                    // Note(majastrz): Integers are serialized as-is
                    buffer.Append(value: value);

                    return;

                case JTokenType.String:
                    WriteEscapedStringLiteral(buffer, value.ToString());

                    return;

                default:
                    // Note(majastrz): JTokenValue can only be created with string or int value.
                    throw new InvalidOperationException($"JTokenExpression has a value of unexpected type '{value.Type}'.");
            }
        }

        private static void WriteEscapedStringLiteral(StringBuilder buffer, string value)
        {
            // Note(majastrz): Strings are serialized enclosed in single quotes. Double single quote character is used to escape a single quote in the string.
            buffer.Append(value: '\'');
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == '\'')
                {
                    buffer.Append('\'');
                }
                buffer.Append(value[i]);
            }
            buffer.Append(value: '\'');
        }

        /// <summary>
        /// Gets a value from the value expression.
        /// </summary>
        /// <param name="valueExpression">The value expression</param>
        private static JToken GetValueFromValueExpression(JTokenExpression valueExpression)
        {
            // Note(majastrz): EvaluateExpression on JTokenExpression just returns the value and does not use the context at all.
            // The constructor of JTokenExpression does not allow you to create one with a null value, so we don't really need to check
            return valueExpression.Value;
        }

        /// <summary>
        /// Determines if the specified string is an identifier.
        /// </summary>
        /// <param name="value">The string to check</param>
        private static bool IsIdentifier(string value)
        {
            return !string.IsNullOrEmpty(value: value) && value.All(IsSupportedIdentifierCharacter);
        }

        /// <summary>
        /// Determines whether character is a supported identifier character.
        /// </summary>
        /// <param name="character">Input character.</param>        
        public static bool IsSupportedIdentifierCharacter(char character)
        {
            return char.IsLetterOrDigit(character) || character == '$' || character == '_';
        }

        /// <summary>
        /// Validates that the specified function expression. Does not perform recursive validation.
        /// </summary>
        /// <param name="functionExpression">The function expression</param>
        private static void ValidateFunctionExpression(FunctionExpression functionExpression)
        {
            ExpressionSerializer.ValidateExpression(expression: functionExpression);
            ExpressionSerializer.ValidateExpressionArray(functionExpression.Parameters);
            ExpressionSerializer.ValidateExpressionArray(functionExpression.Properties);
        }

        /// <summary>
        /// Validates the specified value expression. Does not perform recursive validation.
        /// </summary>
        /// <param name="valueExpression">The value expression</param>
        private static void ValidateValueExpression(JTokenExpression valueExpression)
        {
            ExpressionSerializer.ValidateExpression(expression: valueExpression);
        }

        /// <summary>
        /// Validates that the specified expression is not null. Does not perform recursive validation.
        /// </summary>
        /// <param name="expression">The expression</param>
        private static void ValidateExpression(LanguageExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
        }

        /// <summary>
        /// Validates the specified array of property expressions. Does not perform recursive validation.
        /// </summary>
        /// <param name="expressions">The expressions array</param>
        private static void ValidateExpressionArray(LanguageExpression[] expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }
        }
    }
}
