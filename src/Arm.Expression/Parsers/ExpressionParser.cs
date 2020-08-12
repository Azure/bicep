// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Parsers
{
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Azure.ResourceManager.Deployments.Core.Extensions;
    using Azure.ResourceManager.Deployments.Core.Instrumentation.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Exceptions;
    using Azure.ResourceManager.Deployments.Expression.Expressions;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The language expression parser.
    /// </summary>
    internal static class ExpressionParser
    {
        #region Public methods

        /// <summary>
        /// Determines whether specified value is language expression.
        /// </summary>
        /// <param name="expression">The language expression.</param>
        public static bool IsLanguageExpression(string expression)
        {
            return !string.IsNullOrEmpty(expression) && expression.First() == '[' && expression.Last() == ']';
        }

        /// <summary>
        /// Parses the language expression.
        /// </summary>
        /// <param name="expression">The language expression.</param>
        /// <param name="additionalInfo">The additional Info of the expression.</param>
        public static LanguageExpression ParseLanguageExpression(string expression, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (!ExpressionParser.IsLanguageExpression(expression))
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.TemplateNotLanguageExpression.ToLocalizedMessage(expression),
                    additionalInfo: additionalInfo);
            }

            if (expression[1] == '[')
            {
                return new JTokenExpression(value: expression.Substring(1), additionalInfo: additionalInfo);
            }

            // Note(ilygre): We need to remove first and last characters from expression string, because
            // this characters is only used to disambiguate between pure JSON string and language expression.
            return ExpressionParser.ParseExpression(expression.Substring(1, expression.Length - 2), additionalInfo);
        }

        /// <summary>
        /// Checks if the specified function name is valid according to grammar.
        /// </summary>
        /// <param name="value">The function name to validate.</param>
        /// <param name="additionalInfo">The additional Info of the value.</param>
        public static bool IsFunctionName(string value, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                var scanner = new ExpressionScanner(expression: value, additionalInfo: additionalInfo);
                ExpressionParser.ParseFunctionName(scanner: scanner, expectedTokenType: ExpressionTokenType.EndOfData);

                // Note(majastrz): Since the parse function didn't throw anything, we can assume the function is valid.
                return true;
            }
            catch (ExpressionException)
            {
                // Note(majastrz): This is not a valid function name
                return false;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="additionalInfo">The additional Info of the expression.</param>
        private static LanguageExpression ParseExpression(string expression, TemplateErrorAdditionalInfo additionalInfo)
        {
            var scanner = new ExpressionScanner(expression, additionalInfo);

            var parsedExpression = ExpressionParser.ParseExpressionRecursive(scanner);
            scanner.ExpectToken(ExpressionTokenType.EndOfData);

            return parsedExpression;
        }

        /// <summary>
        /// Parses the expression recursive.
        /// </summary>
        /// <param name="scanner">The expression scanner.</param>
        private static LanguageExpression ParseExpressionRecursive(ExpressionScanner scanner)
        {
            ExpressionToken token;
            if ((token = scanner.AcceptToken(ExpressionTokenType.Literal)) != null)
            {
                return new JTokenExpression((string)token.Value, scanner.AdditionalInfo);
            }
            else if ((token = scanner.AcceptToken(ExpressionTokenType.Integer)) != null)
            {
                return new JTokenExpression((int)token.Value);
            }
            else
            {
                return ExpressionParser.ParseFunctionExpression(scanner);
            }
        }

        /// <summary>
        /// Parses the function name.
        /// </summary>
        /// <param name="scanner">The expression scanner.</param>
        /// <param name="expectedTokenType">The token type we expect at the end of successful parsing of the function name.</param>
        private static string ParseFunctionName(ExpressionScanner scanner, ExpressionTokenType expectedTokenType)
        {
            var function = (string)scanner.ExpectToken(type: ExpressionTokenType.Identifier).Value;

            if (scanner.AcceptToken(type: ExpressionTokenType.Dot) != null)
            {
                function = string.Concat(str0: function, str1: ".", str2: (string)scanner.ExpectToken(type: ExpressionTokenType.Identifier).Value);
            }

            scanner.ExpectToken(type: expectedTokenType);

            return function;
        }

        /// <summary>
        /// Parses the function expression.
        /// </summary>
        /// <param name="scanner">The expression scanner.</param>
        private static FunctionExpression ParseFunctionExpression(ExpressionScanner scanner)
        {
            // Note(majastrz): When we're done parsing the function name successfully, the token should be a left paren
            var function = ExpressionParser.ParseFunctionName(scanner: scanner, expectedTokenType: ExpressionTokenType.LeftParenthesis);

            var parameters = new List<LanguageExpression>();
            if (scanner.AcceptToken(ExpressionTokenType.RightParenthesis) == null)
            {
                do
                {
                    parameters.Add(ExpressionParser.ParseExpressionRecursive(scanner));
                }
                while (scanner.AcceptToken(ExpressionTokenType.Comma) != null);

                scanner.ExpectToken(ExpressionTokenType.RightParenthesis);
            }

            var properties = new List<LanguageExpression>();
            while (true)
            {
                if (scanner.AcceptToken(ExpressionTokenType.Dot) != null)
                {
                    properties.Add(new JTokenExpression((string)scanner.ExpectToken(ExpressionTokenType.Identifier).Value, scanner.AdditionalInfo));
                    continue;
                }

                if (scanner.AcceptToken(ExpressionTokenType.LeftSquareBracket) != null)
                {
                    properties.Add(ExpressionParser.ParseExpressionRecursive(scanner));
                    scanner.ExpectToken(ExpressionTokenType.RightSquareBracket);
                    continue;
                }

                break;
            }

            var functionExpression = new FunctionExpression(function, parameters.ToArray(), properties.ToArray());
            functionExpression.Parameters.ForEach(parameter => parameter.Parent = functionExpression);
            functionExpression.Properties.ForEach(property => property.Parent = functionExpression);
            return functionExpression;
        }

        #endregion
    }
}
