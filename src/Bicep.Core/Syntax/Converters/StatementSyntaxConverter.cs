// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
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
            if (InvalidNameRegex.IsMatch(parameter.Name))
            {
                throw new SyntaxConversionException($"Cannot convert the parameter name \"{parameter.Name}\" to an identifier.", parameter);
            }

            if (parameter.Value is not JObject parameterValue)
            {
                throw new SyntaxConversionException("Expected parameter value to be an object.", parameter.Value);
            }

            var decoratorsAndNewLines = new List<SyntaxBase>();

            if (parameterValue["allowedValue"] is JToken allowedValue &&
                CompileTimeConstantSyntaxConverter.ConvertJArray(allowedValue) is SyntaxBase expression)
            {
                var functionName = LanguageConstants.ParameterAllowedPropertyName;

                decoratorsAndNewLines.Add(SyntaxFactory.CreateDecorator(functionName, expression));
                decoratorsAndNewLines.Add(SyntaxFactory.NewlineToken);
            }

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

            return new ParameterDeclarationSyntax(
                decoratorsAndNewLines,
                SyntaxFactory.CreateToken(TokenType.Identifier, "param"),
                SyntaxFactory.CreateIdentifier(parameter.Name),
                typeSyntax,
                null);
        }

        public static OutputDeclarationSyntax? ConvertToOutputDeclarationWithTypeInfoOnly(JProperty output)
        {
            if (InvalidNameRegex.IsMatch(output.Name))
            {
                throw new SyntaxConversionException($"Cannot convert the output name {output.Name} to an identifier.", output);
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

            SyntaxBase syntheticOutputValue = typeSyntax.TypeName switch
            {
                "array" => SyntaxFactory.CreateArray(Enumerable.Empty<SyntaxBase>()),
                "bool" => SyntaxFactory.CreateBooleanLiteral(true),
                "int" => SyntaxFactory.CreateIntegerLiteral(0),
                "object" => SyntaxFactory.CreateObject(Enumerable.Empty<ObjectPropertySyntax>()),
                "string" => SyntaxFactory.CreateStringLiteral(""),
                _ => throw new InvalidOperationException($"Unknown type name \"{typeSyntax.TypeName}\"."),
            };


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
                throw new SyntaxConversionException($"The type value {value} is invalid.", value);
            }

            return new TypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, typeString.ToLowerInvariant()));
        }
    }
}
