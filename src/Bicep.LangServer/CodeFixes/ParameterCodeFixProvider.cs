// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.CodeFixes
{
    public class ParameterCodeFixProvider : ICodeFixProvider
    {
        private readonly string decoratorName;
        private readonly string[] supportedTypes;
        private readonly SyntaxBase[] decoratorParams;

        public ParameterCodeFixProvider(string decoratorName, string[] supportedTypes, SyntaxBase[] decoratorParams)
        {
            this.decoratorName = decoratorName;
            this.supportedTypes = supportedTypes;
            this.decoratorParams = decoratorParams;
        }

        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            if (matchingNodes.OfType<ParameterDeclarationSyntax>().FirstOrDefault() is not {} parameterSyntax ||
                semanticModel.GetSymbolInfo(parameterSyntax) is not ParameterSymbol parameterSymbol ||
                parameterSymbol.HasDecorator(decoratorName)) 
            {
                yield break;
            }
            if(!supportedTypes.Any(t => parameterSyntax.ParameterType?.TypeName == t))
            {
                yield break;
            }

            var newLeadingNodes = parameterSyntax.LeadingNodes
                .Add(SyntaxFactory.CreateDecorator(decoratorName, decoratorParams))
                .Add(SyntaxFactory.NewlineToken);

            var newParameterSyntax = new ParameterDeclarationSyntax(
                newLeadingNodes,
                parameterSyntax.Keyword,
                parameterSyntax.Name,
                parameterSyntax.Type,
                parameterSyntax.Modifier);
            string text;
            try
            {
                text = newParameterSyntax.ToText();
            }
            catch (Exception)
            {
                yield break;
            }
            var codeReplacement = new CodeReplacement(parameterSyntax.Span, text);

            yield return new CodeFix(
                $"Add @{decoratorName}",
                false,
                codeReplacement);
        }
    }
}
