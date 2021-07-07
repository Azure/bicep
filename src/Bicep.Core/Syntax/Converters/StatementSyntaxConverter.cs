// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
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

        public static TargetScopeSyntax? ConvertToTargetScopeSyntax(TemplateGenericProperty<string> schema)
        {
            if (!Uri.TryCreate(schema.Value, UriKind.Absolute, out var schemaUri))
            {
                throw new SyntaxConversionException($"$schema value \"{schema.Value}\" is not a valid URI.", schema);
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

        public static ParameterDeclarationSyntax ConvertToParameterDeclarationWithTypeInfoOnly(KeyValuePair<string, TemplateInputParameter> parameter)
        {
            if (InvalidNameRegex.IsMatch(parameter.Key))
            {
                throw new SyntaxConversionException($"Cannot convert the parameter name \"{parameter.Key}\" to an identifier.", parameter.Value);
            }

            var decoratorsAndNewLines = new List<SyntaxBase>();

            var typeSyntax = ConvertToType(parameter.Value.Type);

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

            if (parameter.Value.AllowedValues is { Value: var allowedValues } allowedValuesProperty &&
                CompileTimeConstantSyntaxConverter.ConvertJArray(allowedValues) is SyntaxBase expression)
            {
                if (allowedValues.Count == 0)
                {
                    throw new SyntaxConversionException($"The \"allowedValues\" array must contain one or more items.", allowedValuesProperty);
                }

                foreach (var allowedValue in allowedValues)
                {
                    ValidateAllowedValue(allowedValue, typeSyntax);
                }

                decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(LanguageConstants.ParameterAllowedPropertyName, expression));
                decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
                
                if (parameter.Value.DefaultValue is not null)
                {
                    syntheticDefaultValue = new(SyntaxFactory.AssignmentToken, CompileTimeConstantSyntaxConverter.ConvertJToken(allowedValues[0]));
                }
            }

            if (parameter.Value.DefaultValue is not null && syntheticDefaultValue is null)
            {
                syntheticDefaultValue = new(SyntaxFactory.AssignmentToken, CreateSyntheticValueForType(typeSyntax));
            }

            return new ParameterDeclarationSyntax(
                decoratorsAndNewLines,
                SyntaxFactory.CreateToken(TokenType.Identifier, "param"),
                SyntaxFactory.CreateIdentifier(parameter.Key),
                typeSyntax,
                syntheticDefaultValue);
        }

        public static OutputDeclarationSyntax? ConvertToOutputDeclarationWithTypeInfoOnly(KeyValuePair<string, TemplateOutputParameter> output)
        {
            if (InvalidNameRegex.IsMatch(output.Key))
            {
                throw new SyntaxConversionException($"Cannot convert the output name \"{output.Key}\" to an identifier.", output.Value);
            }

            var typeSyntax = ConvertToType(output.Value.Type);

            if (typeSyntax.TypeName == "securestring" || typeSyntax.TypeName == "secureobject")
            {
                // A secure output cannot be referenced from other templates.
                return null;
            }

            SyntaxBase syntheticOutputValue = CreateSyntheticValueForType(typeSyntax);

            return new OutputDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(TokenType.Identifier, "output"),
                SyntaxFactory.CreateIdentifier(output.Key),
                typeSyntax,
                SyntaxFactory.AssignmentToken,
                syntheticOutputValue);
        }

        private static TypeSyntax ConvertToType(TemplateGenericProperty<TemplateParameterType> typeProperty) =>
            new(SyntaxFactory.CreateToken(TokenType.Identifier, typeProperty.Value.ToString().ToLowerInvariant()));

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
                    throw new SyntaxConversionException($"Expected the value {allowedValue.ToString(Formatting.None)} to be a Boolean.", allowedValue);
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
