// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Engines;
using Bicep.Core.Parsing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax.Converters
{
    public static class StatementSyntaxConverter
    {
        private static readonly Regex InvalidNameRegex = new(@"(^[0-9].*)|([^a-zA-Z0-9\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Cf}]+)");

        private static readonly string[] TypeNames = new[]
        {
            "array",
            "bool",
            "int",
            "object",
            "secureObject",
            "secureString",
            "string"
        };

        public static TargetScopeSyntax? ConvertToTargetScopeSyntax(JToken schema)
        {
            if (schema.Value<string>() is not { } schemaValue || !Uri.TryCreate(schemaValue, UriKind.Absolute, out var schemaUri))
            {
                throw new SyntaxConversionException($"$schema value \"{schema}\" is not a valid URI.", schema);
            }

            return (schemaUri?.AbsolutePath) switch
            {
                "/schemas/2019-08-01/tenantDeploymentTemplate.json" => new TargetScopeSyntax(
                    SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                    SyntaxFactory.AssignmentToken,
                    SyntaxFactory.CreateStringLiteral("tenant")),
                "/schemas/2019-08-01/managementGroupDeploymentTemplate.json" => new TargetScopeSyntax(
                    SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                    SyntaxFactory.AssignmentToken,
                    SyntaxFactory.CreateStringLiteral("managementGroup")),
                "/schemas/2018-05-01/subscriptionDeploymentTemplate.json" => new TargetScopeSyntax(
                    SyntaxFactory.CreateToken(TokenType.Identifier, "targetScope"),
                    SyntaxFactory.AssignmentToken,
                    SyntaxFactory.CreateStringLiteral("subscription")),
                "/schemas/2014-04-01-preview/deploymentTemplate.json" or
                "/schemas/2015-01-01/deploymentTemplate.json" or
                "/schemas/2019-04-01/deploymentTemplate.json" => null,
                _ => throw new SyntaxConversionException($"$schema value \"{schema}\" did not match any of the known ARM template deployment schemas.", schema),
            };
        }

        public static ParameterDeclarationSyntax ConvertToParameterDeclarationWithTypeInfoOnly(JProperty parameter)
        {
            if (ExpressionsEngine.IsLanguageExpression(parameter.Name))
            {
                throw new SyntaxConversionException($"The parameter name \"{parameter.Name}\" cannot be an expression.", parameter);
            }

            if (InvalidNameRegex.IsMatch(parameter.Name))
            {
                throw new SyntaxConversionException($"Cannot convert the parameter name \"{parameter.Name}\" to an identifier.", parameter);
            }

            if (parameter.Value is not JObject parameterValue)
            {
                throw new SyntaxConversionException("Expected parameter value to be an object.", parameter.Value);
            }

            var decoratorsAndNewLines = new List<SyntaxBase>();

            if (parameterValue["type"] is not { } type)
            {
                throw new SyntaxConversionException($"Unable to locate \"type\" for parameter \"{parameter.Name}\"", parameter.Value);
            }

            var typeSyntax = ConvertToType(type);

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
            }

            ParameterDefaultValueSyntax? syntheticDefaultValue = null;

            if (parameterValue["allowedValues"] is JToken allowedValues &&
                CompileTimeConstantSyntaxConverter.ConvertJArray(allowedValues) is SyntaxBase expression)
            {
                var allowedValueArray = (JArray)allowedValues;

                if (allowedValueArray.Count == 0)
                {
                    throw new SyntaxConversionException($"The \"allowedValues\" array must contain one or more items.", allowedValues);
                }

                foreach (var allowedValue in allowedValueArray)
                {
                    ValidateAllowedValue(allowedValue, typeSyntax);
                }

                decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(LanguageConstants.ParameterAllowedPropertyName, expression));
                decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
                
                if (parameterValue["defaultValue"] is not null)
                {
                    syntheticDefaultValue = new(SyntaxFactory.AssignmentToken, CompileTimeConstantSyntaxConverter.ConvertJToken(allowedValueArray[0]));
                }
            }

            if (parameterValue["defaultValue"] is not null && syntheticDefaultValue is null)
            {
                syntheticDefaultValue = new(SyntaxFactory.AssignmentToken, CreateSyntheticValueForType(typeSyntax));
            }

            return new ParameterDeclarationSyntax(
                decoratorsAndNewLines,
                SyntaxFactory.CreateToken(TokenType.Identifier, "param"),
                SyntaxFactory.CreateIdentifier(parameter.Name),
                typeSyntax,
                syntheticDefaultValue);
        }

        public static OutputDeclarationSyntax? ConvertToOutputDeclarationWithTypeInfoOnly(JProperty output)
        {
            if (ExpressionsEngine.IsLanguageExpression(output.Name))
            {
                throw new SyntaxConversionException($"The output name \"{output.Name}\" cannot be an expression.", output);
            }

            if (InvalidNameRegex.IsMatch(output.Name))
            {
                throw new SyntaxConversionException($"Cannot convert the output name \"{output.Name}\" to an identifier.", output);
            }

            if (output.Value["type"] is not { } type)
            {
                throw new SyntaxConversionException($"Unable to locate \"type\" for output \"{output.Name}\".", output.Value);
            }

            var typeSyntax = ConvertToType(type);

            if (typeSyntax.TypeName == "securestring" || typeSyntax.TypeName == "secureobject")
            {
                // A secure output cannot be referenced from other templates.
                return null;
            }

            SyntaxBase syntheticOutputValue = CreateSyntheticValueForType(typeSyntax);

            return new OutputDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(TokenType.Identifier, "output"),
                SyntaxFactory.CreateIdentifier(output.Name),
                typeSyntax,
                SyntaxFactory.AssignmentToken,
                syntheticOutputValue);
        }

        private static TypeSyntax ConvertToType(JToken value)
        {
            var typeStringLiteral = CompileTimeConstantSyntaxConverter.ConvertToStringLiteral(value);

            if (typeStringLiteral.TryGetLiteralValue() is not { } typeString ||
                !TypeNames.Contains(typeString, StringComparer.OrdinalIgnoreCase))
            {
                throw new SyntaxConversionException($"The type value {value.ToString(Formatting.None)} is invalid.", value);
            }

            return new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, typeString.ToLowerInvariant()));
        }

        private static SyntaxBase CreateSyntheticValueForType(TypeSyntax typeSyntax) => typeSyntax.TypeName switch
        {
            "array" => SyntaxFactory.CreateArray(Enumerable.Empty<SyntaxBase>()),
            "bool" => SyntaxFactory.CreateBooleanLiteral(true),
            "int" => SyntaxFactory.CreateIntegerLiteral(0),
            "object" => SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()),
            "string" => SyntaxFactory.CreateStringLiteral(""),
            _ => throw new InvalidOperationException($"Unknown type name \"{typeSyntax.TypeName}\"."),
        };

        private static void ValidateAllowedValue(JToken allowedValue, TypeSyntax typeSyntax)
        {
            switch (typeSyntax.TypeName)
            {
                case "array" when allowedValue.Type != JTokenType.Array:
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be an array.", allowedValue);
                case "bool" when allowedValue.Type != JTokenType.Boolean:
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be a boolean.", allowedValue);
                case "int" when allowedValue.Type != JTokenType.Integer:
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be an integer.", allowedValue);
                case "object" when allowedValue.Type != JTokenType.Object:
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be an object.", allowedValue);
                case "string" when allowedValue.Type != JTokenType.String && allowedValue.Type != JTokenType.Uri && allowedValue.Type != JTokenType.Date:
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be a string.", allowedValue);
                default:
                    return;
            };
        }
    }
}
