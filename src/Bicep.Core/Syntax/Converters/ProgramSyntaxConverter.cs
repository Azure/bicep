// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax.Converters
{
    public static class ProgramSyntaxConverter
    {
        public static ProgramSyntax ConvertToProgramSyntaxWithTypeInfoOnly(Template template)
        {
            var targetScope = StatementSyntaxConverter.ConvertToTargetScopeSyntax(template.Schema);

            var parameterDeclarations = template.Parameters is { } parameters
                ? parameters.Select(StatementSyntaxConverter.ConvertToParameterDeclarationWithTypeInfoOnly)
                : Enumerable.Empty<ParameterDeclarationSyntax>();

            var outputDeclarations = template.Outputs is { } outputs
                ? outputs.Select(StatementSyntaxConverter.ConvertToOutputDeclarationWithTypeInfoOnly).OfType<OutputDeclarationSyntax>()
                : Enumerable.Empty<OutputDeclarationSyntax>();

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
    }
}
