// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
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
                .Select(pathToSecureComponent => new PossibleSecret(syntax.PropertyName, PossibleSecretMessage(syntax.ToTextPreserveFormatting() + pathToSecureComponent))));

            trailingAccessExpressions++;
            base.VisitPropertyAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.IndexExpression, PossibleSecretMessage(syntax.ToTextPreserveFormatting() + pathToSecureComponent))));

            trailingAccessExpressions++;
            base.VisitArrayAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        private static string PossibleSecretMessage(string possibleSecretName) => string.Format(CoreResources.PossibleSecretMessageSecureParam, possibleSecretName);

        private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type) => FindPathsToSecureTypeComponents(type, "", new());

        private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type, string path, HashSet<TypeSymbol> visited)
        {
            // types can be recursive. cut out early if we've already seen this type
            if (visited.Contains(type))
            {
                yield break;
            }

            visited.Add(type);

            if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return path;
            }

            if (type is UnionType union)
            {
                foreach (var variantPath in union.Members.SelectMany(m => FindPathsToSecureTypeComponents(m.Type, path, visited)))
                {
                    yield return variantPath;
                }
            }

            // if the expression being visited is dereferencing a specific property or index of this type, we shouldn't warn if the type under inspection
            // *contains* properties or indices that are flagged as secure. We will have already warned if those have been accessed in the expression, and
            // if they haven't, then the value dereferenced isn't sensitive
            //
            //    param p {
            //      prop: {
            //        @secure()
            //        nestedSecret: string
            //        nestedInnocuousProperty: string
            //      }
            //    }
            //
            //    output objectContainingSecrets object = p                     // <-- should be flagged
            //    output propertyContainingSecrets object = p.prop              // <-- should be flagged
            //    output nestedSecret string = p.prop.nestedSecret              // <-- should be flagged
            //    output siblingOfSecret string = p.prop.nestedInnocuousData    // <-- should NOT be flagged
            if (trailingAccessExpressions == 0)
            {
                switch (type)
                {
                    case ObjectType obj:
                        if (obj.AdditionalPropertiesType?.Type is TypeSymbol addlPropsType)
                        {
                            foreach (var dictMemberPath in FindPathsToSecureTypeComponents(addlPropsType, $"{path}.*", visited))
                            {
                                yield return dictMemberPath;
                            }
                        }

                        foreach (var propertyPath in obj.Properties.SelectMany(p => FindPathsToSecureTypeComponents(p.Value.TypeReference.Type, $"{path}.{p.Key}", visited)))
                        {
                            yield return propertyPath;
                        }
                        break;
                    case TupleType tuple:
                        foreach (var pathFromIndex in tuple.Items.SelectMany((ITypeReference typeAtIndex, int index) => FindPathsToSecureTypeComponents(typeAtIndex.Type, $"{path}[{index}]", visited)))
                        {
                            yield return pathFromIndex;
                        }
                        break;
                    case ArrayType array:
                        foreach (var pathFromElement in FindPathsToSecureTypeComponents(array.Item.Type, $"{path}[*]", visited))
                        {
                            yield return pathFromElement;
                        }
                        break;
                }
            }

            visited.Remove(type);
        }


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
