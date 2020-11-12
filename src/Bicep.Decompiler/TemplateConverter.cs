// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.BicepHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Deployments.Expression.Expressions;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Azure.Deployments.Expression.Engines;
using Bicep.Core.FileSystem;

namespace Bicep.Decompiler
{
    public class TemplateConverter
    {
        private readonly INamingResolver nameResolver;
        private readonly IFileResolver fileResolver;
        private readonly Uri fileUri;
        private readonly JObject template;

        private TemplateConverter(IFileResolver fileResolver, Uri fileUri, string content)
        {
            this.fileResolver = fileResolver;
            this.fileUri = fileUri;
            this.template = JsonConvert.DeserializeObject<JObject>(content);
            this.nameResolver = new UniqueNamingResolver();
        }

        public static ProgramSyntax DecompileTemplate(IFileResolver fileResolver, Uri fileUri, string content)
        {
            var instance = new TemplateConverter(fileResolver, fileUri, content);

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

                if (nameResolver.TryRequestResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) == null)
                {
                    throw new InvalidOperationException($"Unable to pick unique name for resource {typeString} {nameString}");
                }
            }
        }

        private LanguageExpression InlineVariablesRecursive(LanguageExpression original)
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
                    var variableExpression = ExpressionHelpers.ParseExpression(stringValue);

                    return InlineVariablesRecursive(variableExpression);
                }
            }

            var inlinedParameters = functionExpression.Parameters.Select(p => InlineVariablesRecursive(p));

            return new FunctionExpression(
                functionExpression.Function,
                inlinedParameters.ToArray(),
                functionExpression.Properties);
        }

        private LanguageExpression InlineVariables(LanguageExpression original)
        {
            var inlined = InlineVariablesRecursive(original);

            return ExpressionHelpers.FlattenStringOperations(inlined);
        }

        private static LanguageExpression FormatNameExpression(IEnumerable<LanguageExpression> nameSegments)
        {
            var pathSegments = nameSegments
                .SelectMany((expression, i) => i == 0 ? new [] { expression } : new [] { new JTokenExpression("/"), expression });
            var nameExpression = ExpressionHelpers.Concat(pathSegments.ToArray());

            return ExpressionHelpers.FlattenStringOperations(nameExpression);
        }

        private static TypeSyntax? TryParseType(JToken? value)
        {
            var typeString = value?.Value<string>();
            if (typeString == null)
            {
                return null;
            }

            return new TypeSyntax(SyntaxHelpers.CreateToken(TokenType.Identifier, typeString.ToLowerInvariant()));
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

            return nameResolver.TryLookupResourceName(typeString, nameExpression);
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
                JTokenType.String => SyntaxHelpers.CreateStringLiteral(expression.Value.Value<string>()!),
                JTokenType.Integer => new NumericLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.Number, expression.Value.ToString()), expression.Value.Value<int>()),
                JTokenType.Boolean =>  expression.Value.Value<bool>() ?
                    new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.TrueKeyword, "true"), true) :
                    new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.FalseKeyword, "false"), false),
                JTokenType.Null => new NullLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.NullKeyword, "null")),
                _ => throw new NotImplementedException($"Unrecognized expression {ExpressionsEngine.SerializeExpression(expression)}"),
            };

        private bool TryReplaceBannedFunction(FunctionExpression expression, [NotNullWhen(true)] out SyntaxBase? syntax)
        {
            if (SyntaxHelpers.BannedBinaryOperatorLookup.TryGetValue(expression.Function, out var bannedOperator))
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
                    SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                    new BinaryOperationSyntax(
                        ParseLanguageExpression(expression.Parameters[0]),
                        bannedOperator,
                        ParseLanguageExpression(expression.Parameters[1])),
                    SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
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
                    SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                    new UnaryOperationSyntax(
                        SyntaxHelpers.CreateToken(TokenType.Exclamation, "!"),
                        ParseLanguageExpression(expression.Parameters[0])),
                    SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
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
                    SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                    new TernaryOperationSyntax(
                        ParseLanguageExpression(expression.Parameters[0]),
                        SyntaxHelpers.CreateToken(TokenType.Question, "?"),
                        ParseLanguageExpression(expression.Parameters[1]),
                        SyntaxHelpers.CreateToken(TokenType.Colon, ":"),
                        ParseLanguageExpression(expression.Parameters[2])),
                    SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
                return true;
            }

            syntax = null;
            return false;
        }

        private SyntaxBase? TryParseStringExpression(LanguageExpression expression)
        {
            var flattenedExpression = ExpressionHelpers.FlattenStringOperations(expression);
            if (flattenedExpression is JTokenExpression flattenedJTokenExpression)
            {
                var stringVal = flattenedJTokenExpression.Value.Value<string>();
                if (stringVal == null)
                {
                    return null;
                }

                return SyntaxHelpers.CreateStringLiteral(stringVal);
            }

            if (!(flattenedExpression is FunctionExpression functionExpression))
            {
                throw new InvalidOperationException($"Expected {nameof(FunctionExpression)}");
            }
            expression = functionExpression;

            var values = new List<string>();
            var stringTokens = new List<Token>();
            var expressions = new List<SyntaxBase>();
            for (var i = 0; i < functionExpression.Parameters.Length; i++)
            {
                // FlattenStringOperations will have already simplified the concat statement to the point where we know there won't be two string literals side-by-side.
                // We can use that knowledge to simplify this logic.

                var isStart = (i == 0);
                var isEnd = (i == functionExpression.Parameters.Length - 1);

                if (functionExpression.Parameters[i] is JTokenExpression jTokenExpression)
                {
                    stringTokens.Add(SyntaxHelpers.CreateStringInterpolationToken(isStart, isEnd, jTokenExpression.Value.ToString()));

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
                    stringTokens.Add(SyntaxHelpers.CreateStringInterpolationToken(isStart, false, ""));
                }

                expressions.Add(ParseLanguageExpression(functionExpression.Parameters[i]));
                isStart = (i == 0);
                isEnd = (i == functionExpression.Parameters.Length - 1);

                if (isEnd)
                {
                    // always make sure we end with a string token
                    stringTokens.Add(SyntaxHelpers.CreateStringInterpolationToken(isStart, isEnd, ""));
                }
            }

            var rawSegments = Lexer.TryGetRawStringSegments(stringTokens);
            if (rawSegments == null)
            {
                return null;
            }
            
            return new StringSyntax(stringTokens, expressions, rawSegments);
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
                        throw new NotImplementedException($"Unable to process parameter with non-constant name {ExpressionsEngine.SerializeExpression(expression)}");
                    }

                    var stringVal = jTokenExpression.Value.Value<string>()!;
                    var resolved = nameResolver.TryLookupName(NameType.Parameter, stringVal) ?? throw new ArgumentException($"Unable to find parameter {stringVal}");
                    baseSyntax = SyntaxHelpers.CreateIdentifier(resolved);
                    break;
                }
                case "variables":
                {
                    if (expression.Parameters.Length != 1 || !(expression.Parameters[0] is JTokenExpression jTokenExpression) || jTokenExpression.Value.Type != JTokenType.String)
                    {
                        throw new NotImplementedException($"Unable to process variable with non-constant name {ExpressionsEngine.SerializeExpression(expression)}");
                    }

                    var stringVal = jTokenExpression.Value.Value<string>()!;
                    var resolved = nameResolver.TryLookupName(NameType.Variable, stringVal) ?? throw new ArgumentException($"Unable to find variable {stringVal}");
                    baseSyntax = SyntaxHelpers.CreateIdentifier(resolved);
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
                                new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resourceName)),
                                SyntaxHelpers.CreateToken(TokenType.Dot, "."),
                                SyntaxHelpers.CreateIdentifier("properties"));
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
                            SyntaxHelpers.CreateIdentifier(resourceName),
                            SyntaxHelpers.CreateToken(TokenType.Dot, "."),
                            SyntaxHelpers.CreateIdentifier("id"));
                    }
                    break;
                }
                case "format":
                {
                    baseSyntax = TryParseStringExpression(expression);
                    break;
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

                    baseSyntax = TryParseStringExpression(expression);
                    break;
                }
                case "__bicep_reference":
                {
                    var resourceRef = (expression.Parameters[0] as JTokenExpression)?.Value.Value<string>() ?? throw new InvalidOperationException("Internal error");
                    baseSyntax = new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resourceRef));
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
                var functionName = SyntaxHelpers.CorrectWellKnownFunctionCasing(expression.Function);

                var expressions = expression.Parameters.Select(ParseLanguageExpression).ToArray();

                baseSyntax = new FunctionCallSyntax(
                    SyntaxHelpers.CreateIdentifier(functionName),
                    SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                    expressions.Select((x, i) => new FunctionArgumentSyntax(x, i < expressions.Length - 1 ? SyntaxHelpers.CreateToken(TokenType.Comma, ",") : null)),
                    SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
            }

            foreach (var property in expression.Properties)
            {
                if (property is JTokenExpression jTokenExpression && jTokenExpression.Value.Type == JTokenType.String)
                {
                    // TODO handle special chars and generate array access instead
                    baseSyntax = new PropertyAccessSyntax(
                        baseSyntax,
                        SyntaxHelpers.CreateToken(TokenType.Dot, "."),
                        SyntaxHelpers.CreateIdentifier(jTokenExpression.Value.Value<string>()!));
                }
                else
                {
                    baseSyntax = new ArrayAccessSyntax(
                        baseSyntax,
                        SyntaxHelpers.CreateToken(TokenType.LeftSquare, "["),
                        ParseLanguageExpression(property),
                        SyntaxHelpers.CreateToken(TokenType.RightSquare, "]"));
                }
            }

            return baseSyntax;
        }

        private SyntaxBase ParseLanguageExpression(LanguageExpression expression)
            => expression switch
            {
                JTokenExpression jTokenExpression => ParseJTokenExpression(jTokenExpression),
                FunctionExpression functionExpression => ParseFunctionExpression(functionExpression),
                _ => throw new NotImplementedException($"Unrecognized expression {ExpressionsEngine.SerializeExpression(expression)}"),
            };

        private SyntaxBase ParseString(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (ExpressionsEngine.IsLanguageExpression(value))
            {
                var expression = ExpressionsEngine.ParseLanguageExpression(value);

                return ParseLanguageExpression(expression);
            }
            
            return SyntaxHelpers.CreateStringLiteral(value);
        }

        private static SyntaxBase ParseIntegerJToken(JValue value)
        {
            // JTokenType.Integer can encapsulate non-C#-int numbers.
            // Best we can do in this case is to convert them to strings.
            var longValue = value.Value<long>();
            if (longValue < int.MinValue || longValue > int.MaxValue)
            {
                return SyntaxHelpers.CreateStringLiteral(value.ToString());
            }

            return new NumericLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.Number, value.ToString()), (int)longValue);
        }

        private SyntaxBase ParseJValue(JValue value)
            => value.Type switch {
                JTokenType.String => ParseString(value.ToString()),
                JTokenType.Integer => ParseIntegerJToken(value),
                JTokenType.Date => ParseString(value.ToString()),
                JTokenType.Float => ParseString(value.ToString()),
                JTokenType.Boolean =>  value.Value<bool>() ?
                    new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.TrueKeyword, "true"), true) :
                    new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.FalseKeyword, "false"), false),
                JTokenType.Null => new NullLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.NullKeyword, "null")),
                _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
            };

        private ArraySyntax ParseJArray(JArray value)
        {
            var items = new List<SyntaxBase>();

            foreach (var item in value)
            {
                items.Add(ParseJToken(item));
            }

            return SyntaxHelpers.CreateArray(items);
        }

        private ObjectSyntax ParseJObject(JObject jObject)
        {
            var properties = new List<ObjectPropertySyntax>();
            foreach (var (key, value) in jObject)
            {
                properties.Add(SyntaxHelpers.CreateObjectProperty(key, ParseJToken(value)));
            }

            return SyntaxHelpers.CreateObject(properties);
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
                typeSyntax = new TypeSyntax(SyntaxHelpers.CreateToken(TokenType.Identifier, "string"));
                objProperties["secure"] = new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.TrueKeyword, "true"), true);
            }

            if (typeSyntax.TypeName == "secureobject")
            {
                typeSyntax = new TypeSyntax(SyntaxHelpers.CreateToken(TokenType.Identifier, "object"));
                objProperties["secure"] = new BooleanLiteralSyntax(SyntaxHelpers.CreateToken(TokenType.TrueKeyword, "true"), true);
            }

            SyntaxBase? modifier = null;
            if (objProperties.Any(x => x.Value != null))
            {
                objProperties["default"] = defaultValue;
                var nonNullProperties = new List<ObjectPropertySyntax>();

                foreach (var (key, propValue) in objProperties)
                {
                    if (propValue is null)
                    {
                        continue;
                    }

                    nonNullProperties.Add(SyntaxHelpers.CreateObjectProperty(key, propValue));
                }

                modifier = SyntaxHelpers.CreateObject(nonNullProperties);
            }
            else if (defaultValue != null)
            {
                modifier = new ParameterDefaultValueSyntax(
                    SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                    defaultValue);
            }

            var identifier = nameResolver.TryLookupName(NameType.Parameter, value.Name) ?? throw new ArgumentException($"Unable to find parameter {value.Name}");

            return new ParameterDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "param"),
                SyntaxHelpers.CreateIdentifier(identifier),
                typeSyntax,
                modifier);
        }

        public VariableDeclarationSyntax ParseVariable(JProperty value)
        {
            var identifier = nameResolver.TryLookupName(NameType.Variable, value.Name) ?? throw new ArgumentException($"Unable to find variable {value.Name}");

            return new VariableDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "var"),
                SyntaxHelpers.CreateIdentifier(identifier),
                SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                ParseJToken(value.Value));
        }

        private string? TryGetModuleFilePath(JObject resource)
        {
            var templateLink = resource["properties"]?["templateLink"]?["uri"]?.Value<string>();
            if (templateLink == null)
            {
                return null;
            }

            var templateLinkExpression = InlineVariables(ExpressionHelpers.ParseExpression(templateLink));

            LanguageExpression? relativePath = null;
            if (templateLinkExpression is FunctionExpression uriExpression && uriExpression.Function == "uri")
            {
                // it's common to format references to files using the uri function. the second param is the relative path (which should match the file system path)
                relativePath = uriExpression.Parameters[1];
            }
            else if (templateLinkExpression is FunctionExpression concatExpression && concatExpression.Function == "concat")
            {
                if (concatExpression.Parameters[0] is FunctionExpression concatUriExpression && concatUriExpression.Function == "uri")
                {
                    // or sometimes the other way around - uri expression inside a concat
                    relativePath = concatExpression.Parameters[1];
                }
                else if (concatExpression.Parameters[0] is FunctionExpression concatParametersExpression && concatParametersExpression.Function == "parameters" && concatExpression.Parameters.Length == 2)
                {
                    // URI prefix in a parameter
                    relativePath = concatExpression.Parameters[1];
                }
            }

            if (!(relativePath is JTokenExpression jTokenExpression))
            {
                throw new ArgumentException($"Failed to process templateLink expression {templateLink}");
            }

            var nestedRelativePath = jTokenExpression.Value.ToString().Trim('/');
            var nestedUri = fileResolver.TryResolveModulePath(fileUri, nestedRelativePath);
            if (nestedUri == null || !fileResolver.TryRead(nestedUri, out _, out _))
            {
                throw new ArgumentException($"Failed to process templateLink expression {templateLink}");
            }

            return Path.ChangeExtension(nestedRelativePath, "bicep").Replace("\\", "/");
        }

        private SyntaxBase? ProcessDependsOn(JObject resource)
        {
            var dependsOnProp = TemplateHelpers.GetProperty(resource, "dependsOn");

            if (dependsOnProp == null)
            {
                return null;
            }

            if (!(dependsOnProp.Value is JArray dependsOn))
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

                var entryExpression = ExpressionHelpers.ParseExpression(entryString);
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
            dependsOn = new JArray(dependsOnExpressions.Select(x => ExpressionsEngine.SerializeExpression(x)));
            if (!dependsOn.Any())
            {
                return null;
            }

            return ParseJToken(dependsOn);
        }

        private SyntaxBase ParseModule(JObject resource, string typeString, string nameString)
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
                TemplateHelpers.AssertUnsupportedProperty(resource, "copy", "Copy loops are not currently supported");
                TemplateHelpers.AssertUnsupportedProperty(resource, "condition", "Conditionals are not currently supported");
                TemplateHelpers.AssertUnsupportedProperty(resource, "scope", "Scope is not supported for linked/nested templates");
                TemplateHelpers.AssertUnsupportedProperty(resource, "subscriptionId", "SubscriptionId is not supported for linked/nested templates");
                TemplateHelpers.AssertUnsupportedProperty(resource, "resourceGroup", "ResourceGroup is not supported for linked/nested templates");
                foreach (var prop in resource.Properties())
                {
                    if (!expectedDeploymentProps.Contains(prop.Name))
                    {
                        throw new NotImplementedException($"Unrecognized top-level resource property '{prop.Name}'");
                    }
                }

                var parameters = (resource["properties"]?["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>();
                var paramProperties = new List<ObjectPropertySyntax>();
                foreach (var param in parameters)
                {
                    paramProperties.Add(SyntaxHelpers.CreateObjectProperty(param.Name, ParseJToken(param.Value["value"])));
                }

                var properties = new List<ObjectPropertySyntax>();
                properties.Add(SyntaxHelpers.CreateObjectProperty("name", ParseJToken(nameString)));
                properties.Add(SyntaxHelpers.CreateObjectProperty("params", SyntaxHelpers.CreateObject(paramProperties)));

                var dependsOn = ProcessDependsOn(resource);
                if (dependsOn != null)
                {
                    properties.Add(SyntaxHelpers.CreateObjectProperty("dependsOn", dependsOn));
                }

                var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");
                
                return new ModuleDeclarationSyntax(
                    SyntaxHelpers.CreateToken(TokenType.Identifier, "module"),
                    SyntaxHelpers.CreateIdentifier(identifier),
                    SyntaxHelpers.CreateStringLiteral(moduleFilePath),
                    SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                    SyntaxHelpers.CreateObject(properties));
            }

            throw new NotImplementedException($"Nested/linked deployments are not currently supported");
        }

        public SyntaxBase ParseResource(JObject template, JToken value)
        {
            var resource = (value as JObject) ?? throw new ArgumentException($"Incorrect resource format");

            // mandatory properties
            var (typeString, nameString, apiVersionString) = TemplateHelpers.ParseResource(resource);

            if (StringComparer.OrdinalIgnoreCase.Equals(typeString, "Microsoft.Resources/deployments"))
            {
                return ParseModule(resource, typeString, nameString);
            }
            
            var expectedResourceProps = new HashSet<string>(new [] {
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
            }, StringComparer.OrdinalIgnoreCase);

            var resourcePropsToOmit = new HashSet<string>(new [] {
                "type",
                "apiVersion",
                "dependsOn",
                "comments",
            }, StringComparer.OrdinalIgnoreCase);

            TemplateHelpers.AssertUnsupportedProperty(resource, "copy", "Copy loops are not currently supported");
            TemplateHelpers.AssertUnsupportedProperty(resource, "condition", "Conditionals are not currently supported");
            TemplateHelpers.AssertUnsupportedProperty(resource, "scope", "Scope is not supported for resources");

            var topLevelProperties = new List<ObjectPropertySyntax>();
            foreach (var prop in resource.Properties())
            {
                if (resourcePropsToOmit.Contains(prop.Name))
                {
                    continue;
                }

                if (!expectedResourceProps.Contains(prop.Name))
                {
                    throw new NotImplementedException($"Unrecognized top-level resource property '{prop.Name}'");
                }

                var valueSyntax = TryParseJToken(prop.Value);
                if (valueSyntax == null)
                {
                    throw new InvalidOperationException($"Parsing failed for top-level property {prop.Name} with value {prop.Value}");
                }

                topLevelProperties.Add(SyntaxHelpers.CreateObjectProperty(prop.Name, valueSyntax));
            }

            var dependsOn = ProcessDependsOn(resource);
            if (dependsOn != null)
            {
                topLevelProperties.Add(SyntaxHelpers.CreateObjectProperty("dependsOn", dependsOn));
            }

            var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");
            
            return new ResourceDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "resource"),
                SyntaxHelpers.CreateIdentifier(identifier),
                ParseString($"{typeString}@{apiVersionString}"),
                SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                SyntaxHelpers.CreateObject(topLevelProperties));
        }

        public OutputDeclarationSyntax ParseOutput(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ArgumentException($"Unable to locate 'type' for output '{value.Name}'");
            var identifier = nameResolver.TryLookupName(NameType.Output, value.Name) ?? throw new ArgumentException($"Unable to find output {value.Name}");

            return new OutputDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "output"),
                SyntaxHelpers.CreateIdentifier(identifier),
                typeSyntax,
                SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                ParseJToken(value.Value?["value"]));
        }

        private TargetScopeSyntax? ParseTargetScope(JObject template)
        {
            switch (template["$schema"]?.ToString())
            {
                case "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        SyntaxHelpers.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                        SyntaxHelpers.CreateStringLiteral("tenant"));
                case "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        SyntaxHelpers.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                        SyntaxHelpers.CreateStringLiteral("managementGroup"));
                case "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#":
                    return new TargetScopeSyntax(
                        SyntaxHelpers.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                        SyntaxHelpers.CreateStringLiteral("subscription"));
            }

            return null;
        }

        private static void AddSyntaxBlock(IList<SyntaxBase> syntaxes, IEnumerable<SyntaxBase> syntaxesToAdd, bool newLineBetweenItems)
        {
            // force enumeration
            var syntaxesToAddArray = syntaxesToAdd.ToArray();

            for (var i = 0; i < syntaxesToAddArray.Length; i++)
            {
                syntaxes.Add(syntaxesToAddArray[i]);
                if (newLineBetweenItems && i < syntaxesToAddArray.Length - 1)
                {
                    // only add a new line between items, not after the last item
                    syntaxes.Add(SyntaxHelpers.NewlineToken);
                }
            }

            if (syntaxesToAdd.Any())
            {
                // always add a new line after a block
                syntaxes.Add(SyntaxHelpers.NewlineToken);
            }
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

            var parameters = (TemplateHelpers.GetProperty(template, "parameters")?.Value as JObject ?? new JObject()).Properties();
            var resources = (TemplateHelpers.GetProperty(template, "resources")?.Value as JArray ?? new JArray()).SelectMany(TemplateHelpers.FlattenAndNormalizeResource);
            var variables = (TemplateHelpers.GetProperty(template, "variables")?.Value as JObject ?? new JObject()).Properties();
            var outputs = (TemplateHelpers.GetProperty(template, "outputs")?.Value as JObject ?? new JObject()).Properties();

            RegisterNames(parameters, resources, variables, outputs);

            AddSyntaxBlock(statements, parameters.Select(ParseParam), false);
            AddSyntaxBlock(statements, variables.Select(ParseVariable), false);
            AddSyntaxBlock(statements, resources.Select(r => ParseResource(template, r)), true);
            AddSyntaxBlock(statements, outputs.Select(ParseOutput), false);

            return new ProgramSyntax(
                statements.SelectMany(x => new [] { x, SyntaxHelpers.NewlineToken}),
                SyntaxHelpers.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<Diagnostic>()
            );
        }
    }
}