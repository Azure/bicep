// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Deployments.Expression.Expressions;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Decompiler
{
    public class TemplateConverter
    {
        private static readonly IExpressionsProvider ExpressionsProvider = new ArmExpressionsProvider();

        private readonly INamingResolver nameResolver;
        private readonly string filePath;
        private readonly JObject template;

        private static LanguageExpression GetLanguageExpression(string value)
        {
            if (ExpressionsProvider.IsLanguageExpression(value))
            {
                return ExpressionsProvider.ParseLanguageExpression(value);
            }

            return new JTokenExpression(value);
        }

        private TemplateConverter(string filePath, string content)
        {
            this.filePath = filePath;
            this.template = JsonConvert.DeserializeObject<JObject>(content);
            this.nameResolver = new UniqueNamingResolver(ExpressionsProvider);
        }

        public static ProgramSyntax DecompileTemplate(string filePath, string content)
        {
            var instance = new TemplateConverter(filePath, content);

            return instance.Parse();
        }

        private void RegisterNames(IEnumerable<JProperty> parameters, IEnumerable<JToken> resources, IEnumerable<JProperty> variables, IEnumerable<JProperty> outputs)
        {
            foreach (var parameter in parameters)
            {
                if (nameResolver.TryRequestName(NameType.Parameter, parameter.Name) == null)
                {
                    throw new InvalidOperationException($"Unable to pick unique name for parameter {parameter.Name}");
                }
            }

            foreach (var output in outputs)
            {
                if (nameResolver.TryRequestName(NameType.Output, output.Name) == null)
                {
                    throw new InvalidOperationException($"Unable to pick unique name for output {output.Name}");
                }
            }

            foreach (var variable in variables)
            {
                if (nameResolver.TryRequestName(NameType.Variable, variable.Name) == null)
                {
                    throw new InvalidOperationException($"Unable to pick unique name for variable {variable.Name}");
                }
            }

            foreach (var resource in resources)
            {
                var nameString = resource["name"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'name' for resource '{resource["name"]}'");
                var typeString = resource["type"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'type' for resource '{resource["name"]}'");

                if (nameResolver.TryRequestResourceName(typeString, GetLanguageExpression(nameString)) == null)
                {
                    throw new InvalidOperationException($"Unable to pick unique name for resource {typeString} {nameString}");
                }
            }
        }

        private static IEnumerable<LanguageExpression> CombineConcatArguments(IEnumerable<LanguageExpression> arguments)
        {
            var current = "";
            foreach (var argument in arguments)
            {
                var stringVal = (argument as JTokenExpression)?.Value.Value<string>();
                if (stringVal != null)
                {
                    current += stringVal;

                    // don't return the string yet - there may be another string to join
                    continue;
                }

                if (current.Length > 0)
                {
                    yield return new JTokenExpression(current);
                    current = "";
                }

                yield return argument;
            }

            if (current.Length > 0)
            {
                yield return new JTokenExpression(current);
            }
        }

        private static LanguageExpression FlattenConcats(LanguageExpression original)
        {
            if (!(original is FunctionExpression functionExpression))
            {
                return original;
            }

            // convert a 'format' to a 'concat' function
            if (functionExpression.NameEquals("format"))
            {
                var formatString = (functionExpression.Parameters[0] as JTokenExpression)?.Value.Value<string>() ?? throw new ArgumentException($"Unable to read format statement {ExpressionsProvider.SerializeExpression(functionExpression)} as string");
                var formatHoleMatches = Regex.Matches(formatString, "(?<={){([0-9]+)}");

                var concatExpressions = new List<LanguageExpression>();
                var nextStart = 0;
                for (var i = 0; i < formatHoleMatches.Count; i++)
                {
                    var match = formatHoleMatches[i];
                    var position = match.Groups[2].Index;
                    var length = match.Groups[2].Length;
                    var intValue = int.Parse(match.Groups[2].Value);

                    // compensate for the {
                    if (nextStart < position - 1)
                    {
                        var betweenPortion = formatString.Substring(nextStart, position - 1 - nextStart);
                        concatExpressions.Add(new JTokenExpression(betweenPortion));
                    }

                    // replace it with the appropriately-numbered expression
                    concatExpressions.Add(functionExpression.Parameters[intValue + 1]);

                    // compensate for the }
                    nextStart = position + length + 1;
                }

                if (nextStart < formatString.Length)
                {
                    var betweenPortion = formatString.Substring(nextStart, formatString.Length - nextStart);
                    concatExpressions.Add(new JTokenExpression(betweenPortion));
                }

                // overwrite the original expression
                functionExpression = new FunctionExpression("concat", concatExpressions.ToArray(), Array.Empty<LanguageExpression>());
            }

            // flatten nested 'concat' functions
            if (functionExpression.NameEquals("concat"))
            {
                var concatExpressions = new List<LanguageExpression>();
                foreach (var parameter in functionExpression.Parameters)
                {
                    // recurse
                    var flattenedParameter = FlattenConcats(parameter);
                    
                    if (parameter is FunctionExpression childFunction && childFunction.NameEquals("concat"))
                    {
                        // concat directly inside a concat - break it out
                        concatExpressions.AddRange(childFunction.Parameters);
                        continue;
                    }

                    concatExpressions.Add(parameter);
                }

                // overwrite the original expression
                functionExpression = new FunctionExpression("concat", CombineConcatArguments(concatExpressions).ToArray(), Array.Empty<LanguageExpression>());
            }

            // just return the inner portion if there's only one concat entry
            if (functionExpression.NameEquals("concat") && functionExpression.Parameters.Length == 1)
            {
                return functionExpression.Parameters[0];
            }

            return functionExpression;
        }

        private static LanguageExpression InlineVariablesRecursive(JObject template, LanguageExpression original)
        {
            if (!(original is FunctionExpression functionExpression))
            {
                return original;
            }

            if (functionExpression.Function == "variables" && functionExpression.Parameters.Length == 1 && functionExpression.Parameters[0] is JTokenExpression variableNameExpression)
            {
                var variableVal = template["variables"]?[variableNameExpression.Value.ToString()];

                if (variableVal == null)
                {
                    throw new ArgumentException($"Unable to resolve variable {variableNameExpression.Value.ToString()}");
                }

                if (variableVal.Type == JTokenType.String && variableVal.ToObject<string>() is string stringValue)
                {
                    var variableExpression = GetLanguageExpression(stringValue);

                    return InlineVariablesRecursive(template, variableExpression);
                }
            }

            var inlinedParameters = functionExpression.Parameters.Select(p => InlineVariablesRecursive(template, p));

            return new FunctionExpression(
                functionExpression.Function,
                inlinedParameters.ToArray(),
                functionExpression.Properties);
        }

        private static LanguageExpression FlattenConcatsAndInlineVariables(JObject template, LanguageExpression original)
        {
            var inlined = InlineVariablesRecursive(template, original);

            return FlattenConcats(inlined);
        }

        private static IEnumerable<JToken> FlattenAndNormalizeResource(JToken resource)
        {
            var parentType = resource["type"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'type' for resource '{resource["name"]}'");
            var parentName = resource["name"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'name' for resource '{resource["name"]}'");

            // normalize the name
            var parentNameExpression = FlattenConcats(GetLanguageExpression(parentName));
            resource["name"] = ExpressionsProvider.SerializeExpression(parentNameExpression);

            yield return resource;

            var childResources = resource["resources"] as JArray;
            if (childResources == null)
            {
                yield break;
            }

            resource["resources"] = null;
            foreach (var childResource in childResources)
            {
                var childType = childResource["type"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'type' for resource '{childResource["name"]}'");
                var childName = childResource["name"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'name' for resource '{childResource["name"]}'");

                // child may sometimes be specified using the fully-qualified type and name
                if (!StringComparer.OrdinalIgnoreCase.Equals(parentType.Split("/")[0], childType.Split("/")[0]))
                {
                    childResource["type"] = $"{parentType}/{childType}";
                    childResource["name"] = ExpressionsProvider.SerializeExpression(new FunctionExpression("concat", new LanguageExpression[] { parentNameExpression, new JTokenExpression("/"), GetLanguageExpression(childName)}, Array.Empty<LanguageExpression>()));
                }

                foreach (var result in FlattenAndNormalizeResource(childResource))
                {
                    // recurse
                    yield return result;
                }
            }
        }

        private static LanguageExpression FormatNameExpression(IEnumerable<LanguageExpression> nameSegments)
        {
            var pathSegments = nameSegments
                .SelectMany((expression, i) => i == 0 ? new [] { expression } : new [] { new JTokenExpression("/"), expression });
            var nameExpression = new FunctionExpression("concat", pathSegments.ToArray(), Array.Empty<LanguageExpression>());

            return FlattenConcats(nameExpression);
        }

        private static TypeSyntax? TryParseType(JToken? value)
        {
            var typeString = value?.Value<string>();
            if (typeString == null)
            {
                return null;
            }

            return new TypeSyntax(Helpers.CreateToken(TokenType.Identifier, typeString.ToLowerInvariant()));
        }

        private string? TryLookupResource(LanguageExpression expression)
        {
            if (!(expression is FunctionExpression functionExpression))
            {
                return null;
            }

            if (functionExpression.NameEquals("concat") &&
                functionExpression.Parameters.Length == 2 &&
                functionExpression.Parameters[0] is JTokenExpression)
            {
                // this is a heuristic but appears to be quite - e.g. to format a resourceId using concat('My.Rp/type', parameters(name)) rather than resourceId().
                // It doesn't really hurt to see if we can find a match for it.
                functionExpression = new FunctionExpression(
                    "resourceid",
                    functionExpression.Parameters,
                    new LanguageExpression[] { });
            }
            
            // TODO: find resources with same name variable - this is pretty common

            if (!functionExpression.NameEquals("resourceid") ||
                functionExpression.Parameters.Length < 2)
            {
                return null;
            }

            var typeString = (functionExpression.Parameters[0] as JTokenExpression)?.Value.Value<string>();
            if (typeString == null)
            {
                return null;
            }

            var nameExpression = FormatNameExpression(functionExpression.Parameters.Skip(1));

            return nameResolver.TryLookupResourceName(typeString, FlattenConcats(nameExpression));
        }

        private SyntaxBase? TryParseJToken(JToken? value)
            => value is null ? null : ParseJToken(value);

        private SyntaxBase ParseJToken(JToken? value)
            => value switch {
                JObject jObject => ParseJObject(jObject),
                JArray jArray => ParseJArray(jArray),
                JValue jValue => ParseJValue(jValue),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
            };

        private SyntaxBase ParseJTokenExpression(JTokenExpression expression)
            => expression.Value.Type switch
            {
                JTokenType.String => Helpers.CreateStringLiteral(expression.Value.Value<string>()!),
                JTokenType.Integer => new NumericLiteralSyntax(Helpers.CreateToken(TokenType.Number, expression.Value.ToString()), expression.Value.Value<int>()),
                JTokenType.Boolean =>  expression.Value.Value<bool>() ?
                    new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.TrueKeyword, "true"), true) :
                    new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.FalseKeyword, "false"), false),
                JTokenType.Null => new NullLiteralSyntax(Helpers.CreateToken(TokenType.NullKeyword, "null")),
                _ => throw new NotImplementedException($"Unrecognized expression {ExpressionsProvider.SerializeExpression(expression)}"),
            };

        private bool TryReplaceBannedFunction(FunctionExpression expression, [NotNullWhen(true)] out SyntaxBase? syntax)
        {
            if (Helpers.BannedBinaryOperatorLookup.TryGetValue(expression.Function, out var bannedOperator))
            {
                if (expression.Parameters.Length != 2)
                {
                    throw new ArgumentException($"Expected 2 parameters for binary function {expression.Function}");
                }

                if (expression.Properties.Any())
                {
                    throw new ArgumentException($"Expected 0 properties for binary function {expression.Function}");
                }

                syntax = new ParenthesizedExpressionSyntax(
                    Helpers.CreateToken(TokenType.LeftParen, "("),
                    new BinaryOperationSyntax(
                        ParseLanguageExpression(expression.Parameters[0]),
                        bannedOperator,
                        ParseLanguageExpression(expression.Parameters[1])),
                    Helpers.CreateToken(TokenType.RightParen, ")"));
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(expression.Function, "not"))
            {
                if (expression.Parameters.Length != 1)
                {
                    throw new ArgumentException($"Expected 1 parameters for unary function {expression.Function}");
                }

                if (expression.Properties.Any())
                {
                    throw new ArgumentException($"Expected 0 properties for unary function {expression.Function}");
                }

                syntax = new ParenthesizedExpressionSyntax(
                    Helpers.CreateToken(TokenType.LeftParen, "("),
                    new UnaryOperationSyntax(
                        Helpers.CreateToken(TokenType.Exclamation, "!"),
                        ParseLanguageExpression(expression.Parameters[0])),
                    Helpers.CreateToken(TokenType.RightParen, ")"));
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(expression.Function, "if"))
            {
                if (expression.Parameters.Length != 3)
                {
                    throw new ArgumentException($"Expected 3 parameters for ternary function {expression.Function}");
                }

                if (expression.Properties.Any())
                {
                    throw new ArgumentException($"Expected 0 properties for ternary function {expression.Function}");
                }

                syntax = new ParenthesizedExpressionSyntax(
                    Helpers.CreateToken(TokenType.LeftParen, "("),
                    new TernaryOperationSyntax(
                        ParseLanguageExpression(expression.Parameters[0]),
                        Helpers.CreateToken(TokenType.Question, "?"),
                        ParseLanguageExpression(expression.Parameters[1]),
                        Helpers.CreateToken(TokenType.Colon, ":"),
                        ParseLanguageExpression(expression.Parameters[2])),
                    Helpers.CreateToken(TokenType.RightParen, ")"));
                return true;
            }

            syntax = null;
            return false;
        }

        private SyntaxBase ParseFunctionExpression(FunctionExpression expression)
        {
            SyntaxBase? baseSyntax = null;
            switch (expression.Function.ToLowerInvariant())
            {
                case "parameters":
                {
                    if (expression.Parameters.Length != 1 || !(expression.Parameters[0] is JTokenExpression jTokenExpression) || jTokenExpression.Value.Type != JTokenType.String)
                    {
                        throw new NotImplementedException($"Unrecognized expression {ExpressionsProvider.SerializeExpression(expression)}");
                    }

                    var stringVal = jTokenExpression.Value.Value<string>()!;
                    var resolved = nameResolver.TryLookupName(NameType.Parameter, stringVal) ?? throw new ArgumentException($"Unable to find parameter {stringVal}");
                    baseSyntax = Helpers.CreateIdentifier(resolved);
                    break;
                }
                case "variables":
                {
                    if (expression.Parameters.Length != 1 || !(expression.Parameters[0] is JTokenExpression jTokenExpression) || jTokenExpression.Value.Type != JTokenType.String)
                    {
                        throw new NotImplementedException($"Unrecognized expression {ExpressionsProvider.SerializeExpression(expression)}");
                    }

                    var stringVal = jTokenExpression.Value.Value<string>()!;
                    var resolved = nameResolver.TryLookupName(NameType.Variable, stringVal) ?? throw new ArgumentException($"Unable to find variable {stringVal}");
                    baseSyntax = Helpers.CreateIdentifier(resolved);
                    break;
                }
                case "reference":
                {
                    if (expression.Parameters.Length == 1 && expression.Parameters[0] is FunctionExpression resourceIdExpression && resourceIdExpression.NameEquals("resourceid"))
                    {
                        // resourceid directly inside a reference - check if it's a reference to a known resource
                        var resourceName = TryLookupResource(resourceIdExpression);
                        
                        if (resourceName != null)
                        {
                            baseSyntax = new PropertyAccessSyntax(
                                new VariableAccessSyntax(Helpers.CreateIdentifier(resourceName)),
                                Helpers.CreateToken(TokenType.Dot, "."),
                                Helpers.CreateIdentifier("properties"));
                        }
                    }
                    break;
                }
                case "resourceid":
                {
                    var resourceName = TryLookupResource(expression);

                    if (resourceName != null)
                    {
                        baseSyntax = new PropertyAccessSyntax(
                            Helpers.CreateIdentifier(resourceName),
                            Helpers.CreateToken(TokenType.Dot, "."),
                            Helpers.CreateIdentifier("id"));
                    }
                    break;
                }
                case "format":
                {
                    // this will remove 'format' which should avoid an infinite loop
                    return ParseLanguageExpression(FlattenConcats(expression));
                }
                case "concat":
                {
                    var canInterpolate = expression.Parameters.Any(p => p is JTokenExpression jTokenExpression && jTokenExpression.Value.Type == JTokenType.String);
                    if (!canInterpolate)
                    {
                        // we might be dealing with an array
                        break;
                    }

                    if (expression.Parameters.Length < 1)
                    {
                        // empty concat statement - let's just ignore that...
                        break;
                    }

                    var flattenedExpression = FlattenConcats(expression);
                    if (flattenedExpression is JTokenExpression flattenedJTokenExpression)
                    {
                        var stringVal = flattenedJTokenExpression.Value.Value<string>();
                        if (stringVal == null)
                        {
                            // let the outer block handle it
                            break;
                        }

                        return Helpers.CreateStringLiteral(stringVal);
                    }

                    if (!(flattenedExpression is FunctionExpression functionExpression))
                    {
                        throw new InvalidOperationException($"Expected {nameof(FunctionExpression)}");
                    }
                    expression = functionExpression;

                    var values = new List<string>();
                    var stringTokens = new List<Token>();
                    var expressions = new List<SyntaxBase>();
                    for (var i = 0; i < expression.Parameters.Length; i++)
                    {
                        // flattenconcats should already ensure we're not going to run into two
                        // string literals side-by-side, so assume either token:expression or expression

                        var isStart = (i == 0);
                        var isEnd = (i == expression.Parameters.Length - 1);

                        if (expression.Parameters[i] is JTokenExpression jTokenExpression)
                        {
                            stringTokens.Add(Helpers.CreateStringInterpolationToken(isStart, isEnd, jTokenExpression.Value.ToString()));

                            // if done, exit early
                            if (isEnd)
                            {
                                break;
                            }
                            // otherwise, process the expression in this iteration
                            i++;
                        }
                        else
                        {
                            //  we always need a token between expressions, even if it's empty
                            stringTokens.Add(Helpers.CreateStringInterpolationToken(isStart, false, ""));
                        }

                        expressions.Add(ParseLanguageExpression(expression.Parameters[i]));
                        isStart = (i == 0);
                        isEnd = (i == expression.Parameters.Length - 1);

                        if (isEnd)
                        {
                            // always make sure we end with a string token
                            stringTokens.Add(Helpers.CreateStringInterpolationToken(isStart, isEnd, ""));
                        }
                    }

                    var rawSegments = Lexer.TryGetRawStringSegments(stringTokens);
                    if (rawSegments != null)
                    {
                        baseSyntax = new StringSyntax(
                            stringTokens,
                            expressions,
                            rawSegments);
                    }
                    
                    break;
                }
                case "__bicep_reference":
                {
                    var resourceRef = (expression.Parameters[0] as JTokenExpression)?.Value.Value<string>() ?? throw new InvalidOperationException("Internal error");
                    baseSyntax = new VariableAccessSyntax(Helpers.CreateIdentifier(resourceRef));
                    break;
                }
                default:
                    if (TryReplaceBannedFunction(expression, out var replacedBannedSyntax))
                    {
                        baseSyntax = replacedBannedSyntax;
                    }
                    break;
            }

            if (baseSyntax == null)
            {
                // Try to correct the name - ARM JSON is case-insensitive, but Bicep is sensitive
                var functionName = Helpers.CorrectWellKnownFunctionCasing(expression.Function);

                var expressions = expression.Parameters.Select(ParseLanguageExpression).ToArray();

                baseSyntax = new FunctionCallSyntax(
                    Helpers.CreateIdentifier(functionName),
                    Helpers.CreateToken(TokenType.LeftParen, "("),
                    expressions.Select((x, i) => new FunctionArgumentSyntax(x, i < expressions.Length - 1 ? Helpers.CreateToken(TokenType.Comma, ",") : null)),
                    Helpers.CreateToken(TokenType.RightParen, ")"));
            }

            foreach (var property in expression.Properties)
            {
                if (property is JTokenExpression jTokenExpression && jTokenExpression.Value.Type == JTokenType.String)
                {
                    // TODO handle special chars and generate array access instead
                    baseSyntax = new PropertyAccessSyntax(
                        baseSyntax,
                        Helpers.CreateToken(TokenType.Dot, "."),
                        Helpers.CreateIdentifier(jTokenExpression.Value.Value<string>()!));
                }
                else
                {
                    baseSyntax = new ArrayAccessSyntax(
                        baseSyntax,
                        Helpers.CreateToken(TokenType.LeftSquare, "["),
                        ParseLanguageExpression(property),
                        Helpers.CreateToken(TokenType.RightSquare, "]"));
                }
            }

            return baseSyntax;
        }

        private SyntaxBase ParseLanguageExpression(LanguageExpression expression)
            => expression switch
            {
                JTokenExpression jTokenExpression => ParseJTokenExpression(jTokenExpression),
                FunctionExpression functionExpression => ParseFunctionExpression(functionExpression),
                _ => throw new NotImplementedException($"Unrecognized expression {ExpressionsProvider.SerializeExpression(expression)}"),
            };

        private SyntaxBase ParseString(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (ExpressionsProvider.IsLanguageExpression(value))
            {
                var expression = ExpressionsProvider.ParseLanguageExpression(value);

                return ParseLanguageExpression(expression);
            }
            
            return Helpers.CreateStringLiteral(value);
        }

        private static SyntaxBase ParseIntegerJToken(JValue value)
        {
            // JTokenType.Integer can encapsulate non-C#-int numbers.
            // Best we can do in this case is to convert them to strings.
            var longValue = value.Value<long>();
            if (longValue < int.MinValue || longValue > int.MaxValue)
            {
                return Helpers.CreateStringLiteral(value.ToString());
            }

            return new NumericLiteralSyntax(Helpers.CreateToken(TokenType.Number, value.ToString()), (int)longValue);
        }

        private SyntaxBase ParseJValue(JValue value)
            => value.Type switch {
                JTokenType.String => ParseString(value.ToString()),
                JTokenType.Integer => ParseIntegerJToken(value),
                JTokenType.Date => ParseString(value.ToString()),
                JTokenType.Float => ParseString(value.ToString()),
                JTokenType.Boolean =>  value.Value<bool>() ?
                    new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.TrueKeyword, "true"), true) :
                    new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.FalseKeyword, "false"), false),
                JTokenType.Null => new NullLiteralSyntax(Helpers.CreateToken(TokenType.NullKeyword, "null")),
                _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
            };

        private ArraySyntax ParseJArray(JArray value)
        {
            var items = new List<ArrayItemSyntax>();
            items.Add(new ArrayItemSyntax(Helpers.CreateToken(TokenType.NewLine, "\n")));
            foreach (var item in value)
            {
                items.Add(new ArrayItemSyntax(ParseJToken(item)));
                items.Add(new ArrayItemSyntax(Helpers.CreateToken(TokenType.NewLine, "\n")));
            }

            return new ArraySyntax(
                Helpers.CreateToken(TokenType.LeftSquare, "["),
                items,
                Helpers.CreateToken(TokenType.RightSquare, "]"));
        }

        private ObjectSyntax ParseJObject(JObject value)
        {
            var properties = new List<SyntaxBase>();
            properties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
            foreach (var kvp in value)
            {
                properties.Add(new ObjectPropertySyntax(
                    Helpers.CreateObjectPropertyKey(kvp.Key),
                    Helpers.CreateToken(TokenType.Colon, ":"),
                    ParseJToken(kvp.Value)));
                properties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
            }

            return new ObjectSyntax(
                Helpers.CreateToken(TokenType.LeftBrace, "{"),
                properties,
                Helpers.CreateToken(TokenType.RightBrace, "}")
            );
        }

        public ParameterDeclarationSyntax ParseParam(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ArgumentException($"Unable to locate 'type' for parameter '{value.Name}'");

            var defaultValue = TryParseJToken(value.Value?["defaultValue"]);
            var objProperties = new Dictionary<string, SyntaxBase?>
            {
                ["allowed"] = TryParseJToken(value.Value?["allowedValues"]),
                ["minValue"] = TryParseJToken(value.Value?["minValue"]),
                ["maxValue"] = TryParseJToken(value.Value?["maxValue"]),
                ["minLength"] = TryParseJToken(value.Value?["minLength"]),
                ["maxLength"] = TryParseJToken(value.Value?["maxLength"]),
                ["metadata"] = TryParseJToken(value.Value?["metadata"]),
            };

            if (typeSyntax.TypeName == "securestring")
            {
                typeSyntax = new TypeSyntax(Helpers.CreateToken(TokenType.Identifier, "string"));
                objProperties["secure"] = new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.TrueKeyword, "true"), true);
            }

            if (typeSyntax.TypeName == "secureobject")
            {
                typeSyntax = new TypeSyntax(Helpers.CreateToken(TokenType.Identifier, "object"));
                objProperties["secure"] = new BooleanLiteralSyntax(Helpers.CreateToken(TokenType.TrueKeyword, "true"), true);
            }

            SyntaxBase? modifier = null;
            if (objProperties.Any(x => x.Value != null))
            {
                objProperties["default"] = defaultValue;
                var nonNullProperties = new List<SyntaxBase>();
                nonNullProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
                foreach (var kvp in objProperties)
                {
                    if (kvp.Value is null)
                    {
                        continue;
                    }

                    nonNullProperties.Add(new ObjectPropertySyntax(
                        Helpers.CreateObjectPropertyKey(kvp.Key),
                        Helpers.CreateToken(TokenType.Colon, ":"),
                        kvp.Value));
                    nonNullProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
                }

                modifier = new ObjectSyntax(
                    Helpers.CreateToken(TokenType.LeftBrace, "{"),
                    nonNullProperties,
                    Helpers.CreateToken(TokenType.RightBrace, "}"));
            }
            else if (defaultValue != null)
            {
                modifier = new ParameterDefaultValueSyntax(
                    Helpers.CreateToken(TokenType.Assignment, "="),
                    defaultValue);
            }

            var identifier = nameResolver.TryLookupName(NameType.Parameter, value.Name) ?? throw new ArgumentException($"Unable to find parameter {value.Name}");

            return new ParameterDeclarationSyntax(
                Helpers.CreateToken(TokenType.Identifier, "param"),
                Helpers.CreateIdentifier(identifier),
                typeSyntax,
                modifier);
        }

        public VariableDeclarationSyntax ParseVariable(JProperty value)
        {
            var identifier = nameResolver.TryLookupName(NameType.Variable, value.Name) ?? throw new ArgumentException($"Unable to find variable {value.Name}");

            return new VariableDeclarationSyntax(
                Helpers.CreateToken(TokenType.Identifier, "var"),
                Helpers.CreateIdentifier(identifier),
                Helpers.CreateToken(TokenType.Assignment, "="),
                ParseJToken(value.Value));
        }

        private string? TryGetModuleFilePath(JObject resource)
        {
            var templateLink = resource["properties"]?["templateLink"]?["uri"]?.Value<string>();
            if (templateLink == null)
            {
                return null;
            }

            var templateLinkExpression = FlattenConcatsAndInlineVariables(template, GetLanguageExpression(templateLink));

            LanguageExpression? relativePath = null;
            if (templateLinkExpression is FunctionExpression uriExpression && uriExpression.Function == "uri")
            {
                // it's common to format references to files using the uri function. the second param is the relative path (which should match the file system path)
                relativePath = uriExpression.Parameters[1];
            }
            else if (templateLinkExpression is FunctionExpression concatExpression && concatExpression.Function == "concat" &&
                concatExpression.Parameters[0] is FunctionExpression concatUriExpression && concatUriExpression.Function == "uri")
            {
                // or sometimes the other way around - uri expression inside a concat
                relativePath = concatUriExpression.Parameters[1];
            }

            if (!(relativePath is JTokenExpression jTokenExpression))
            {
                throw new ArgumentException($"Failed to process templateLink expression {templateLink}");
            }

            var nestedRelativePath = jTokenExpression.Value.ToString();
            var fullNestedFilePath = Path.Combine(Path.GetDirectoryName(filePath), nestedRelativePath);
            if (!File.Exists(fullNestedFilePath))
            {
                throw new ArgumentException($"Failed to process templateLink expression {templateLink}");
            }

            return Path.ChangeExtension(nestedRelativePath, "bicep").Replace("\\", "/");
        }

        public SyntaxBase ParseResource(JObject template, JToken value)
        {
            var resource = (value as JObject) ?? throw new ArgumentException($"Incorrect resource format");

            // mandatory properties
            var nameString = value["name"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'name' for resource '{resource["name"]}'");
            var typeString = value["type"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'type' for resource '{resource["name"]}'");
            var apiVersionString = value["apiVersion"]?.Value<string>() ?? throw new ArgumentException($"Unable to parse 'apiVersion' for resource '{resource["name"]}'");

            if (StringComparer.OrdinalIgnoreCase.Equals(typeString, "Microsoft.Resources/deployments"))
            {
                var expectedDeploymentProps = new HashSet<string>(new [] {
                    "name",
                    "type",
                    "apiVersion",
                    "location",
                    "properties",
                    "dependsOn",
                    "comments",
                }, StringComparer.OrdinalIgnoreCase);

                var moduleFilePath = TryGetModuleFilePath(resource);
                if (moduleFilePath != null)
                {
                    foreach (var prop in resource.Properties())
                    {
                        if (!expectedDeploymentProps.Contains(prop.Name))
                        {
                            if (prop.Name.ToLowerInvariant() == "copy")
                            {
                                throw new NotImplementedException($"Copy loops are not currently supported");
                            }

                            if (prop.Name.ToLowerInvariant() == "condition")
                            {
                                throw new NotImplementedException($"Conditionals are not currently supported");
                            }

                            if (resource["scope"] != null || resource["subscriptionId"] != null || resource["resourceGroup"] != null)
                            {
                                throw new ArgumentException($"'scope', 'subscriptionId' and 'resourceGroup' are not yet supported for linked templates");
                            }

                            throw new NotImplementedException($"Unrecognized top-level resource property '{prop.Name}'");
                        }
                    }

                    var paramProperties = new List<SyntaxBase>();
                    paramProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));

                    var parameters = (value["properties"]?["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>();
                    foreach (var param in parameters)
                    {
                        paramProperties.Add(new ObjectPropertySyntax(
                            Helpers.CreateObjectPropertyKey(param.Name),
                            Helpers.CreateToken(TokenType.Colon, ":"),
                            ParseJToken(param.Value["value"])));
                        paramProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
                    }

                    var moduleParams = new ObjectSyntax(
                        Helpers.CreateToken(TokenType.LeftBrace, "{"),
                        paramProperties,
                        Helpers.CreateToken(TokenType.RightBrace, "}"));

                    var moduleProperties = new List<SyntaxBase>();
                    moduleProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));

                    moduleProperties.Add(new ObjectPropertySyntax(
                        Helpers.CreateObjectPropertyKey("name"),
                        Helpers.CreateToken(TokenType.Colon, ":"),
                        ParseJToken(nameString)));
                    moduleProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));

                    moduleProperties.Add(new ObjectPropertySyntax(
                        Helpers.CreateObjectPropertyKey("params"),
                        Helpers.CreateToken(TokenType.Colon, ":"),
                        moduleParams));
                    moduleProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));

                    if (resource["dependsOn"] != null)
                    {
                        if (!(resource["dependsOn"] is JArray dependsOn))
                        {
                            throw new InvalidOperationException($"Parsing failed for dependsOn");
                        }

                        // bicep dependsOn behaves more like the 'reference' function - try to patch up this behavior
                        var dependsOnExpressions = dependsOn.Select(entry =>
                        {
                            var entryString = entry.Value<string>();
                            if (entryString == null)
                            {
                                throw new InvalidOperationException($"Parsing failed for dependsOn");
                            }

                            var entryExpression = GetLanguageExpression(entryString);
                            var resourceRef = TryLookupResource(entryExpression);
                            if (resourceRef != null)
                            {
                                // add a magic function - we'll turn this into an identifier later on
                                return new FunctionExpression("__bicep_reference", new [] { new JTokenExpression(resourceRef) }, Array.Empty<LanguageExpression>());
                            }

                            // nothing we can do, leave it alone
                            return entryExpression;
                        });

                        // use the patched dependsOn
                        dependsOn = new JArray(dependsOnExpressions.Select(ExpressionsProvider.SerializeExpression));

                        if (dependsOn.Any())
                        {
                            moduleProperties.Add(new ObjectPropertySyntax(
                                Helpers.CreateObjectPropertyKey("dependsOn"),
                                Helpers.CreateToken(TokenType.Colon, ":"),
                                TryParseJToken(dependsOn)!));
                            moduleProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
                        }
                    }

                    var moduleBody = new ObjectSyntax(
                        Helpers.CreateToken(TokenType.LeftBrace, "{"),
                        moduleProperties,
                        Helpers.CreateToken(TokenType.RightBrace, "}"));

                    var moduleIdentifier = nameResolver.TryLookupResourceName(typeString, GetLanguageExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");
                    
                    return new ModuleDeclarationSyntax(
                        Helpers.CreateToken(TokenType.Identifier, "module"),
                        Helpers.CreateIdentifier(moduleIdentifier),
                        Helpers.CreateStringLiteral(moduleFilePath),
                        Helpers.CreateToken(TokenType.Assignment, "="),
                        moduleBody);
                }

                throw new NotImplementedException($"Nested/linked deployments are not currently supported");
            }
            
            var topLevelProps = new HashSet<string>(new [] {
                "name",
                "type",
                "apiVersion",
                "sku",
                "kind",
                "managedBy",
                "managedByExtended",
                "location",
                "extendedLocation",
                "zones",
                "plan",
                "eTag",
                "tags",
                "scale",
                "identity",
                "properties",
                "dependsOn",
                "comments",
                "resources",
            }, StringComparer.OrdinalIgnoreCase);

            var topLevelPropsToOmit = new HashSet<string>(new [] {
                "type",
                "apiVersion",
                "dependsOn",
                "comments",
                "resources",
            }, StringComparer.OrdinalIgnoreCase);

            var nonNullProperties = new List<SyntaxBase>();
            nonNullProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
            foreach (var prop in resource.Properties())
            {
                if (topLevelPropsToOmit.Contains(prop.Name))
                {
                    continue;
                }

                if (!topLevelProps.Contains(prop.Name))
                {
                    if (prop.Name.ToLowerInvariant() == "copy")
                    {
                        throw new NotImplementedException($"Copy loops are not currently supported");
                    }

                    if (prop.Name.ToLowerInvariant() == "condition")
                    {
                        throw new NotImplementedException($"Conditionals are not currently supported");
                    }

                    throw new NotImplementedException($"Unrecognized top-level resource property '{prop.Name}'");
                }

                var valueSyntax = TryParseJToken(prop.Value);
                if (valueSyntax == null)
                {
                    throw new InvalidOperationException($"Parsing failed for top-level property {prop.Name} with value {prop.Value}");
                }

                nonNullProperties.Add(new ObjectPropertySyntax(
                    Helpers.CreateObjectPropertyKey(prop.Name),
                    Helpers.CreateToken(TokenType.Colon, ":"),
                    valueSyntax));
                nonNullProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
            }

            if (resource["dependsOn"] != null)
            {
                if (!(resource["dependsOn"] is JArray dependsOn))
                {
                    throw new InvalidOperationException($"Parsing failed for dependsOn");
                }

                // bicep dependsOn behaves more like the 'reference' function - try to patch up this behavior
                var dependsOnExpressions = dependsOn.Select(entry =>
                {
                    var entryString = entry.Value<string>();
                    if (entryString == null)
                    {
                        throw new InvalidOperationException($"Parsing failed for dependsOn");
                    }

                    var entryExpression = GetLanguageExpression(entryString);
                    var resourceRef = TryLookupResource(entryExpression);
                    if (resourceRef != null)
                    {
                        // add a magic function - we'll turn this into an identifier later on
                        return new FunctionExpression("__bicep_reference", new [] { new JTokenExpression(resourceRef) }, Array.Empty<LanguageExpression>());
                    }

                    // nothing we can do, leave it alone
                    return entryExpression;
                });

                // use the patched dependsOn
                dependsOn = new JArray(dependsOnExpressions.Select(ExpressionsProvider.SerializeExpression));

                if (dependsOn.Any())
                {
                    nonNullProperties.Add(new ObjectPropertySyntax(
                        Helpers.CreateObjectPropertyKey("dependsOn"),
                        Helpers.CreateToken(TokenType.Colon, ":"),
                        TryParseJToken(dependsOn)!));
                    nonNullProperties.Add(Helpers.CreateToken(TokenType.NewLine, "\n"));
                }
            }

            var resourceBody = new ObjectSyntax(
                Helpers.CreateToken(TokenType.LeftBrace, "{"),
                nonNullProperties,
                Helpers.CreateToken(TokenType.RightBrace, "}"));

            var identifier = nameResolver.TryLookupResourceName(typeString, GetLanguageExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");
            
            return new ResourceDeclarationSyntax(
                Helpers.CreateToken(TokenType.Identifier, "resource"),
                Helpers.CreateIdentifier(identifier),
                ParseString($"{typeString}@{apiVersionString}"),
                Helpers.CreateToken(TokenType.Assignment, "="),
                resourceBody);
        }

        public OutputDeclarationSyntax ParseOutput(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ArgumentException($"Unable to locate 'type' for output '{value.Name}'");
            var identifier = nameResolver.TryLookupName(NameType.Output, value.Name) ?? throw new ArgumentException($"Unable to find output {value.Name}");

            return new OutputDeclarationSyntax(
                Helpers.CreateToken(TokenType.Identifier, "output"),
                Helpers.CreateIdentifier(identifier),
                typeSyntax,
                Helpers.CreateToken(TokenType.Assignment, "="),
                ParseJToken(value.Value?["value"]));
        }

        private TargetScopeSyntax? ParseTargetScope(JObject template)
        {
            switch (template["$schema"]?.ToString())
            {
                case "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        Helpers.CreateToken(TokenType.Identifier, "targetScope"),
                        Helpers.CreateToken(TokenType.Assignment, "="),
                        Helpers.CreateStringLiteral("tenant"));
                case "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        Helpers.CreateToken(TokenType.Identifier, "targetScope"),
                        Helpers.CreateToken(TokenType.Assignment, "="),
                        Helpers.CreateStringLiteral("managementGroup"));
                case "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        Helpers.CreateToken(TokenType.Identifier, "targetScope"),
                        Helpers.CreateToken(TokenType.Assignment, "="),
                        Helpers.CreateStringLiteral("subscription"));
            }

            return null;
        }

        private ProgramSyntax Parse()
        {
            var statements = new List<SyntaxBase>();

            if ((template["functions"] as JArray)?.Any() == true)
            {
                throw new NotImplementedException($"User defined functions are not currently supported");
            }

            var targetScope = ParseTargetScope(template);
            if (targetScope != null)
            {
                statements.Add(targetScope);
            }

            var parameters = ((template["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>()).ToArray();
            var resources = (template["resources"] as JArray ?? Enumerable.Empty<JToken>()).SelectMany(FlattenAndNormalizeResource).ToArray();
            var variables = ((template["variables"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>()).ToArray();
            var outputs = ((template["outputs"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>()).ToArray();

            RegisterNames(parameters, resources, variables, outputs);

            statements.AddRange(parameters.Select(ParseParam));
            statements.AddRange(variables.Select(ParseVariable));
            statements.AddRange(resources.Select(r => ParseResource(template, r)));
            statements.AddRange(outputs.Select(ParseOutput));

            return new ProgramSyntax(
                statements.SelectMany(x => new [] { x, Helpers.CreateToken(TokenType.NewLine, "\n" )}),
                Helpers.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<Diagnostic>()
            );
        }
    }
}