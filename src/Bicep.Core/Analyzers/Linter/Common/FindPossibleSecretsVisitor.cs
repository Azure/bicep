// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Common
{
    public sealed class FindPossibleSecretsVisitor : SyntaxVisitor
    {
        public record PossibleSecret
        {
            public SyntaxBase Syntax { get; }
            public string FoundMessage { get; } // E.g. "secure parameter 'parameter1'

            public PossibleSecret(SyntaxBase syntax, string foundMessage)
            {
                Syntax = syntax;
                FoundMessage = foundMessage;
            }
        }

        private const string ListFunctionPrefix = "list";
        private readonly SemanticModel semanticModel;
        private readonly List<PossibleSecret> possibleSecrets = new();

        /// <summary>
        /// Searches in an expression for possible references to sensitive data, such as secure parameters or list* functions (may but
        /// not all of which return sensitive information)
        /// </summary>
        public static IImmutableList<PossibleSecret> FindPossibleSecrets(SemanticModel semanticModel, SyntaxBase syntax)
        {
            FindPossibleSecretsVisitor visitor = new(semanticModel);
            visitor.Visit(syntax);
            return visitor.possibleSecrets.ToImmutableList();
        }

        private FindPossibleSecretsVisitor(SemanticModel model)
        {
            this.semanticModel = model;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            // Look for references of secure parameters, e.g.:
            //
            //   @secure()
            //   param secureParam string
            //   output badResult string = 'this is the value ${secureParam}'
            Symbol? symbol = semanticModel.GetSymbolInfo(syntax);
            if (symbol is ParameterSymbol param)
            {
                if (param.IsSecure())
                {
                    string foundMessage = string.Format(CoreResources.PossibleSecretMessageSecureParam, syntax.Name.IdentifierName);
                    this.possibleSecrets.Add(new PossibleSecret(syntax.Name, foundMessage));
                }
            }

            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            // Look for usage of list*() stand-alone function, e.g.:
            //
            //   output badResult object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
            //

            if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction
                && listFunction.Name.IdentifierName.StartsWithOrdinalInsensitively(ListFunctionPrefix))
            {
                string foundMessage = string.Format(CoreResources.PossibleSecretMessageFunction, syntax.Name.IdentifierName);
                this.possibleSecrets.Add(new PossibleSecret(syntax, foundMessage));
            }

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            if (syntax.Name.IdentifierName.StartsWithOrdinalInsensitively(ListFunctionPrefix))
            {
                bool found = false;

                Symbol? baseSymbol = semanticModel.GetSymbolInfo(syntax.BaseExpression);
                if (baseSymbol is ResourceSymbol)
                {
                    // It's a usage of a list*() member function for a resource value, e.g.:
                    //
                    //   output badResult object = stg.listKeys().keys[0].value
                    //
                    found = true;
                }
                else if (baseSymbol is BuiltInNamespaceSymbol)
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
