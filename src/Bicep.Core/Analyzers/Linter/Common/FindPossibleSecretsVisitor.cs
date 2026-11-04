// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Common
{
    public sealed class FindPossibleSecretsVisitor : AstVisitor
    {
        public record PossibleSecret(SyntaxBase Syntax, string FoundMessage) { }

        private readonly SemanticModel semanticModel;
        private readonly List<PossibleSecret> possibleSecrets = new();
        private uint trailingAccessExpressions = 0;

        /// <summary>
        /// Searches in an expression for possible references to sensitive data, such as secure parameters or list* functions (many but
        /// not all of which return sensitive information)
        /// </summary>
        public static IEnumerable<PossibleSecret> FindPossibleSecretsInExpression(SemanticModel semanticModel, SyntaxBase syntax)
        {
            FindPossibleSecretsVisitor visitor = new(semanticModel);
            visitor.Visit(syntax);
            return visitor.possibleSecrets;
        }

        private FindPossibleSecretsVisitor(SemanticModel model)
        {
            this.semanticModel = model;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var secretCount = possibleSecrets.Count;

            // Look for references of secure values, e.g.:
            //
            //   @secure()
            //   param secureParam string
            //   output badResult string = 'this is the value ${secureParam}'
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.Name, PossibleSecretMessage(syntax.Name.IdentifierName + pathToSecureComponent))));

            // A copied member of a secure object may have a plain type (for example secureObject.number).
            // Follow its initializer only when the type hasn't already told us where the secret is.
            if (possibleSecrets.Count == secretCount &&
                trailingAccessExpressions == 0 &&
                semanticModel.GetSymbolInfo(syntax) is VariableSymbol variable &&
                semanticModel.Binder.TryGetCycle(variable) is null)
            {
                VisitForwarded(variable.DeclaringVariable.Value, syntax.Name);
            }

            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            CheckAccessExpression(syntax);
            trailingAccessExpressions++;
            base.VisitPropertyAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            CheckAccessExpression(syntax);
            trailingAccessExpressions++;
            base.VisitArrayAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        private void CheckAccessExpression(AccessExpressionSyntax syntax)
        {
            var secretCount = possibleSecrets.Count;
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.IndexExpression, PossibleSecretMessage(syntax.ToString() + pathToSecureComponent))));

            if (possibleSecrets.Count == secretCount &&
                trailingAccessExpressions == 0 &&
                syntax.TryGetPropertyName() is { } propertyName &&
                TryGetObjectPropertyValue(syntax.BaseExpression, propertyName) is { } propertyValue)
            {
                VisitForwarded(propertyValue, syntax.IndexExpression);
            }
        }

        private SyntaxBase? TryGetObjectPropertyValue(SyntaxBase syntax, string propertyName)
            // Follow only the selected property, not its siblings. Spreads may replace that property,
            // and cyclic variables already have a compiler diagnostic, so neither is followed here.
            => syntax switch
            {
                ObjectSyntax obj when !obj.Children.OfType<SpreadExpressionSyntax>().Any()
                    => obj.TryGetPropertyByName(propertyName)?.Value,
                VariableAccessSyntax access when semanticModel.GetSymbolInfo(access) is VariableSymbol variable &&
                    semanticModel.Binder.TryGetCycle(variable) is null
                    => TryGetObjectPropertyValue(variable.DeclaringVariable.Value, propertyName),
                AccessExpressionSyntax access when access.TryGetPropertyName() is { } selectedProperty &&
                    TryGetObjectPropertyValue(access.BaseExpression, selectedProperty) is { } selectedValue
                    => TryGetObjectPropertyValue(selectedValue, propertyName),
                _ => null,
            };

        private void VisitForwarded(SyntaxBase syntax, SyntaxBase anchor)
        {
            var firstForwardedSecret = possibleSecrets.Count;
            Visit(syntax);

            // Keep the secret's name, but report it on this read rather than in a shared variable declaration.
            for (var index = firstForwardedSecret; index < possibleSecrets.Count; index++)
            {
                possibleSecrets[index] = possibleSecrets[index] with { Syntax = anchor };
            }
        }

        internal static string PossibleSecretMessage(string possibleSecretName) => string.Format(CoreResources.PossibleSecretMessageSecureParam, possibleSecretName);

        private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type)
            => TypeHelper.FindPathsToSecureTypeComponents(type, trailingAccessExpressions > 0);

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (DoesNotRevealSecrets(syntax))
            {
                return;
            }

            // Look for usage of list*() stand-alone function, e.g.:
            //
            //   output badResult object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
            //

            if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction
                && listFunction.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
            {
                string foundMessage = string.Format(CoreResources.PossibleSecretMessageFunction, syntax.Name.IdentifierName);
                this.possibleSecrets.Add(new PossibleSecret(syntax, foundMessage));
            }

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            if (DoesNotRevealSecrets(syntax))
            {
                // Some functions return a result that doesn't reveal their arguments' values.
                return;
            }

            if (syntax.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
            {
                bool found = false;

                if (semanticModel.ResourceMetadata.TryLookup(syntax.BaseExpression) is not null)
                {
                    // It's a usage of a list*() member function for a resource value, e.g.:
                    //
                    //   output badResult object = stg.listKeys().keys[0].value
                    //
                    found = true;
                }
                else if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, AzNamespaceType.BuiltInName, syntax) is not null)
                {
                    // It's a usage of a built-in list*() function as a member of the built-in "az" module, e.g.:
                    //
                    //   output badResult object = az.listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
                    //
                    found = true;
                }

                if (found)
                {
                    string foundMessage = string.Format(CoreResources.PossibleSecretMessageFunction, syntax.Name.IdentifierName);
                    this.possibleSecrets.Add(new PossibleSecret(syntax, foundMessage));
                }
            }

            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        private bool DoesNotRevealSecrets(FunctionCallSyntaxBase syntax)
            // nameof() never reads its argument's value; the other allow-listed functions don't reveal it.
            => semanticModel.GetSymbolInfo(syntax) is FunctionSymbol function &&
                (function.FunctionFlags & (FunctionFlags.DoesNotRevealSecrets | FunctionFlags.IsArgumentValueIndependent)) != 0;

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            // Comparisons and logical operators produce booleans, which this analysis treats as non-secret.
            if (syntax.Operator is BinaryOperator.Equals or BinaryOperator.NotEquals or
                BinaryOperator.EqualsInsensitive or BinaryOperator.NotEqualsInsensitive or
                BinaryOperator.LessThan or BinaryOperator.LessThanOrEqual or
                BinaryOperator.GreaterThan or BinaryOperator.GreaterThanOrEqual or
                BinaryOperator.LogicalAnd or BinaryOperator.LogicalOr)
            {
                return;
            }

            base.VisitBinaryOperationSyntax(syntax);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            if (syntax.Operator is UnaryOperator.Not)
            {
                return;
            }

            base.VisitUnaryOperationSyntax(syntax);
        }
    }
}
