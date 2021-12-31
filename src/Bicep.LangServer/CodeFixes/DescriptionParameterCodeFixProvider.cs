// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CodeFixes
{
    public class DescriptionParameterCodeFixProvider : ICodeFixProvider
    {
        public static string[] SupportedTypes= new []{"string", "object"};
        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            if (matchingNodes.OfType<ParameterDeclarationSyntax>().FirstOrDefault() is not {} parameterSyntax ||
                semanticModel.GetSymbolInfo(parameterSyntax) is not ParameterSymbol parameterSymbol ||
                parameterSymbol.HasDecorator("description") ) 
            {
                yield break;
            }

            var newLeadingNodes = parameterSyntax.LeadingNodes
                .Add(SyntaxFactory.CreateDecorator("description", SyntaxFactory.CreateStringLiteral(String.Empty)))
                .Add(SyntaxFactory.NewlineToken);

            var newParameterSyntax = new ParameterDeclarationSyntax(
                newLeadingNodes,
                parameterSyntax.Keyword,
                parameterSyntax.Name,
                parameterSyntax.Type,
                parameterSyntax.Modifier);

            var codeReplacement = CodeReplacement.FromSyntax(parameterSyntax.Span, newParameterSyntax);

            yield return new CodeFix(
                "Add @description",
                false,
                codeReplacement);
        }
    }
}
