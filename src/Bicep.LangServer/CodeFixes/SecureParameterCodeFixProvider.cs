// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CodeFixes
{
    public class SecureParameterCodeFixProvider : ICodeFixProvider
    {
        public static string[] SupportedTypes= new []{"string", "object"};
        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            if (matchingNodes.OfType<ParameterDeclarationSyntax>().FirstOrDefault() is not {} parameterSyntax ||
                semanticModel.GetSymbolInfo(parameterSyntax) is not ParameterSymbol parameterSymbol ||
                parameterSymbol.IsSecure()) 
            {
                yield break;
            }
            if(!SupportedTypes.Any(t => parameterSyntax.ParameterType?.TypeName == t))
            {
                yield break;
            }

            var newLeadingNodes = parameterSyntax.LeadingNodes
                .Add(SyntaxFactory.CreateDecorator("secure"))
                .Add(SyntaxFactory.NewlineToken);

            var newParameterSyntax = new ParameterDeclarationSyntax(
                newLeadingNodes,
                parameterSyntax.Keyword,
                parameterSyntax.Name,
                parameterSyntax.Type,
                parameterSyntax.Modifier);

            var codeReplacement = CodeReplacement.FromSyntax(parameterSyntax.Span, newParameterSyntax);

            yield return new CodeFix(
                "Add @secure",
                false,
                codeReplacement);
        }
    }
}
