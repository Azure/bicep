// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Parsers
{
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Azure.ResourceManager.Deployments.Core.Instrumentation.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Configuration;
    using Azure.ResourceManager.Deployments.Expression.Exceptions;
    using Azure.ResourceManager.Deployments.Expression.Extensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The language expression scanner.
    /// </summary>
    public class ExpressionScanner
    {
        #region Constructor

        /// <summary>
        /// Gets the current expression token.
        /// </summary>
        private ExpressionToken CurrentToken
        {
            get { return this.HasToken ? this.Enumerator.Current : ExpressionToken.EndOfData; }
        }

        /// <summary>
        /// Gets or sets the language expression.
        /// </summary>
        private string Expression { get; set; }

        /// <summary>
        /// Gets or sets the tokens enumerator.
        /// </summary>
        private IEnumerator<ExpressionToken> Enumerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current expression token has value.
        /// </summary>
        private bool HasToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating The additional Info of the expression.
        /// </summary>
        public TemplateErrorAdditionalInfo AdditionalInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionScanner" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="additionalInfo">The additional Info of the expression.</param>
        public ExpressionScanner(string expression, TemplateErrorAdditionalInfo additionalInfo)
        {
            this.Expression = expression;
            this.Enumerator = ExpressionScanner.ParseExpression(this.Expression, additionalInfo).GetEnumerator();
            this.MoveNext();
            this.AdditionalInfo = additionalInfo;
        }

        /// <summary>
        /// Moves to the next expression token.
        /// </summary>
        private void MoveNext()
        {
            this.HasToken = this.Enumerator.MoveNext();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Accepts the specified expression token type.
        /// </summary>
        /// <param name="type">The expression token type.</param>
        public ExpressionToken AcceptToken(ExpressionTokenType type)
        {
            if (this.CurrentToken.Type == type)
            {
                var token = this.CurrentToken;
                this.MoveNext();
                return token;
            }

            return null;
        }

        /// <summary>
        /// Expects the specified expression token type.
        /// </summary>
        /// <param name="type">The expression token type.</param>
        public ExpressionToken ExpectToken(ExpressionTokenType type)
        {
            if (this.CurrentToken.Type == type)
            {
                var token = this.CurrentToken;
                this.MoveNext();
                return token;
            }

            throw new ExpressionException(
                message: ErrorResponseMessages.UnexpectedExpressionToken.ToLocalizedMessage(this.Expression, type, this.CurrentToken.Type),
                additionalInfo: this.AdditionalInfo);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Parses the language expression.
        /// </summary>
        /// <param name="expression">The language expression.</param>
        /// <param name="additionalInfo">The additional info of the expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Validation code for throttling is complex by design.")]
        public static IEnumerable<ExpressionToken> ParseExpression(string expression, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            if (expression.Length > ExpressionConstants.ExpressionLimit)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.ExpressionLimitExceeded.ToLocalizedMessage(
                       ExpressionConstants.ExpressionLimit,
                       expression.Length),
                    additionalInfo: additionalInfo);
            }

            for (var currentPosition = 0; currentPosition < expression.Length; ++currentPosition)
            {
                if (char.IsWhiteSpace(expression[currentPosition]))
                {
                    continue;
                }

                var startPosition = currentPosition;
                switch (expression[currentPosition])
                {
                    case '.':
                        yield return new ExpressionToken { Type = ExpressionTokenType.Dot, Value = "." };
                        break;

                    case ',':
                        yield return new ExpressionToken { Type = ExpressionTokenType.Comma, Value = ", " };
                        break;

                    case '(':
                        yield return new ExpressionToken { Type = ExpressionTokenType.LeftParenthesis, Value = "(" };
                        break;

                    case ')':
                        yield return new ExpressionToken { Type = ExpressionTokenType.RightParenthesis, Value = ")" };
                        break;

                    case '[':
                        yield return new ExpressionToken { Type = ExpressionTokenType.LeftSquareBracket, Value = "[" };
                        break;

                    case ']':
                        yield return new ExpressionToken { Type = ExpressionTokenType.RightSquareBracket, Value = "]" };
                        break;

                    case '\'':
                        while (currentPosition < expression.Length)
                        {
                            currentPosition = ExpressionScanner.ScanForward(
                                expression: expression,
                                startPosition: currentPosition + 1,
                                predicate: character => character != '\'');

                            // NOTE(jogao): skip the 2nd \' for next ScanForward
                            if (currentPosition + 1 < expression.Length && expression[currentPosition + 1] == '\'')
                            {
                                currentPosition++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (currentPosition >= expression.Length)
                        {
                            throw new ExpressionException(
                                message: ErrorResponseMessages.ExpressionUnterminatedString.ToLocalizedMessage(expression, startPosition),
                                additionalInfo: additionalInfo);
                        }

                        yield return new ExpressionToken
                        {
                            Type = ExpressionTokenType.Literal,
                            Value = ExpressionExtensions.UnescapeStringLiteral(expression.Substring(startPosition + 1, currentPosition - startPosition - 1))
                        };

                        break;

                    default:
                        if (expression[currentPosition] == '+' || expression[currentPosition] == '-' || char.IsDigit(expression[currentPosition]))
                        {
                            currentPosition = expression[currentPosition] == '+' || expression[currentPosition] == '-'
                                ? currentPosition + 1
                                : currentPosition;

                            var tokenType = ExpressionTokenType.Integer;
                            currentPosition = ExpressionScanner.ScanForward(
                                expression: expression,
                                startPosition: currentPosition,
                                predicate: char.IsDigit);

                            if (currentPosition < expression.Length && expression[currentPosition] == '.')
                            {
                                tokenType = ExpressionTokenType.Float;
                                currentPosition = ExpressionScanner.ScanForward(
                                    expression: expression,
                                    startPosition: currentPosition + 1,
                                    predicate: char.IsDigit);
                            }

                            if (currentPosition + 1 < expression.Length && (expression[currentPosition] == 'e' || expression[currentPosition] == 'E'))
                            {
                                currentPosition = expression[currentPosition + 1] == '+' || expression[currentPosition + 1] == '-'
                                    ? currentPosition + 2
                                    : currentPosition + 1;

                                tokenType = ExpressionTokenType.Float;
                                currentPosition = ExpressionScanner.ScanForward(
                                    expression: expression,
                                    startPosition: currentPosition,
                                    predicate: char.IsDigit);
                            }

                            if (currentPosition < expression.Length && ExpressionScanner.IsSupportedIdentifierCharacter(expression[currentPosition]))
                            {
                                throw new ExpressionException(
                                    message: ErrorResponseMessages.ExpressionUnexpectedCharacter.ToLocalizedMessage(expression, expression[currentPosition], currentPosition),
                                    additionalInfo: additionalInfo);
                            }

                            currentPosition--;

                            var number = expression.Substring(
                                startIndex: startPosition,
                                length: currentPosition - startPosition + 1);

                            if (tokenType == ExpressionTokenType.Integer)
                            {
                                int value;
                                if (!int.TryParse(number, out value))
                                {
                                    throw new ExpressionException(
                                       message: ErrorResponseMessages.ExpressionInvalidNumber.ToLocalizedMessage(expression, number, startPosition),
                                       additionalInfo: additionalInfo);
                                }

                                yield return new ExpressionToken
                                {
                                    Type = ExpressionTokenType.Integer,
                                    Value = value
                                };
                            }
                            else
                            {
                                double value;
                                if (!double.TryParse(number, out value))
                                {
                                    throw new ExpressionException(
                                       message: ErrorResponseMessages.ExpressionInvalidNumber.ToLocalizedMessage(expression, number, startPosition),
                                       additionalInfo: additionalInfo);
                                }

                                throw new ExpressionException(
                                    message: ErrorResponseMessages.ExpressionUnsupportedFloat.ToLocalizedMessage(expression, startPosition),
                                    additionalInfo: additionalInfo);
                            }
                        }
                        else if (ExpressionScanner.IsSupportedIdentifierCharacter(expression[currentPosition]))
                        {
                            currentPosition = ExpressionScanner.ScanForward(
                                expression: expression,
                                startPosition: currentPosition,
                                predicate: ExpressionScanner.IsSupportedIdentifierCharacter);

                            currentPosition--;

                            var identifier = expression.Substring(
                                startIndex: startPosition,
                                length: currentPosition - startPosition + 1);

                            yield return new ExpressionToken
                            {
                                Type = ExpressionTokenType.Identifier,
                                Value = identifier
                            };
                        }
                        else
                        {
                            throw new ExpressionException(
                                message: ErrorResponseMessages.ExpressionUnexpectedCharacter.ToLocalizedMessage(expression, expression[currentPosition], currentPosition),
                                additionalInfo: additionalInfo);
                        }

                        break;
                }
            }
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
        /// Scans the expression forward while predicate is evaluated as true.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="predicate">The predicate.</param>
        private static int ScanForward(string expression, int startPosition, Func<char, bool> predicate)
        {
            while (startPosition < expression.Length && predicate(expression[startPosition]))
            {
                startPosition++;
            }

            return startPosition;
        }

        #endregion
    }
}
