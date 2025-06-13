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
            // Look for references of secure values, e.g.:
            //
            //   @secure()
            //   param secureParam string
            //   output badResult string = 'this is the value ${secureParam}'
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.Name, PossibleSecretMessage(syntax.Name.IdentifierName + pathToSecureComponent))));

            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.PropertyName, PossibleSecretMessage(syntax.ToString() + pathToSecureComponent))));

            trailingAccessExpressions++;
            base.VisitPropertyAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.IndexExpression, PossibleSecretMessage(syntax.ToString() + pathToSecureComponent))));

            trailingAccessExpressions++;
            base.VisitArrayAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        private static string PossibleSecretMessage(string possibleSecretName) => string.Format(CoreResources.PossibleSecretMessageSecureParam, possibleSecretName);

        private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type)
            => TypeHelper.FindPathsToSecureTypeComponents(type, trailingAccessExpressions > 0);

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
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
    }
}
