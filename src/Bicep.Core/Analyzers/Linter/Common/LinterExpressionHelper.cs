// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem.Providers.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Common
{
    public static class LinterExpressionHelper
    {
        /// <summary>
        /// Tries to retrieve a string literal from the expression.
        /// </summary>
        public static string? TryGetEvaluatedStringLiteral(SemanticModel model, SyntaxBase? expression)
            => (expression is { } && model.GetTypeInfo(expression).Type is TypeSystem.Types.StringLiteralType stringLiteralType) ? stringLiteralType.RawStringValue : null;

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
                string searchName = resourceNameExpression.ToString();
                string? evaluatedSearchNameLiteral = TryGetEvaluatedStringLiteral(model, resourceNameExpression);

                foreach (var (resource, resourceName) in resourcesAndNames)
                {
                    // First try a expression text match
                    if (resourceName.ToString().EqualsOrdinally(searchName))
                    {
                        yield return resource;
                    }

                    // Then literal values (if they both evaluate to literal values)
                    if (evaluatedSearchNameLiteral is not null
                        && TryGetEvaluatedStringLiteral(model, resourceName) is { } resourceNameLiteral
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
