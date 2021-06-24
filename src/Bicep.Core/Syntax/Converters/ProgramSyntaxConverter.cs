// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax.Converters
{
    public static class ProgramSyntaxConverter
    {
        public static ProgramSyntax ConvertToProgramSyntaxWithTypeInfoOnly(JObject templateObject)
        {
            var schema = GetProperty<JToken>(templateObject, "$schema");

            if (schema is null)
            {
                throw new SyntaxConversionException("Cannot locate $schema for the template.", templateObject);
            }

            var contentVersion = GetProperty<JToken>(templateObject, "contentVersion");

            if (contentVersion is null)
            {
                throw new SyntaxConversionException("Cannot locate contentVersion for the template.", templateObject);
            }

            if (contentVersion.Type != JTokenType.String)
            {
                throw new SyntaxConversionException("Expected contentVersion to be a string.", contentVersion);
            }

            var resources = GetProperty<JToken>(templateObject, "resources");

            if (resources is null)
            {
                throw new SyntaxConversionException("Cannot locate resources for the template.", templateObject);
            }

            if (resources is not JArray)
            {
                throw new SyntaxConversionException("Expected resources to be an array.", resources);
            }

            var targetScope = StatementSyntaxConverter.ConvertToTargetScopeSyntax(schema);

            var parameters = GetProperty(templateObject, "parameters", new JObject()).Properties();
            var parameterDeclarations = parameters.Select(StatementSyntaxConverter.ConvertToParameterDeclarationWithTypeInfoOnly);

            var outputs = GetProperty(templateObject, "outputs", new JObject()).Properties();
            var outputDeclarations = outputs.Select(StatementSyntaxConverter.ConvertToParameterDeclarationWithTypeInfoOnly);

            var statements = new List<SyntaxBase>();

            if (targetScope is not null)
            {
                statements.Add(targetScope);
            }

            AddSyntaxBlock(statements, parameterDeclarations, false);
            AddSyntaxBlock(statements, outputDeclarations, false);

            return new ProgramSyntax(
                statements.SelectMany(x => new[] { x, SyntaxFactory.NewlineToken }),
                SyntaxFactory.CreateToken(TokenType.EndOfFile),
                Enumerable.Empty<IDiagnostic>()
            );
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

        private static T? GetProperty<T>(JObject templateObject, string propertyName) =>
            templateObject.Property(propertyName, StringComparison.OrdinalIgnoreCase)?.Value is T value ? value : default;

        private static T GetProperty<T>(JObject templateObject, string propertyName, T fallbackValue) where T : notnull =>
            templateObject.Property(propertyName, StringComparison.OrdinalIgnoreCase)?.Value is T value ? value : fallbackValue;
    }
}
