// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
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
using Bicep.Decompiler.Exceptions;
using Bicep.Core.Workspaces;
using System.Collections.Immutable;
using Bicep.Core.Extensions;

namespace Bicep.Decompiler
{
    public class TemplateConverter
    {
        private readonly INamingResolver nameResolver;
        private readonly Workspace workspace;
        private readonly IFileResolver fileResolver;
        private readonly Uri fileUri;
        private readonly JObject template;

        private TemplateConverter(Workspace workspace, IFileResolver fileResolver, Uri fileUri, JObject template)
        {
            this.workspace = workspace;
            this.fileResolver = fileResolver;
            this.fileUri = fileUri;
            this.template = template;
            this.nameResolver = new UniqueNamingResolver();
        }

        public static ProgramSyntax DecompileTemplate(Workspace workspace, IFileResolver fileResolver, Uri fileUri, string content)
        {
            var instance = new TemplateConverter(workspace, fileResolver, fileUri, JObject.Parse(content, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Load,
            }));

            return instance.Parse();
        }

        private void RegisterNames(IEnumerable<JProperty> parameters, IEnumerable<JToken> resources, IEnumerable<JProperty> variables, IEnumerable<JProperty> outputs)
        {
            // Register names in order: parameters, outputs, resources, variables to deal with naming clashes.
            // This avoids renaming 'external' template symbolic names (params & outputs) where possible,
            // and prioritizes picking a shorter resource name over a shorter variable name.

            foreach (var parameter in parameters)
            {
                if (nameResolver.TryRequestName(NameType.Parameter, parameter.Name) == null)
                {
                    throw new ConversionFailedException($"Unable to pick unique name for parameter {parameter.Name}", parameter);
                }
            }

            foreach (var output in outputs)
            {
                if (nameResolver.TryRequestName(NameType.Output, output.Name) == null)
                {
                    throw new ConversionFailedException($"Unable to pick unique name for output {output.Name}", output);
                }
            }

            foreach (var resource in resources)
            {
                var nameString = resource["name"]?.Value<string>() ?? throw new ConversionFailedException($"Unable to parse 'name' for resource '{resource["name"]}'", resource);
                var typeString = resource["type"]?.Value<string>() ?? throw new ConversionFailedException($"Unable to parse 'type' for resource '{resource["name"]}'", resource);

                if (nameResolver.TryRequestResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) == null)
                {
                    throw new ConversionFailedException($"Unable to pick unique name for resource {typeString} {nameString}", resource);
                }
            }

            foreach (var variable in variables)
            {
                if (nameResolver.TryRequestName(NameType.Variable, variable.Name) == null)
                {
                    throw new ConversionFailedException($"Unable to pick unique name for variable {variable.Name}", variable);
                }
            }
        }

        private LanguageExpression InlineVariablesRecursive(LanguageExpression original)
        {
            if (original is not FunctionExpression functionExpression)
            {
                return original;
            }

            if (functionExpression.Function == "variables" && functionExpression.Parameters.Length == 1 && functionExpression.Parameters[0] is JTokenExpression variableNameExpression)
            {
                var variableVal = template["variables"]?[variableNameExpression.Value.ToString()];

                if (variableVal == null)
                {
                    throw new ArgumentException($"Unable to resolve variable {variableNameExpression.Value}");
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
            var normalizedForm = ExpressionHelpers.TryGetResourceNormalizedForm(expression);
            if (normalizedForm is null)
            {
                // try to look it up without the type string, using the expression as-is as the name
                // it's fairly common to refer to another resource by name only
                return nameResolver.TryLookupResourceName(null, expression);
            }

            return nameResolver.TryLookupResourceName(normalizedForm.Value.typeString, normalizedForm.Value.nameExpression);
        }

        private SyntaxBase? TryParseJToken(JToken? value)
            => value is null ? null : ParseJToken(value);

        private SyntaxBase ParseJToken(JToken? value)
            => value switch {
                JObject jObject => ParseJObject(jObject),
                JArray jArray => ParseJArray(jArray),
                JValue jValue => ParseJValue(jValue),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new ConversionFailedException($"Unrecognized token type {value.Type}", value),
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

            if (flattenedExpression is not FunctionExpression functionExpression)
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
                    baseSyntax = new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resolved));
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
                    baseSyntax = new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resolved));
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
                            new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resourceName)),
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
                JTokenType.Uri => ParseString(value.ToString()),
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
                ObjectPropertySyntax objectProperty;
                if (ExpressionsEngine.IsLanguageExpression(key))
                {
                    var keySyntax = ParseString(key);
                    if (keySyntax is not StringSyntax)
                    {
                        keySyntax = SyntaxHelpers.CreateInterpolatedKey(keySyntax);
                    }
                    
                    objectProperty = new ObjectPropertySyntax(keySyntax, SyntaxHelpers.CreateToken(TokenType.Colon, ":"), ParseJToken(value));
                }
                else
                {
                    objectProperty = SyntaxHelpers.CreateObjectProperty(key, ParseJToken(value));
                }

                properties.Add(objectProperty);
            }

            return SyntaxHelpers.CreateObject(properties);
        }

        public ParameterDeclarationSyntax ParseParam(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ConversionFailedException($"Unable to locate 'type' for parameter '{value.Name}'", value);

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

            var identifier = nameResolver.TryLookupName(NameType.Parameter, value.Name) ?? throw new ConversionFailedException($"Unable to find parameter {value.Name}", value);

            return new ParameterDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "param"),
                SyntaxHelpers.CreateIdentifier(identifier),
                typeSyntax,
                modifier);
        }

        public VariableDeclarationSyntax ParseVariable(JProperty value)
        {
            var identifier = nameResolver.TryLookupName(NameType.Variable, value.Name) ?? throw new ConversionFailedException($"Unable to find variable {value.Name}", value);

            return new VariableDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "var"),
                SyntaxHelpers.CreateIdentifier(identifier),
                SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                ParseJToken(value.Value));
        }

        private SyntaxBase GetModuleFilePath(JObject resource, string templateLink)
        {
            StringSyntax createFakeModulePath(string templateLinkExpression)
                => SyntaxHelpers.CreateStringLiteralWithComment("?", $"TODO: replace with correct path to {templateLinkExpression}");

            var templateLinkExpression = InlineVariables(ExpressionHelpers.ParseExpression(templateLink));

            var nestedRelativePath = ExpressionHelpers.TryGetLocalFilePathForTemplateLink(templateLinkExpression);
            if (nestedRelativePath is null)
            {
                // return the original expression so that the author can fix it up rather than failing
                return createFakeModulePath(templateLink);
            }
            
            var nestedUri = fileResolver.TryResolveModulePath(fileUri, nestedRelativePath);
            if (nestedUri == null || !fileResolver.TryRead(nestedUri, out _, out _))
            {
                // return the original expression so that the author can fix it up rather than failing
                return createFakeModulePath(templateLink);
            }

            var filePath = Path.ChangeExtension(nestedRelativePath, "bicep").Replace("\\", "/");
            return SyntaxHelpers.CreateStringLiteral(filePath);
        }

        private SyntaxBase? ProcessCondition(JObject resource)
        {
            JProperty? conditionProperty = TemplateHelpers.GetProperty(resource, "condition");

            if (conditionProperty == null)
            {
                return null;
            }

            SyntaxBase conditionExpression = ParseJToken(conditionProperty.Value);

            if (conditionExpression is not ParenthesizedExpressionSyntax)
            {
                conditionExpression = new ParenthesizedExpressionSyntax(
                    SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                    conditionExpression,
                    SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
            }

            return new IfConditionSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "if"),
                conditionExpression);
        }

        private SyntaxBase? ProcessDependsOn(JObject resource)
        {
            var dependsOnProp = TemplateHelpers.GetProperty(resource, "dependsOn");

            if (dependsOnProp == null)
            {
                return null;
            }

            if (dependsOnProp.Value is not JArray dependsOn)
            {
                throw new ConversionFailedException($"Parsing failed for dependsOn - expected an array", resource);
            }

            var syntaxItems = new List<SyntaxBase>();
            foreach (var entry in dependsOn)
            {
                var entryString = entry.Value<string>();
                if (entryString == null)
                {
                    throw new ConversionFailedException($"Parsing failed for dependsOn - expected a string value", entry);
                }

                var entryExpression = ExpressionHelpers.ParseExpression(entryString);
                var resourceRef = TryLookupResource(entryExpression);

                SyntaxBase syntaxEntry;
                if (resourceRef != null)
                {
                    syntaxEntry = new VariableAccessSyntax(SyntaxHelpers.CreateIdentifier(resourceRef));
                }
                else
                {
                    // we can't output anything intelligent - convert the expression and move on.
                    syntaxEntry = ParseJToken(entry);
                }

                
                syntaxItems.Add(syntaxEntry);
            }

            return SyntaxHelpers.CreateArray(syntaxItems);
        }

        private SyntaxBase? TryGetScopeProperty(JObject resource)
        {
            var subscriptionId = TemplateHelpers.GetProperty(resource, "subscriptionId");
            var resourceGroup = TemplateHelpers.GetProperty(resource, "resourceGroup");

            switch (subscriptionId, resourceGroup)
            {
                case (JProperty subId, JProperty rgName):
                    return new FunctionCallSyntax(
                        SyntaxHelpers.CreateIdentifier("resourceGroup"),
                        SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                        new [] { 
                            new FunctionArgumentSyntax(ParseJToken(subId.Value), SyntaxHelpers.CreateToken(TokenType.Comma, ",")),
                            new FunctionArgumentSyntax(ParseJToken(rgName.Value), null),
                        },
                        SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
                case (null, JProperty rgName):
                    return new FunctionCallSyntax(
                        SyntaxHelpers.CreateIdentifier("resourceGroup"),
                        SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                        new [] { 
                            new FunctionArgumentSyntax(ParseJToken(rgName.Value), null),
                        },
                        SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
                case (JProperty subId, null):
                    return new FunctionCallSyntax(
                        SyntaxHelpers.CreateIdentifier("subscription"),
                        SyntaxHelpers.CreateToken(TokenType.LeftParen, "("),
                        new [] { 
                            new FunctionArgumentSyntax(ParseJToken(subId.Value), null),
                        },
                        SyntaxHelpers.CreateToken(TokenType.RightParen, ")"));
            }

            return null;
        }

        private SyntaxBase ParseModule(JObject resource, string typeString, string nameString)
        {
            var expectedProps = new HashSet<string>(new [] {
                "condition",
                "name",
                "type",
                "apiVersion",
                "location",
                "properties",
                "dependsOn",
                "comments",
            }, StringComparer.OrdinalIgnoreCase);

            var propsToOmit = new HashSet<string>(new [] {
                "resourceGroup",
                "subscriptionId",
            }, StringComparer.OrdinalIgnoreCase);

            TemplateHelpers.AssertUnsupportedProperty(resource, "copy", "The 'copy' property is not supported");
            TemplateHelpers.AssertUnsupportedProperty(resource, "scope", "The 'scope' property is not supported");
            foreach (var prop in resource.Properties())
            {
                if (propsToOmit.Contains(prop.Name))
                {
                    continue;
                }

                if (!expectedProps.Contains(prop.Name))
                {
                    throw new ConversionFailedException($"Unrecognized top-level resource property '{prop.Name}'", prop);
                }
            }

            var ifCondition = ProcessCondition(resource);

            var parameters = (resource["properties"]?["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>();
            var paramProperties = new List<ObjectPropertySyntax>();
            foreach (var param in parameters)
            {
                paramProperties.Add(SyntaxHelpers.CreateObjectProperty(param.Name, ParseJToken(param.Value["value"])));
            }

            var properties = new List<ObjectPropertySyntax>();
            properties.Add(SyntaxHelpers.CreateObjectProperty("name", ParseJToken(nameString)));

            var scope = TryGetScopeProperty(resource);
            if (scope is not null)
            {
                properties.Add(SyntaxHelpers.CreateObjectProperty("scope", scope));
            }

            properties.Add(SyntaxHelpers.CreateObjectProperty("params", SyntaxHelpers.CreateObject(paramProperties)));

            var dependsOn = ProcessDependsOn(resource);
            if (dependsOn != null)
            {
                properties.Add(SyntaxHelpers.CreateObjectProperty("dependsOn", dependsOn));
            }

            var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");
            
            var nestedTemplate = TemplateHelpers.GetNestedProperty(resource, "properties", "template");
            if (nestedTemplate is not null)
            {
                if (nestedTemplate is not JObject nestedTemplateObject)
                {
                    throw new ConversionFailedException($"Expected template objectfor {typeString} {nameString}", nestedTemplate);
                }

                var expressionEvaluationScope = TemplateHelpers.GetNestedProperty(resource, "properties", "expressionEvaluationOptions", "scope")?.ToString();
                if (!StringComparer.OrdinalIgnoreCase.Equals(expressionEvaluationScope, "inner"))
                {
                    throw new ConversionFailedException($"Nested template decompilation requires 'inner' expression evaluation scope. See 'https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/linked-templates#expression-evaluation-scope-in-nested-templates' for more information {typeString} {nameString}", nestedTemplate);
                }
            
                var filePath = $"./nested_{identifier}.bicep";
                var nestedModuleUri = fileResolver.TryResolveModulePath(fileUri, filePath) ?? throw new ConversionFailedException($"Unable to module uri for {typeString} {nameString}", nestedTemplate);
                if (workspace.TryGetSyntaxTree(nestedModuleUri, out _))
                {
                    throw new ConversionFailedException($"Unable to generate duplicate module to path ${nestedModuleUri} for {typeString} {nameString}", nestedTemplate);
                }

                var nestedConverter = new TemplateConverter(workspace, fileResolver, nestedModuleUri, nestedTemplateObject);
                var nestedSyntaxTree = new SyntaxTree(nestedModuleUri, ImmutableArray<int>.Empty, nestedConverter.Parse());
                workspace.UpsertSyntaxTrees(nestedSyntaxTree.AsEnumerable());

                return new ModuleDeclarationSyntax(
                    SyntaxHelpers.CreateToken(TokenType.Identifier, "module"),
                    SyntaxHelpers.CreateIdentifier(identifier),
                    SyntaxHelpers.CreateStringLiteral(filePath),
                    SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                    ifCondition,
                    SyntaxHelpers.CreateObject(properties));
            }

            var templateLink = TemplateHelpers.GetNestedProperty(resource, "properties", "templateLink", "uri");
            if (templateLink?.Value<string>() is not string templateLinkString)
            {
                throw new ConversionFailedException($"Unable to find {resource["name"]}.properties.templateLink.uri for linked template.", resource);
            }

            return new ModuleDeclarationSyntax(
                SyntaxHelpers.CreateToken(TokenType.Identifier, "module"),
                SyntaxHelpers.CreateIdentifier(identifier),
                GetModuleFilePath(resource, templateLinkString),
                SyntaxHelpers.CreateToken(TokenType.Assignment, "="),
                ifCondition,
                SyntaxHelpers.CreateObject(properties));
        }

        public SyntaxBase ParseResource(JObject template, JToken value)
        {
            var resource = (value as JObject) ?? throw new ConversionFailedException($"Incorrect resource format", value);

            // mandatory properties
            var (typeString, nameString, apiVersionString) = TemplateHelpers.ParseResource(resource);

            if (StringComparer.OrdinalIgnoreCase.Equals(typeString, "Microsoft.Resources/deployments"))
            {
                return ParseModule(resource, typeString, nameString);
            }
            
            var expectedResourceProps = new HashSet<string>(new [] {
                "condition",
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
                "condition",
                "type",
                "apiVersion",
                "dependsOn",
                "comments",
            }, StringComparer.OrdinalIgnoreCase);

            TemplateHelpers.AssertUnsupportedProperty(resource, "copy", "The 'copy' property is not supported");
            TemplateHelpers.AssertUnsupportedProperty(resource, "scope", "The 'scope' property is not supported");

            var topLevelProperties = new List<ObjectPropertySyntax>();
            foreach (var prop in resource.Properties())
            {
                if (resourcePropsToOmit.Contains(prop.Name))
                {
                    continue;
                }

                if (!expectedResourceProps.Contains(prop.Name))
                {
                    throw new ConversionFailedException($"Unrecognized top-level resource property '{prop.Name}'", prop);
                }

                var valueSyntax = TryParseJToken(prop.Value);
                if (valueSyntax == null)
                {
                    throw new ConversionFailedException($"Parsing failed for property value {prop.Value}", prop.Value);
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
                ProcessCondition(resource),
                SyntaxHelpers.CreateObject(topLevelProperties));
        }

        public OutputDeclarationSyntax ParseOutput(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ConversionFailedException($"Unable to locate 'type' for output '{value.Name}'", value);
            var identifier = nameResolver.TryLookupName(NameType.Output, value.Name) ?? throw new ConversionFailedException($"Unable to find output {value.Name}", value);

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

            var functions = TemplateHelpers.GetProperty(template, "functions")?.Value as JArray;
            if (functions?.Any() == true)
            {
                throw new ConversionFailedException($"User defined functions are not currently supported", functions);
            }

            var targetScope = ParseTargetScope(template);
            if (targetScope != null)
            {
                statements.Add(targetScope);
            }

            var parameters = (TemplateHelpers.GetProperty(template, "parameters")?.Value as JObject ?? new JObject()).Properties();
            var resources = TemplateHelpers.GetProperty(template, "resources")?.Value as JArray ?? new JArray();
            var variables = (TemplateHelpers.GetProperty(template, "variables")?.Value as JObject ?? new JObject()).Properties();
            var outputs = (TemplateHelpers.GetProperty(template, "outputs")?.Value as JObject ?? new JObject()).Properties();

            // FlattenAndNormalizeResource has side effects, so use .ToArray() to force single enumeration
            var flattenedResources = resources.SelectMany(TemplateHelpers.FlattenAndNormalizeResource).ToArray();

            RegisterNames(parameters, flattenedResources, variables, outputs);

            AddSyntaxBlock(statements, parameters.Select(ParseParam), false);
            AddSyntaxBlock(statements, variables.Select(ParseVariable), false);
            AddSyntaxBlock(statements, flattenedResources.Select(r => ParseResource(template, r)), true);
            AddSyntaxBlock(statements, outputs.Select(ParseOutput), false);

            return new ProgramSyntax(
                statements.SelectMany(x => new [] { x, SyntaxHelpers.NewlineToken}),
                SyntaxHelpers.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<Diagnostic>()
            );
        }
    }
}