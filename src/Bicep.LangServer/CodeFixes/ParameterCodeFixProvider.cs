// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
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
            if(parameterSyntax.ParameterType is SimpleTypeSyntax simpleType && !supportedTypes.Any(t => simpleType.TypeName == t))
            {
                yield break;
            }

            var decorator = SyntaxFactory.CreateDecorator(decoratorName, decoratorParams);
            var decoratorText = $"{decorator.ToText()}{Environment.NewLine}";
            var newSpan = new TextSpan(parameterSyntax.Span.Position, 0);
            var codeReplacement = new CodeReplacement(newSpan, decoratorText);

            yield return new CodeFix(
                $"Add @{decoratorName}",
                false,
                CodeFixKind.Refactor,
                codeReplacement);
        }
    }
}
