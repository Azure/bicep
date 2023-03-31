// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Common
{
    public static class LinterExpressionHelper
    {
        /// <summary>
        /// Attempts to find a resource with the same name as the given expression
        /// </summary>
        /// <remarks>This is not intended to be foolproof, but simply to handle common scenarios</remarks>
        public static IEnumerable<DeclaredResourceMetadata> TryFindResourceByNameExpression(SemanticModel model, SyntaxBase resourceNameExpression)
        {
            // CONSIDER: Support child resources and arrays and other improvements

            var resourcesAndNames = new List<(DeclaredResourceMetadata resource, SyntaxBase name)>();
            foreach (var resource in model.DeclaredResources.Where(r => r.IsAzResource))
            {
                if (resource.Symbol.DeclaringSyntax is ResourceDeclarationSyntax declarationSyntax
                    && declarationSyntax.TryGetBody()?.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName) is ObjectPropertySyntax objectPropertySyntax
                    && objectPropertySyntax.Value is SyntaxBase resourceNameSyntax)
                {
                    resourcesAndNames.Add((resource, resourceNameSyntax));
                }
            }

            if (resourcesAndNames.Any())
            {
                string formattedSearchName = resourceNameExpression.ToText();
                string? evaluatedSearchNameLiteral = SyntaxHelper.TryGetEvaluatedStringLiteral(model, resourceNameExpression, true)?.stringValue;

                foreach (var (resource, resourceName) in resourcesAndNames)
                {
                    // First try a formatted expression match
                    if (resourceName.ToText().EqualsOrdinally(formattedSearchName))
                    {
                        yield return resource;
                    }

                    // Then literal values (if they both evaluate to literal values)
                    if (evaluatedSearchNameLiteral is not null
                        && SyntaxHelper.TryGetEvaluatedStringLiteral(model, resourceName, true) is (string resourceNameLiteral, _, _)
                        && evaluatedSearchNameLiteral.EqualsOrdinally(resourceNameLiteral))
                    {
                        yield return resource;
                    }
                }
            }
        }

        private static readonly Regex IsRegexRegex = new("[.$^([\\]]", RegexOptions.Compiled);

        public static IEnumerable<FunctionCallSyntaxBase> FindFunctionCallsByName(SemanticModel model, SyntaxBase root, string @namespace, string functionNameOrRegex)
        {
            bool isFunctionNameARegex = IsRegexRegex.IsMatch(functionNameOrRegex);
            Regex? regex = isFunctionNameARegex ? new Regex(functionNameOrRegex) : null;

            return SyntaxAggregator.Aggregate(
                source: root,
                seed: new List<FunctionCallSyntaxBase>(),
                function: (accumulated, syntax) =>
                {
                    if (SemanticModelHelper.TryGetFunctionInNamespace(model, @namespace, syntax) is FunctionCallSyntaxBase functionCallSyntax)
                    {
                        string functionName = functionCallSyntax.Name.IdentifierName;
                        if (regex is not null && regex.IsMatch(functionName)
                            || functionName.EqualsOrdinally(functionNameOrRegex))
                        {
                            accumulated.Add(functionCallSyntax);
                        }
                    }

                    return accumulated;
                },
                resultSelector: accumulated => accumulated);
        }
    }
}
