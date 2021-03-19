// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.BicepHelpers;
using Bicep.Decompiler.Exceptions;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler
{
    public class TemplateConverter
    {
        private const string ResourceCopyLoopIndexVar = "i";
        private const string PropertyCopyLoopIndexVar = "j";

        private INamingResolver nameResolver;
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

            foreach (var resource in resources.OfType<JObject>())
            {
                var (typeString, nameString, _) = TemplateHelpers.ParseResource(resource);
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

            return new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, typeString.ToLowerInvariant()));
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
                JTokenType.String => SyntaxFactory.CreateStringLiteral(expression.Value.Value<string>()!),
                JTokenType.Integer => new IntegerLiteralSyntax(SyntaxFactory.CreateToken(TokenType.Integer, expression.Value.ToString()), expression.Value.Value<long>()),
                JTokenType.Boolean =>  expression.Value.Value<bool>() ?
                    new BooleanLiteralSyntax(SyntaxFactory.TrueKeywordToken, true) :
                    new BooleanLiteralSyntax(SyntaxFactory.FalseKeywordToken, false),
                JTokenType.Null => new NullLiteralSyntax(SyntaxFactory.NullKeywordToken),
                _ => throw new NotImplementedException($"Unrecognized expression {ExpressionsEngine.SerializeExpression(expression)}"),
            };

        private bool TryReplaceBannedFunction(FunctionExpression expression, [NotNullWhen(true)] out SyntaxBase? syntax)
        {
            if (SyntaxHelpers.TryGetBinaryOperatorReplacement(expression.Function) is TokenType binaryTokenType)
            {
                var binaryOperator = Operators.TokenTypeToBinaryOperator[binaryTokenType];
                switch (binaryOperator)
                {
                    // ARM actually allows >= 2 args for and(), or() and coalesce()
                    case BinaryOperator.LogicalAnd:
                    case BinaryOperator.LogicalOr:
                    case BinaryOperator.Coalesce:
                        if (expression.Parameters.Length < 2)
                        {
                            throw new ArgumentException($"Expected a minimum of 2 parameters for function {expression.Function}");
                        }
                        break;
                    default:
                        if (expression.Parameters.Length != 2)
                        {
                            throw new ArgumentException($"Expected 2 parameters for binary function {expression.Function}");
                        }
                        break;
                }

                if (expression.Properties.Any())
                {
                    throw new ArgumentException($"Expected 0 properties for binary function {expression.Function}");
                }

                var binaryOperation = new BinaryOperationSyntax(
                    ParseLanguageExpression(expression.Parameters[0]),
                    SyntaxFactory.CreateToken(binaryTokenType, Operators.BinaryOperatorToText[binaryOperator]),
                    ParseLanguageExpression(expression.Parameters[1]));

                foreach (var parameter in expression.Parameters.Skip(2))
                {
                    binaryOperation = new BinaryOperationSyntax(
                        binaryOperation,
                        SyntaxFactory.CreateToken(binaryTokenType, Operators.BinaryOperatorToText[binaryOperator]),
                        ParseLanguageExpression(parameter));
                }

                syntax = new ParenthesizedExpressionSyntax(
                    SyntaxFactory.LeftParenToken,
                    binaryOperation,
                    SyntaxFactory.RightParenToken);
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
                    SyntaxFactory.LeftParenToken,
                    new UnaryOperationSyntax(
                        SyntaxFactory.ExclamationToken,
                        ParseLanguageExpression(expression.Parameters[0])),
                    SyntaxFactory.RightParenToken);
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
                    SyntaxFactory.LeftParenToken,
                    new TernaryOperationSyntax(
                        ParseLanguageExpression(expression.Parameters[0]),
                        SyntaxFactory.QuestionToken,
                        ParseLanguageExpression(expression.Parameters[1]),
                        SyntaxFactory.ColonToken,
                        ParseLanguageExpression(expression.Parameters[2])),                        
                    SyntaxFactory.RightParenToken);
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

                return SyntaxFactory.CreateStringLiteral(stringVal);
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
                    stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, isEnd, jTokenExpression.Value.ToString()));

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
                    stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, false, ""));
                }

                expressions.Add(ParseLanguageExpression(functionExpression.Parameters[i]));
                isStart = (i == 0);
                isEnd = (i == functionExpression.Parameters.Length - 1);

                if (isEnd)
                {
                    // always make sure we end with a string token
                    stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, isEnd, ""));
                }
            }

            var rawSegments = Lexer.TryGetRawStringSegments(stringTokens);
            if (rawSegments == null)
            {
                return null;
            }
            
            return new StringSyntax(stringTokens, expressions, rawSegments);
        }

        private bool CanInterpolate(FunctionExpression function)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(function.Function, "format"))
            {
                return true;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(function.Function, "concat"))
            {
                return false;
            }

            foreach (var parameter in function.Parameters)
            {
                if (parameter is JTokenExpression jTokenExpression && jTokenExpression.Value.Type == JTokenType.String)
                {
                    return true;
                }

                if (parameter is FunctionExpression paramFunction && CanInterpolate(paramFunction))
                {
                    return true;
                }
            }
            
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
                        throw new NotImplementedException($"Unable to process parameter with non-constant name {ExpressionsEngine.SerializeExpression(expression)}");
                    }

                    var stringVal = jTokenExpression.Value.Value<string>()!;
                    var resolved = nameResolver.TryLookupName(NameType.Parameter, stringVal) ?? throw new ArgumentException($"Unable to find parameter {stringVal}");
                    baseSyntax = new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resolved));
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
                    baseSyntax = new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resolved));
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
                                new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resourceName)),
                                SyntaxFactory.DotToken,
                                SyntaxFactory.CreateIdentifier("properties"));
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
                            new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resourceName)),
                            SyntaxFactory.DotToken,
                            SyntaxFactory.CreateIdentifier("id"));
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
                    if (!CanInterpolate(expression))
                    {
                        // we might be dealing with an array
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

                baseSyntax = SyntaxFactory.CreateFunctionCall(functionName, expressions);
            }

            foreach (var property in expression.Properties)
            {
                if (property is JTokenExpression jTokenExpression && jTokenExpression.Value.Type == JTokenType.String)
                {
                    // TODO handle special chars and generate array access instead
                    baseSyntax = new PropertyAccessSyntax(
                        baseSyntax,
                        SyntaxFactory.DotToken,
                        SyntaxFactory.CreateIdentifier(jTokenExpression.Value.Value<string>()!));
                }
                else
                {
                    baseSyntax = new ArrayAccessSyntax(
                        baseSyntax,
                        SyntaxFactory.LeftSquareToken,
                        ParseLanguageExpression(property),
                        SyntaxFactory.RightSquareToken);
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
            
            return SyntaxFactory.CreateStringLiteral(value);
        }

        private static IntegerLiteralSyntax ParseIntegerJToken(JValue value)
        {
            var longValue = value.Value<long>();
            return SyntaxFactory.CreateIntegerLiteral(longValue);
        }

        private SyntaxBase ParseJValue(JValue value)
            => value.Type switch {
                JTokenType.String => ParseString(value.ToString()),
                JTokenType.Uri => ParseString(value.ToString()),
                JTokenType.Integer => ParseIntegerJToken(value),
                JTokenType.Date => ParseString(value.ToString()),
                JTokenType.Float => ParseString(value.ToString()),
                JTokenType.Boolean =>  value.Value<bool>() ?
                    new BooleanLiteralSyntax(SyntaxFactory.TrueKeywordToken, true) :
                    new BooleanLiteralSyntax(SyntaxFactory.FalseKeywordToken, false),
                JTokenType.Null => new NullLiteralSyntax(SyntaxFactory.NullKeywordToken),
                _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
            };

        private ArraySyntax ParseJArray(JArray value)
        {
            var items = new List<SyntaxBase>();

            foreach (var item in value)
            {
                items.Add(ParseJToken(item));
            }

            return SyntaxFactory.CreateArray(items);
        }

        private ObjectSyntax ParseJObject(JObject jObject)
        {
            var properties = new List<ObjectPropertySyntax>();
            foreach (var (key, value) in jObject)
            {
                // here we're handling a property copy
                if (key == "copy" && value is JArray)
                {
                    foreach (var entry in value)
                    {
                        if (entry is not JObject copyProperty)
                        {
                            throw new ConversionFailedException($"Expected a copy object", entry);
                        }

                        var name = TemplateHelpers.AssertRequiredProperty(copyProperty, "name", "The copy object is missing a \"name\" property").ToString();
                        var count = TemplateHelpers.AssertRequiredProperty(copyProperty, "count", "The copy object is missing a \"count\" property");
                        var input = TemplateHelpers.AssertRequiredProperty(copyProperty, "input", "The copy object is missing a \"input\" property");

                        var objectVal = ProcessNamedCopySyntax(input, PropertyCopyLoopIndexVar, input => ParseJToken(input), count, name);

                        properties.Add(SyntaxFactory.CreateObjectProperty(name, objectVal));
                    }
                }
                else if (ExpressionsEngine.IsLanguageExpression(key))
                {
                    var keySyntax = ParseString(key);
                    if (keySyntax is not StringSyntax)
                    {
                        keySyntax = SyntaxFactory.CreateInterpolatedKey(keySyntax);
                    }
                    
                    var objectProperty = new ObjectPropertySyntax(keySyntax, SyntaxFactory.ColonToken, ParseJToken(value));
                    properties.Add(objectProperty);
                }
                else
                {
                    var objectProperty = SyntaxFactory.CreateObjectProperty(key, ParseJToken(value));
                    properties.Add(objectProperty);
                }
            }

            return SyntaxFactory.CreateObject(properties);
        }

        public ParameterDeclarationSyntax ParseParam(JProperty value)
        {
            var decoratorsAndNewLines = new List<SyntaxBase>();

            foreach (var parameterPropertyName in new[] { "minValue", "maxValue", "minLength", "maxLength", "allowedValues", "metadata" })
            {
                if (TryParseJToken(value.Value?[parameterPropertyName]) is SyntaxBase expression)
                {
                    var functionName = parameterPropertyName == "allowedValues" ? "allowed" : parameterPropertyName;

                    if (parameterPropertyName == "metadata" &&
                        expression is ObjectSyntax metadataObject &&
                        metadataObject.Properties.Any() &&
                        !metadataObject.Properties.Skip(1).Any() &&
                        metadataObject.Properties.Single() is ObjectPropertySyntax metadataProperty &&
                        metadataProperty.TryGetKeyText() == "description")
                    {
                        // Replace metadata decorator with description decorator if the metadata object only contains description.
                        functionName = "description";
                        expression = metadataProperty.Value;
                    }

                    decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(functionName, expression));
                    decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
                }
            }

            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ConversionFailedException($"Unable to locate 'type' for parameter '{value.Name}'", value);

            if (typeSyntax.TypeName == "securestring")
            {
                typeSyntax = new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, "string"));
                decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator("secure"));
                decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
            }

            if (typeSyntax.TypeName == "secureobject")
            {
                typeSyntax = new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, "object"));
                decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator("secure"));
                decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
            }

            // If there are decorators, insert a NewLine token at the beginning to make it more readable.
            var leadingNodes = decoratorsAndNewLines.Count > 0
                ? SyntaxFactory.NewlineToken.AsEnumerable().Concat(decoratorsAndNewLines)
                : decoratorsAndNewLines;

            SyntaxBase? modifier = TryParseJToken(value.Value?["defaultValue"]) is SyntaxBase defaultValue
                ? new ParameterDefaultValueSyntax(SyntaxFactory.AssignmentToken, defaultValue)
                : null;

            var identifier = nameResolver.TryLookupName(NameType.Parameter, value.Name) ?? throw new ConversionFailedException($"Unable to find parameter {value.Name}", value);

            return new ParameterDeclarationSyntax(
                leadingNodes,
                SyntaxFactory.CreateToken(TokenType.Identifier, "param"),
                SyntaxFactory.CreateIdentifier(identifier),
                typeSyntax,
                modifier);
        }

        public VariableDeclarationSyntax ParseVariable(JProperty value)
        {
            var identifier = nameResolver.TryLookupName(NameType.Variable, value.Name) ?? throw new ConversionFailedException($"Unable to find variable {value.Name}", value);

            return new VariableDeclarationSyntax(
                SyntaxFactory.CreateToken(TokenType.Identifier, "var"),
                SyntaxFactory.CreateIdentifier(identifier),
                SyntaxFactory.AssignmentToken,
                ParseJToken(value.Value));
        }

        private SyntaxBase GetModuleFilePath(JObject resource, string templateLink)
        {
            StringSyntax createFakeModulePath(string templateLinkExpression)
                => SyntaxFactory.CreateStringLiteralWithComment("?", $"TODO: replace with correct path to {templateLinkExpression}");

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
            return SyntaxFactory.CreateStringLiteral(filePath);
        }


        /// <summary>
        /// Used to generate a for-expression for a copy loop, where the copyIndex does not accept a 'name' parameter.
        /// </summary>
        public ForSyntax ProcessUnnamedCopySyntax<TToken>(TToken input, string indexIdentifier, Func<TToken, SyntaxBase> getSyntaxForInputFunc, JToken count)
            where TToken : JToken
        {
            return PerformScopedAction(() => {
                input = ExpressionHelpers.ReplaceFunctionExpressions(input, function => {
                    if (!StringComparer.OrdinalIgnoreCase.Equals(function.Function, "copyIndex"))
                    {
                        return;
                    }
    
                    if (function.Parameters.Length == 0)
                    {
                        // copyIndex() - replace with '<index>'
                        function.Function = "variables";
                        function.Parameters = new LanguageExpression[]
                        {
                            new JTokenExpression(indexIdentifier),
                        };
                    }
                    else if (function.Parameters.Length == 1 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == null) // exclude 'named' copyIndex - it does not apply to resources.
                    {
                        // copyIndex(<offset>) - replace with '<index> + <offset>'
                        var varExpression = new FunctionExpression(
                            "variables",
                            new LanguageExpression[]
                            {
                                new JTokenExpression(indexIdentifier),
                            },
                            Array.Empty<LanguageExpression>());

                        function.Function = "add";
                        function.Parameters = new LanguageExpression[]
                        {
                            varExpression,
                            function.Parameters[0],
                        };
                    }
                });

                return SyntaxFactory.CreateRangedForSyntax(indexIdentifier, ParseJToken(count), getSyntaxForInputFunc(input));
            }, new [] { indexIdentifier });
        }

        /// <summary>
        /// Used to generate a for-expression for a copy loop, where the copyIndex requires a 'name' parameter.
        /// </summary>
        public ForSyntax ProcessNamedCopySyntax<TToken>(TToken input, string indexIdentifier, Func<TToken, SyntaxBase> getSyntaxForInputFunc, JToken count, string name)
            where TToken : JToken
        {
            return PerformScopedAction(() => {
                input = ExpressionHelpers.ReplaceFunctionExpressions(input, function => {
                    if (!StringComparer.OrdinalIgnoreCase.Equals(function.Function, "copyIndex"))
                    {
                        return;
                    }

                    if (function.Parameters.Length == 1 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == name)
                    {
                        // copyIndex(<name>) - replace with '<index>'
                        function.Function = "variables";
                        function.Parameters = new LanguageExpression[]
                        {
                            new JTokenExpression(indexIdentifier),
                        };
                    }
                    else if (function.Parameters.Length == 2 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == name)
                    {
                        // copyIndex(<name>, <offset>) - replace with '<index> + <offset>'
                        var varExpression = new FunctionExpression(
                            "variables",
                            new LanguageExpression[]
                            {
                                new JTokenExpression(indexIdentifier),
                            },
                            Array.Empty<LanguageExpression>());

                        function.Function = "add";
                        function.Parameters = new LanguageExpression[]
                        {
                            varExpression,
                            function.Parameters[1],
                        };
                    }
                });

                return SyntaxFactory.CreateRangedForSyntax(indexIdentifier, ParseJToken(count), getSyntaxForInputFunc(input));
            }, new [] { indexIdentifier });
        }

        private (SyntaxBase body, IEnumerable<SyntaxBase> decorators) ProcessResourceCopy(JObject resource, Func<JObject, SyntaxBase> resourceBodyFunc)
        {
            if (TemplateHelpers.GetProperty(resource, "copy")?.Value is not JObject copyProperty)
            {
                return (resourceBodyFunc(resource), Enumerable.Empty<SyntaxBase>());
            }

            TemplateHelpers.AssertUnsupportedProperty(resource, "condition", "The 'copy' property is not supported in conjunction with the 'condition' property");

            var name = TemplateHelpers.AssertRequiredProperty(copyProperty, "name", "The copy object is missing a \"name\" property");
            var count = TemplateHelpers.AssertRequiredProperty(copyProperty, "count", "The copy object is missing a \"count\" property");

            var bodySyntax = ProcessUnnamedCopySyntax(resource, ResourceCopyLoopIndexVar, resource => resourceBodyFunc(resource), count);

            var decoratorAndNewLines = new List<SyntaxBase>();

            if(IsSerialMode(TemplateHelpers.GetProperty(copyProperty, "mode")?.Value, resource))
            {
                var batchSize = ParseBatchSize(TemplateHelpers.GetProperty(copyProperty, "batchSize")?.Value, resource);
                decoratorAndNewLines.Add(SyntaxFactory.CreateDecorator("batchSize", batchSize));
                decoratorAndNewLines.Add(SyntaxFactory.NewlineToken);
            }

            return (bodySyntax, decoratorAndNewLines);
        }

        private static IntegerLiteralSyntax ParseBatchSize(JToken? batchSize, JObject resource)
        {
            if(batchSize is null)
            {
                // default batch size is 1
                return SyntaxFactory.CreateIntegerLiteral(1);
            }

            if(batchSize is not JValue value || batchSize.Type != JTokenType.Integer)
            {
                throw new ConversionFailedException("Expected the value of the \"batchSize\" property to be an integer.", resource);
            }

            return ParseIntegerJToken(value);
        }

        private static bool IsSerialMode(JToken? mode, JObject resource)
        {
            const string Serial = "serial";
            const string Parallel = "parallel";

            if (mode is null)
            {
                // default mode is parallel
                return false;
            }

            if (mode.Type != JTokenType.String)
            {
                throw new ConversionFailedException("Expected the value of the \"mode\" property in a property copy object to be a string.", resource);
            }

            var value = mode.ToString();
            
            if (string.Equals(value, Serial, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
                        
            if (string.Equals(value, Parallel, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new ConversionFailedException($"Expected the value of the \"mode\" property to be either \"{Serial}\" or \"{Parallel}\", but the provided value was \"{value}\".", resource);
        }

        private SyntaxBase ProcessCondition(JObject resource, SyntaxBase body)
        {
            JProperty? conditionProperty = TemplateHelpers.GetProperty(resource, "condition");

            if (conditionProperty == null)
            {
                return body;
            }

            SyntaxBase conditionExpression = ParseJToken(conditionProperty.Value);

            if (conditionExpression is not ParenthesizedExpressionSyntax)
            {
                conditionExpression = new ParenthesizedExpressionSyntax(
                    SyntaxFactory.LeftParenToken,
                    conditionExpression,
                    SyntaxFactory.RightParenToken);
            }

            return new IfConditionSyntax(
                SyntaxFactory.CreateToken(TokenType.Identifier, "if"),
                conditionExpression,
                body);
        }

        private SyntaxBase? ProcessDependsOn(IReadOnlyDictionary<string, string> copyResourceLookup, JObject resource)
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
                var resourceRef = TryLookupResource(entryExpression) ?? copyResourceLookup.TryGetValue(entryString);

                SyntaxBase syntaxEntry;
                if (resourceRef != null)
                {
                    syntaxEntry = new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resourceRef));
                }
                else
                {
                    // we can't output anything intelligent - convert the expression and move on.
                    syntaxEntry = ParseJToken(entry);
                }

                
                syntaxItems.Add(syntaxEntry);
            }

            return SyntaxFactory.CreateArray(syntaxItems);
        }

        private SyntaxBase? TryModuleGetScopeProperty(JObject resource)
        {
            var subscriptionId = TemplateHelpers.GetProperty(resource, "subscriptionId");
            var resourceGroup = TemplateHelpers.GetProperty(resource, "resourceGroup");

            switch (subscriptionId, resourceGroup)
            {
                case (JProperty subId, JProperty rgName):
                    return SyntaxFactory.CreateFunctionCall("resourceGroup", ParseJToken(subId.Value), ParseJToken(rgName.Value));
                case (null, JProperty rgName):
                    return SyntaxFactory.CreateFunctionCall("resourceGroup", ParseJToken(rgName.Value));
                case (JProperty subId, null):
                    return SyntaxFactory.CreateFunctionCall("subscription", ParseJToken(subId.Value));
            }

            return null;
        }

        private SyntaxBase ParseModule(IReadOnlyDictionary<string, string> copyResourceLookup, JObject resource, string typeString, string nameString)
        {
            var expectedProps = new HashSet<string>(new [] {
                "name",
                "type",
                "apiVersion",
                "location",
                "properties",
                "dependsOn",
                "comments",
            }, StringComparer.OrdinalIgnoreCase);

            var propsToOmit = new HashSet<string>(new [] {
                "condition",
                "copy",
                "resourceGroup",
                "subscriptionId",
            }, StringComparer.OrdinalIgnoreCase);

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

            var (body, decorators) = ProcessResourceCopy(resource, x => ProcessModuleBody(copyResourceLookup, x, nameString));
            var value = ProcessCondition(resource, body);

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
                    decorators,
                    SyntaxFactory.CreateToken(TokenType.Identifier, "module"),
                    SyntaxFactory.CreateIdentifier(identifier),
                    SyntaxFactory.CreateStringLiteral(filePath),
                    SyntaxFactory.AssignmentToken,
                    value);
            }

            var pathProperty = TemplateHelpers.GetNestedProperty(resource, "properties", "templateLink", "uri") ??
                TemplateHelpers.GetNestedProperty(resource, "properties", "templateLink", "relativePath");

            if (pathProperty?.Value<string>() is not string templatePathString)
            {
                throw new ConversionFailedException($"Unable to find \"uri\" or \"relativePath\" properties under {resource["name"]}.properties.templateLink for linked template.", resource);
            }

            return new ModuleDeclarationSyntax(
                decorators,
                SyntaxFactory.CreateToken(TokenType.Identifier, "module"),
                SyntaxFactory.CreateIdentifier(identifier),
                GetModuleFilePath(resource, templatePathString),
                SyntaxFactory.AssignmentToken,
                value);
        }

        private ObjectSyntax ProcessModuleBody(IReadOnlyDictionary<string, string> copyResourceLookup, JObject resource, string nameString)
        {
            var parameters = (resource["properties"]?["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>();
            var paramProperties = new List<ObjectPropertySyntax>();
            foreach (var param in parameters)
            {
                paramProperties.Add(SyntaxFactory.CreateObjectProperty(param.Name, ParseJToken(param.Value["value"])));
            }

            var properties = new List<ObjectPropertySyntax>();
            properties.Add(SyntaxFactory.CreateObjectProperty("name", ParseJToken(nameString)));

            var scope = TryModuleGetScopeProperty(resource);
            if (scope is not null)
            {
                properties.Add(SyntaxFactory.CreateObjectProperty("scope", scope));
            }

            properties.Add(SyntaxFactory.CreateObjectProperty("params", SyntaxFactory.CreateObject(paramProperties)));

            var dependsOn = ProcessDependsOn(copyResourceLookup, resource);
            if (dependsOn != null)
            {
                properties.Add(SyntaxFactory.CreateObjectProperty("dependsOn", dependsOn));
            }

            return SyntaxFactory.CreateObject(properties);
        }

        private SyntaxBase? TryGetResourceScopeProperty(JObject resource)
        {
            if (TemplateHelpers.GetProperty(resource, "scope") is not JProperty scopeProperty)
            {
                return null;
            }

            var scopeExpression = ExpressionHelpers.ParseExpression(scopeProperty.Value.Value<string>());
            if (TryLookupResource(scopeExpression) is string resourceName)
            {
                return SyntaxFactory.CreateIdentifier(resourceName);
            }

            if (TryParseStringExpression(scopeExpression) is SyntaxBase parsedSyntax)
            {
                return parsedSyntax;                    
            }

            throw new ConversionFailedException($"Parsing failed for property value {scopeProperty}", scopeProperty);
        }

        public SyntaxBase ParseResource(IReadOnlyDictionary<string, string> copyResourceLookup, JToken token)
        {
            var resource = (token as JObject) ?? throw new ConversionFailedException("Incorrect resource format", token);

            // mandatory properties
            var (typeString, nameString, apiVersionString) = TemplateHelpers.ParseResource(resource);

            if (StringComparer.OrdinalIgnoreCase.Equals(typeString, "Microsoft.Resources/deployments"))
            {
                return ParseModule(copyResourceLookup, resource, typeString, nameString);
            }
            
            var (body, decorators) = ProcessResourceCopy(resource, x => ProcessResourceBody(copyResourceLookup, x));
            var value = ProcessCondition(resource, body);

            var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");

            return new ResourceDeclarationSyntax(
                decorators,
                SyntaxFactory.CreateToken(TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(identifier),
                ParseString($"{typeString}@{apiVersionString}"),
                null,
                SyntaxFactory.AssignmentToken,
                value);
        }

        private ObjectSyntax ProcessResourceBody(IReadOnlyDictionary<string, string> copyResourceLookup, JObject resource)
        {
            var expectedResourceProps = new HashSet<string>(new[]
            {
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
                "scope",
            }, StringComparer.OrdinalIgnoreCase);

            var resourcePropsToOmit = new HashSet<string>(new[]
            {
                "condition",
                "copy",
                "type",
                "apiVersion",
                "dependsOn",
                "comments",
                "scope",
            }, StringComparer.OrdinalIgnoreCase);

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

                topLevelProperties.Add(SyntaxFactory.CreateObjectProperty(prop.Name, valueSyntax));
            }

            var scope = TryGetResourceScopeProperty(resource);
            if (scope is not null)
            {
                topLevelProperties.Add(SyntaxFactory.CreateObjectProperty("scope", scope));
            }

            var dependsOn = ProcessDependsOn(copyResourceLookup, resource);
            if (dependsOn != null)
            {
                topLevelProperties.Add(SyntaxFactory.CreateObjectProperty("dependsOn", dependsOn));
            }

            return SyntaxFactory.CreateObject(topLevelProperties);
        }

        public OutputDeclarationSyntax ParseOutput(JProperty value)
        {
            var typeSyntax = TryParseType(value.Value?["type"]) ?? throw new ConversionFailedException($"Unable to locate 'type' for output '{value.Name}'", value);
            var identifier = nameResolver.TryLookupName(NameType.Output, value.Name) ?? throw new ConversionFailedException($"Unable to find output {value.Name}", value);

            SyntaxBase valueSyntax;
            var copyVal = value.Value?["copy"];
            if (copyVal is null)
            {
                valueSyntax = ParseJToken(value.Value?["value"]);
            }
            else
            {
                if (copyVal is not JObject copyProperty)
                {
                    throw new ConversionFailedException($"Expected a copy object", copyVal);
                }

                var count = TemplateHelpers.AssertRequiredProperty(copyProperty, "count", "The copy object is missing a \"count\" property").ToString();
                var input = TemplateHelpers.AssertRequiredProperty(copyProperty, "input", "The copy object is missing an \"input\" property");

                valueSyntax = ProcessUnnamedCopySyntax(input, ResourceCopyLoopIndexVar, value => ParseJToken(value), count);
            }

            return new OutputDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(TokenType.Identifier, "output"),
                SyntaxFactory.CreateIdentifier(identifier),
                typeSyntax,
                SyntaxFactory.AssignmentToken,
                valueSyntax);
        }

        private TargetScopeSyntax? ParseTargetScope(JObject template)
        {
            var schema = TemplateHelpers.AssertRequiredProperty(template, "$schema", "Unable to find a template property named $schema.");
            if (!Uri.TryCreate(schema.ToString(), UriKind.Absolute, out var schemaUri))
            {
                schemaUri = null;
            }

            switch (schemaUri?.AbsolutePath)
            {
                case "/schemas/2019-08-01/tenantDeploymentTemplate.json":
                    return new TargetScopeSyntax(
                        SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxFactory.AssignmentToken,
                        SyntaxFactory.CreateStringLiteral("tenant"));
                case "/schemas/2019-08-01/managementGroupDeploymentTemplate.json":
                    return new TargetScopeSyntax(
                        SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxFactory.AssignmentToken,
                        SyntaxFactory.CreateStringLiteral("managementGroup"));
                case "/schemas/2018-05-01/subscriptionDeploymentTemplate.json":
                    return new TargetScopeSyntax(
                        SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                        SyntaxFactory.AssignmentToken,
                        SyntaxFactory.CreateStringLiteral("subscription"));
                case "/schemas/2014-04-01-preview/deploymentTemplate.json":
                case "/schemas/2015-01-01/deploymentTemplate.json":
                case "/schemas/2019-04-01/deploymentTemplate.json":
                    // targetScope not required for rg-level templates as 'resourceGroup' is the default
                    return null;
            }

            throw new ConversionFailedException($"$schema value \"{schema}\" did not match any of the known ARM template deployment schemas.", schema);
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
                    syntaxes.Add(SyntaxFactory.NewlineToken);
                }
            }

            if (syntaxesToAdd.Any())
            {
                // always add a new line after a block
                syntaxes.Add(SyntaxFactory.NewlineToken);
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

            var copyResourceLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources.OfType<JObject>())
            {
                var loopName = TemplateHelpers.GetNestedProperty(resource, "copy", "name")?.ToString();
                if (loopName is null)
                {
                    continue;
                }

                var (typeString, nameString, _) = TemplateHelpers.ParseResource(resource);
                if (nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) is {} resourceRef)
                {
                    copyResourceLookup[loopName] = resourceRef;
                }
            }

            AddSyntaxBlock(statements, parameters.Select(ParseParam), false);
            AddSyntaxBlock(statements, variables.Select(ParseVariable), false);
            AddSyntaxBlock(statements, flattenedResources.Select(resource => ParseResource(copyResourceLookup, resource)), true);
            AddSyntaxBlock(statements, outputs.Select(ParseOutput), false);

            return new ProgramSyntax(
                statements.SelectMany(x => new [] { x, SyntaxFactory.NewlineToken}),
                SyntaxFactory.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<IDiagnostic>()
            );
        }

        private T PerformScopedAction<T>(Func<T> action, IEnumerable<string> scopeVariables)
        {
            var prevNameResolver = nameResolver;

            try
            {
                nameResolver = new ScopedNamingResolver(prevNameResolver, scopeVariables);

                return action();
            }
            finally
            {
                nameResolver = prevNameResolver;
            }
        }
    }
}
