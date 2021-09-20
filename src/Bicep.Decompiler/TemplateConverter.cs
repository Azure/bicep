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
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.BicepHelpers;
using Bicep.Decompiler.Exceptions;
using Newtonsoft.Json;
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
        private readonly Uri bicepFileUri;
        private readonly JObject template;
        private readonly Dictionary<ModuleDeclarationSyntax, Uri> jsonTemplateUrisByModule;

        private TemplateConverter(Workspace workspace, IFileResolver fileResolver, Uri bicepFileUri, JObject template, Dictionary<ModuleDeclarationSyntax, Uri> jsonTemplateUrisByModule)
        {
            this.workspace = workspace;
            this.fileResolver = fileResolver;
            this.bicepFileUri = bicepFileUri;
            this.template = template;
            this.nameResolver = new UniqueNamingResolver();
            this.jsonTemplateUrisByModule = jsonTemplateUrisByModule;
        }

        public static (ProgramSyntax programSyntax, IReadOnlyDictionary<ModuleDeclarationSyntax, Uri> jsonTemplateUrisByModule) DecompileTemplate(
            Workspace workspace,
            IFileResolver fileResolver,
            Uri bicepFileUri,
            string content)
        {
            var instance = new TemplateConverter(
                workspace,
                fileResolver,
                bicepFileUri,
                JObject.Parse(content, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                    LineInfoHandling = LineInfoHandling.Load,
                }),
                new());

            return (instance.Parse(), instance.jsonTemplateUrisByModule);
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

            foreach (var (name, value, _) in GetVariables(variables))
            {
                if (nameResolver.TryRequestName(NameType.Variable, name) == null)
                {
                    throw new ConversionFailedException($"Unable to pick unique name for variable {name}", value);
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
            => value switch
            {
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
                JTokenType.Boolean => expression.Value.Value<bool>() ?
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

                var leftParameter = expression.Parameters[0];
                var rightParameter = expression.Parameters[1];
                // if token is = or != check to see if they are insensitive conditions i.e =~ or !~
                if (binaryTokenType is TokenType.Equals || binaryTokenType is TokenType.NotEquals)
                {
                    if(leftParameter is FunctionExpression leftFunctionExpression &&
                        rightParameter is FunctionExpression rightFunctionExpression &&
                        leftFunctionExpression.Function == "toLower" &&
                        rightFunctionExpression.Function == "toLower")
                    {
                        leftParameter = leftFunctionExpression.Parameters[0];
                        rightParameter = rightFunctionExpression.Parameters[0];
                        binaryTokenType = binaryTokenType == TokenType.Equals ? TokenType.EqualsInsensitive : TokenType.NotEqualsInsensitive;
                        binaryOperator = Operators.TokenTypeToBinaryOperator[binaryTokenType];
                    }
                }

                var binaryOperation = new BinaryOperationSyntax(
                    ParseLanguageExpression(leftParameter),
                    SyntaxFactory.CreateToken(binaryTokenType, Operators.BinaryOperatorToText[binaryOperator]),
                    ParseLanguageExpression(rightParameter));

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

            if (SyntaxHelpers.TryGetEmptyFunctionKeywordReplacement(expression.Function) is TokenType keywordTokenType)
            {
                if (expression.Parameters.Any())
                {
                    throw new ArgumentException($"Expected 0 parameters for function {expression.Function}");
                }

                if (expression.Properties.Any())
                {
                    throw new ArgumentException($"Expected 0 properties for function {expression.Function}");
                }

                syntax = SyntaxFactory.CreateToken(keywordTokenType);
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

                // Check to see if the inner expression is also a function and if it is equals we can
                // simplify the expression from (!(a == b)) to (a != b)
                if (expression.Parameters[0] is FunctionExpression functionExpression){
                    if (StringComparer.OrdinalIgnoreCase.Equals(functionExpression.Function, "equals")){
                        return TryReplaceBannedFunction(
                            new FunctionExpression("notEquals", functionExpression.Parameters, functionExpression.Properties),
                        out syntax);
                    }
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
            var expressions = new List<SyntaxBase>();
            for (var i = 0; i < functionExpression.Parameters.Length; i++)
            {
                var isEnd = (i == functionExpression.Parameters.Length - 1);

                // FlattenStringOperations will have already simplified the concat statement to the point where we know there won't be two string literals side-by-side.
                // We can use that knowledge to simplify this logic.

                if (functionExpression.Parameters[i] is JTokenExpression jTokenExpression)
                {
                    values.Add(jTokenExpression.Value.ToString());

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
                    values.Add("");
                }

                expressions.Add(ParseLanguageExpression(functionExpression.Parameters[i]));
                isEnd = (i == functionExpression.Parameters.Length - 1);

                if (isEnd)
                {
                    // always make sure we end with a string token
                    values.Add("");
                }
            }

            return SyntaxFactory.CreateString(values, expressions);
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
                            // reference(resourceId(<...>))
                            // check if it's a reference to a known resource
                            if (TryLookupResource(expression.Parameters[0]) is { } resourceName)
                            {
                                baseSyntax = new PropertyAccessSyntax(
                                    new VariableAccessSyntax(SyntaxFactory.CreateIdentifier(resourceName)),
                                    SyntaxFactory.DotToken,
                                    SyntaxFactory.CreateIdentifier("properties"));
                            }
                        }
                        else if (expression.Parameters.Length == 1)
                        {
                            // reference(<name>)
                            // let's try looking the name up directly
                            if (TryLookupResource(expression.Parameters[0]) is { } resourceName)
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

        private SyntaxBase ParseString(string? value, IJsonLineInfo lineInfo)
        {
            try
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
            catch (Exception exception)
            {
                throw new ConversionFailedException(exception.Message, lineInfo, exception);
            }
        }

        private static IntegerLiteralSyntax ParseIntegerJToken(JValue value)
        {
            var longValue = value.Value<long>();
            return SyntaxFactory.CreateIntegerLiteral(longValue);
        }

        private SyntaxBase ParseJValue(JValue value)
            => value.Type switch
            {
                JTokenType.String => ParseString(value.ToString(), value),
                JTokenType.Uri => ParseString(value.ToString(), value),
                JTokenType.Integer => ParseIntegerJToken(value),
                JTokenType.Date => ParseString(value.ToString(), value),
                JTokenType.Float => ParseString(value.ToString(), value),
                JTokenType.Boolean => value.Value<bool>() ?
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
            foreach (var property in jObject.Properties())
            {
                var key = property.Name;
                var value = property.Value;

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
                    var keySyntax = ParseString(key, property);
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

            // Metadata/description should be first so users see what the parameter is for before
            // seeing informationi such as a long list of allowed values
            foreach (var parameterPropertyName in new[] { "metadata", "minValue", "maxValue", "minLength", "maxLength", "allowedValues" })
            {
                if (TryParseJToken(value.Value?[parameterPropertyName]) is SyntaxBase expression)
                {
                    var functionName = parameterPropertyName == "allowedValues" ? LanguageConstants.ParameterAllowedPropertyName : parameterPropertyName;

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

            switch (typeSyntax.TypeName)
            {
                case "securestring":
                    typeSyntax = new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, "string"));
                    decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(LanguageConstants.ParameterSecurePropertyName));
                    decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
                    break;
                case "secureobject":
                    typeSyntax = new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, "object"));
                    decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(LanguageConstants.ParameterSecurePropertyName));
                    decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
                    break;
                case "__bicep_replace":
                    var fixupToken = SyntaxHelpers.CreatePlaceholderToken(TokenType.Identifier, "TODO: fill in correct type");
                    typeSyntax = new TypeSyntax(fixupToken);
                    break;
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

        public VariableDeclarationSyntax ParseVariable(string name, JToken value, bool isCopyVariable)
        {
            var identifier = nameResolver.TryLookupName(NameType.Variable, name) ?? throw new ConversionFailedException($"Unable to find variable {name}", value);

            SyntaxBase variableValue;
            if (isCopyVariable)
            {
                if (value is not JObject copyProperty)
                {
                    throw new ConversionFailedException($"Expected a copy object", value);
                }

                var count = TemplateHelpers.AssertRequiredProperty(copyProperty, "count", "The copy object is missing a \"count\" property");
                var input = TemplateHelpers.AssertRequiredProperty(copyProperty, "input", "The copy object is missing a \"input\" property");

                variableValue = ProcessNamedCopySyntax(input, ResourceCopyLoopIndexVar, input => ParseJToken(input), count, name);
            }
            else
            {
                variableValue = ParseJToken(value);
            }

            return new VariableDeclarationSyntax(
                SyntaxFactory.CreateToken(TokenType.Identifier, "var"),
                SyntaxFactory.CreateIdentifier(identifier),
                SyntaxFactory.AssignmentToken,
                variableValue);
        }

        private (SyntaxBase moduleFilePathStringLiteral, Uri? jsonTemplateUri) GetModuleFilePath(string templateLink)
        {
            StringSyntax createFakeModulePath(string templateLinkExpression)
                => SyntaxFactory.CreateStringLiteralWithComment("?", $"TODO: replace with correct path to {templateLinkExpression}");

            var templateLinkExpression = InlineVariables(ExpressionHelpers.ParseExpression(templateLink));

            var nestedRelativePath = ExpressionHelpers.TryGetLocalFilePathForTemplateLink(templateLinkExpression);
            if (nestedRelativePath is null)
            {
                // return the original expression so that the author can fix it up rather than failing
                return (createFakeModulePath(templateLink), null);
            }

            var nestedUri = fileResolver.TryResolveFilePath(bicepFileUri, nestedRelativePath);
            if (nestedUri is null || !fileResolver.TryRead(nestedUri, out _, out _))
            {
                // return the original expression so that the author can fix it up rather than failing
                return (createFakeModulePath(templateLink), null);
            }

            var existIdenticalUrisWithDifferentExtensions = jsonTemplateUrisByModule.Values.Any(uri =>
                uri != nestedUri && PathHelper.RemoveExtension(uri) == PathHelper.RemoveExtension(nestedUri));

            /*
             * If there exist another nested template with the same path and filename but a different extension,
             * append ".bicep" to path of the current nested template to avoid the generate bicep files overwrite each other.
             * Otherwise, change the extenstion of the nested template to ".bicep".
             */
            var moduleFilePath = (existIdenticalUrisWithDifferentExtensions
                ? nestedRelativePath + ".bicep"
                : Path.ChangeExtension(nestedRelativePath, ".bicep")).Replace("\\", "/");

            return (SyntaxFactory.CreateStringLiteral(moduleFilePath), nestedUri);
        }


        /// <summary>
        /// Used to generate a for-expression for a copy loop, where the copyIndex does not accept a 'name' parameter.
        /// </summary>
        public ForSyntax ProcessUnnamedCopySyntax<TToken>(TToken input, string indexIdentifier, Func<TToken, SyntaxBase> getSyntaxForInputFunc, JToken count)
            where TToken : JToken
        {
            // Give it a fake name for now - it'll be replaced anyway.
            // This avoids a lot of code duplication to be able to handle the unamed copy loop.

            return ProcessNamedCopySyntax(input, indexIdentifier, getSyntaxForInputFunc, count, "__BICEP_REPLACE");
        }

        /// <summary>
        /// Used to generate a for-expression for a copy loop, where the copyIndex requires a 'name' parameter.
        /// </summary>
        public ForSyntax ProcessNamedCopySyntax<TToken>(TToken input, string indexIdentifier, Func<TToken, SyntaxBase> getSyntaxForInputFunc, JToken count, string name)
            where TToken : JToken
        {
            return PerformScopedAction(() =>
            {
                // simplify things by converting unnamed -> named copyIndex expressions
                // this avoids the scenario with a nested copy loop referring ambiguously to the outer index with copyIndex()
                input = JTokenHelpers.RewriteExpressions(input, expression =>
                {
                    if (ExpressionHelpers.TryGetNamedFunction(expression, "copyIndex") is { } function)
                    {
                        if (function.Parameters.Length == 0)
                        {
                            // copyIndex() -> copyIndex(<name>)
                            return new FunctionExpression(
                                "copyIndex",
                                new[] { new JTokenExpression(name) },
                                function.Properties);
                        }
                        else if (function.Parameters.Length == 1 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == null)
                        {
                            // we've got a non-string param - it must be the index!
                            // copyIndex(<index>) -> copyIndex(<name>, <index>)
                            return new FunctionExpression(
                                "copyIndex",
                                new[] { new JTokenExpression(name), function.Parameters[0] },
                                function.Properties);
                        }
                    }

                    return expression;
                });

                input = JTokenHelpers.RewriteExpressions(input, expression =>
                {
                    if (expression is not FunctionExpression function || !StringComparer.OrdinalIgnoreCase.Equals(function.Function, "copyIndex"))
                    {
                        return expression;
                    }

                    if (function.Parameters.Length == 1 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == name)
                    {
                        // copyIndex(<name>) - replace with '<index>'
                        return new FunctionExpression(
                            "variables",
                            new[] { new JTokenExpression(indexIdentifier) },
                            function.Properties);
                    }
                    else if (function.Parameters.Length == 2 && ExpressionHelpers.TryGetStringValue(function.Parameters[0]) == name)
                    {
                        // copyIndex(<name>, <offset>) - replace with '<index> + <offset>'
                        var varExpression = new FunctionExpression(
                            "variables",
                            new[] { new JTokenExpression(indexIdentifier), },
                            Array.Empty<LanguageExpression>());

                        return new FunctionExpression(
                            "add",
                            new[]
                            {
                                varExpression,
                                function.Parameters[1],
                            },
                            function.Properties);
                    }

                    return expression;
                });

                return SyntaxFactory.CreateRangedForSyntax(indexIdentifier, ParseJToken(count), getSyntaxForInputFunc(input));
            }, new[] { indexIdentifier });
        }

        private (SyntaxBase body, IEnumerable<SyntaxBase> decorators) ProcessResourceCopy(JObject resource, Func<JObject, SyntaxBase> resourceBodyFunc)
        {
            if (TemplateHelpers.GetProperty(resource, "copy")?.Value is not JObject copyProperty)
            {
                return (resourceBodyFunc(resource), Enumerable.Empty<SyntaxBase>());
            }

            var name = TemplateHelpers.AssertRequiredProperty(copyProperty, "name", "The copy object is missing a \"name\" property").ToString();
            var count = TemplateHelpers.AssertRequiredProperty(copyProperty, "count", "The copy object is missing a \"count\" property");

            var bodySyntax = ProcessNamedCopySyntax(resource, ResourceCopyLoopIndexVar, resource => resourceBodyFunc(resource), count, name);

            var decoratorAndNewLines = new List<SyntaxBase>();

            if (IsSerialMode(TemplateHelpers.GetProperty(copyProperty, "mode")?.Value, resource))
            {
                var batchSize = ParseBatchSize(TemplateHelpers.GetProperty(copyProperty, "batchSize")?.Value, resource);
                decoratorAndNewLines.Add(SyntaxFactory.CreateDecorator("batchSize", batchSize));
                decoratorAndNewLines.Add(SyntaxFactory.NewlineToken);
            }

            return (bodySyntax, decoratorAndNewLines);
        }

        private static IntegerLiteralSyntax ParseBatchSize(JToken? batchSize, JObject resource)
        {
            if (batchSize is null)
            {
                // default batch size is 1
                return SyntaxFactory.CreateIntegerLiteral(1);
            }

            if (batchSize is not JValue value || batchSize.Type != JTokenType.Integer)
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
            if (body is ForSyntax)
            {
                // condition within the loop has already been processed
                return body;
            }

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
            var expectedProps = new HashSet<string>(new[] {
                "name",
                "type",
                "apiVersion",
                "location",
                "properties",
                "dependsOn",
                "comments",
            }, StringComparer.OrdinalIgnoreCase);

            var propsToOmit = new HashSet<string>(new[] {
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

            var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");

            var nestedProperties = TemplateHelpers.GetNestedProperty(resource, "properties");
            var nestedTemplate = TemplateHelpers.GetNestedProperty(resource, "properties", "template");
            if (nestedProperties is not null && nestedTemplate is not null)
            {
                if (nestedTemplate is not JObject nestedTemplateObject)
                {
                    throw new ConversionFailedException($"Expected template object for {typeString} {nameString}", nestedTemplate);
                }

                var expressionEvaluationScope = TemplateHelpers.GetNestedProperty(resource, "properties", "expressionEvaluationOptions", "scope")?.ToString();
                if (!StringComparer.OrdinalIgnoreCase.Equals(expressionEvaluationScope, "inner"))
                {
                    if (TemplateHelpers.GetNestedProperty(nestedTemplateObject, "parameters") is { } existingParameters &&
                        existingParameters.Children().Any())
                    {
                        throw new ConversionFailedException($"Outer-scoped nested templates cannot contain parameters", existingParameters);
                    }

                    var (rewrittenTemplate, parameters) = TemplateHelpers.ConvertNestedTemplateInnerToOuter(nestedTemplateObject);

                    if (TemplateHelpers.GetNestedProperty(rewrittenTemplate, "parameters") is not JObject rewrittenParameters)
                    {
                        rewrittenParameters = new JObject();
                        rewrittenTemplate["parameters"] = rewrittenParameters;
                    }

                    foreach (var parameter in parameters.Keys)
                    {
                        if (TemplateHelpers.GetNestedProperty(template, "parameters", parameter) is { } parentTemplateParam &&
                            parentTemplateParam.DeepClone() is JObject nestedParam)
                        {
                            rewrittenParameters[parameter] = nestedParam;
                            TemplateHelpers.RemoveNestedProperty(nestedParam, "defaultValue");
                        }
                        else
                        {
                            rewrittenParameters[parameter] = new JObject
                            {
                                ["type"] = parameters[parameter].type,
                            };
                        }
                    }

                    nestedTemplateObject = rewrittenTemplate;
                    nestedProperties["template"] = rewrittenTemplate;
                    nestedProperties["parameters"] = new JObject(parameters.Select(x => new JProperty(
                        x.Key,
                        new JObject
                        {
                            ["value"] = ExpressionsEngine.SerializeExpression(x.Value.expression),
                        })));
                }

                var (nestedBody, nestedDecorators) = ProcessResourceCopy(resource, x => ProcessModuleBody(copyResourceLookup, x));
                var nestedValue = ProcessCondition(resource, nestedBody);

                var filePath = $"./nested_{identifier}.bicep";
                var nestedModuleUri = fileResolver.TryResolveFilePath(bicepFileUri, filePath) ?? throw new ConversionFailedException($"Unable to module uri for {typeString} {nameString}", nestedTemplate);
                if (workspace.TryGetSourceFile(nestedModuleUri, out _))
                {
                    throw new ConversionFailedException($"Unable to generate duplicate module to path ${nestedModuleUri} for {typeString} {nameString}", nestedTemplate);
                }

                var nestedConverter = new TemplateConverter(workspace, fileResolver, nestedModuleUri, nestedTemplateObject, this.jsonTemplateUrisByModule);
                var nestedBicepFile = new BicepFile(nestedModuleUri, ImmutableArray<int>.Empty, nestedConverter.Parse());
                workspace.UpsertSourceFile(nestedBicepFile);

                return new ModuleDeclarationSyntax(
                    nestedDecorators,
                    SyntaxFactory.CreateToken(TokenType.Identifier, LanguageConstants.ModuleKeyword),
                    SyntaxFactory.CreateIdentifier(identifier),
                    SyntaxFactory.CreateStringLiteral(filePath),
                    SyntaxFactory.AssignmentToken,
                    nestedValue);
            }

            var pathProperty = TemplateHelpers.GetNestedProperty(resource, "properties", "templateLink", "uri") ??
                TemplateHelpers.GetNestedProperty(resource, "properties", "templateLink", "relativePath");

            if (pathProperty?.Value<string>() is not string templatePathString)
            {
                throw new ConversionFailedException($"Unable to find \"uri\" or \"relativePath\" properties under {resource["name"]}.properties.templateLink for linked template.", resource);
            }

            var (body, decorators) = ProcessResourceCopy(resource, x => ProcessModuleBody(copyResourceLookup, x));
            var value = ProcessCondition(resource, body);

            var (modulePath, jsonTemplateUri) = GetModuleFilePath(templatePathString);
            var module = new ModuleDeclarationSyntax(
                decorators,
                SyntaxFactory.CreateToken(TokenType.Identifier, LanguageConstants.ModuleKeyword),
                SyntaxFactory.CreateIdentifier(identifier),
                modulePath,
                SyntaxFactory.AssignmentToken,
                value);

            /*
             * We need to save jsonTemplateUri because it may not necessarily end with .json extension.
             * When decompiling the module, jsonTemplateUri will be used to load the JSON template file.
             */
            if (jsonTemplateUri is not null)
            {
                this.jsonTemplateUrisByModule[module] = jsonTemplateUri;
            }

            return module;
        }

        private ObjectSyntax ProcessModuleBody(IReadOnlyDictionary<string, string> copyResourceLookup, JObject resource)
        {
            var parameters = (resource["properties"]?["parameters"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>();
            var paramProperties = new List<ObjectPropertySyntax>();
            foreach (var param in parameters)
            {
                if (param.Value["reference"] is {} referenceValue)
                {
                    throw new ConversionFailedException($"Failed to convert parameter \"{param.Name}\": KeyVault secret references are not currently supported by the decompiler.", referenceValue);
                }

                paramProperties.Add(SyntaxFactory.CreateObjectProperty(param.Name, ParseJToken(param.Value["value"])));
            }

            var properties = new List<ObjectPropertySyntax>();
            var nameProperty = TemplateHelpers.GetProperty(resource, "name");
            properties.Add(SyntaxFactory.CreateObjectProperty("name", ParseJToken(nameProperty?.Value)));

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

            var scopeExpression = ExpressionHelpers.ParseExpression(scopeProperty.Value.ToString());
            if (scopeExpression is JTokenExpression value && string.Equals(value.Value.ToString(), "/", StringComparison.OrdinalIgnoreCase))
            {
                // tenant scope resources can be deployed from any other scope as long as the "scope" property is set to "/"
                // the bicep equivalent is "scope: tenant()"
                return SyntaxFactory.CreateFunctionCall("tenant");
            }

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

            var (value, decorators) = ProcessResourceCopy(resource, resource =>
            {
                var body = ProcessResourceBody(copyResourceLookup, resource);

                return ProcessCondition(resource, body);
            });

            var identifier = nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) ?? throw new ArgumentException($"Unable to find resource {typeString} {nameString}");

            return new ResourceDeclarationSyntax(
                decorators,
                SyntaxFactory.CreateToken(TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(identifier),
                SyntaxFactory.CreateStringLiteral($"{typeString}@{apiVersionString}"),
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
            var scope = TryGetResourceScopeProperty(resource);
            if (scope is not null)
            {
                topLevelProperties.Add(SyntaxFactory.CreateObjectProperty("scope", scope));
            }

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

        private static IEnumerable<(string name, JToken value, bool isCopyVariable)> GetVariables(IEnumerable<JProperty> variables)
        {
            var nonCopyVariables = variables.Where(x => !StringComparer.OrdinalIgnoreCase.Equals(x.Name, "copy"));
            foreach (var nonCopyVariable in nonCopyVariables)
            {
                yield return (nonCopyVariable.Name, nonCopyVariable.Value, false);
            }

            var copyVariables = variables.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, "copy"))?.Value as JArray;
            foreach (var copyVariable in copyVariables ?? Enumerable.Empty<JToken>())
            {
                if (copyVariable is not JObject variableObject)
                {
                    throw new ConversionFailedException($"Expected a copy object", copyVariable);
                }

                var name = TemplateHelpers.AssertRequiredProperty(variableObject, "name", "The copy object is missing a \"name\" property").ToString();

                yield return (name, variableObject, true);
            }
        }

        private ProgramSyntax Parse()
        {
            var statements = new List<SyntaxBase>();

            var functions = TemplateHelpers.GetProperty(template, "functions")?.Value as JArray;
            if (functions?.Any() == true)
            {
                var fixupToken = SyntaxHelpers.CreatePlaceholderToken(TokenType.Unrecognized, "TODO: User defined functions are not supported and have not been decompiled");
                statements.Add(fixupToken);
            }

            var targetScope = ParseTargetScope(template);
            if (targetScope != null)
            {
                statements.Add(targetScope);
            }

            if (TemplateHelpers.GetProperty(template, "resources")?.Value is JObject resourcesObject)
            {
                throw new ConversionFailedException($"Decompilation of symbolic name templates is not currently supported", resourcesObject);
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
                if (nameResolver.TryLookupResourceName(typeString, ExpressionHelpers.ParseExpression(nameString)) is { } resourceRef)
                {
                    copyResourceLookup[loopName] = resourceRef;
                }
            }

            AddSyntaxBlock(statements, parameters.Select(ParseParam), false);
            AddSyntaxBlock(statements, GetVariables(variables).Select(x => ParseVariable(x.name, x.value, x.isCopyVariable)), false);
            AddSyntaxBlock(statements, flattenedResources.Select(resource => ParseResource(copyResourceLookup, resource)), true);
            AddSyntaxBlock(statements, outputs.Select(ParseOutput), false);

            return new ProgramSyntax(
                statements.SelectMany(x => new[] { x, SyntaxFactory.NewlineToken }),
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
