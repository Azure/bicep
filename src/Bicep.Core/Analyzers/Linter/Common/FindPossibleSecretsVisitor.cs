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

        private record PropertyValue(SyntaxBase Syntax, TypeSymbol? Type = null, string? Name = null, bool IsSecure = false);
        // IsDefinite means a source guarantees the property exists, not that only one value is possible.
        // IsMerge keeps union() sources in precedence order so nested reads merge them instead of replacing them.
        private record PropertyResolution(PropertyValue[] Values, bool IsDefinite, bool IsMerge = false);

        private readonly SemanticModel semanticModel;
        private readonly List<PossibleSecret> possibleSecrets = new();
        private readonly Dictionary<SyntaxBase, PossibleSecret[]> secretsBySyntax = new();
        private readonly Dictionary<(SyntaxBase Syntax, string PropertyName), PropertyResolution> propertyInitializers = new();
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
                // Cache even empty results; public diamonds otherwise repeat all their initializer reads.
                possibleSecrets.AddRange(GetSecretsIn(variable.DeclaringVariable.Value)
                    .Select(secret => secret with { Syntax = syntax.Name }));
            }

            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (CheckAccessExpression(syntax))
            {
                return;
            }
            trailingAccessExpressions++;
            base.VisitPropertyAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (CheckAccessExpression(syntax))
            {
                Visit(syntax.IndexExpression);
                return;
            }
            trailingAccessExpressions++;
            base.VisitArrayAccessSyntax(syntax);
            trailingAccessExpressions--;
        }

        private bool CheckAccessExpression(AccessExpressionSyntax syntax)
        {
            var secretCount = possibleSecrets.Count;
            possibleSecrets.AddRange(FindPathsToSecureTypeComponents(semanticModel.GetTypeInfo(syntax))
                .Select(pathToSecureComponent => new PossibleSecret(syntax.IndexExpression, PossibleSecretMessage(syntax.ToString() + pathToSecureComponent))));

            if (possibleSecrets.Count != secretCount || trailingAccessExpressions > 0)
            {
                return possibleSecrets.Count != secretCount;
            }

            var followFallback = semanticModel.GetTypeInfo(syntax) is AnyType ||
                !TypeHelper.IsOrContainsSecureType(semanticModel.GetTypeInfo(syntax.BaseExpression));

            if (syntax.TryGetPropertyName() is { } propertyName)
            {
                var property = ResolvePropertyInitializers(syntax.BaseExpression, propertyName);
                if (property.IsDefinite || followFallback)
                {
                    foreach (var value in property.Values)
                    {
                        AddForwardedSecrets(value, syntax);
                    }
                    return property.IsDefinite || property.Values.Length > 0;
                }
            }
            // Any other read, like parts[0] or cfg.password from json(secret), may select any part of the value.
            // If its type has lost track of the secret, follow the whole initializer, just as reading it inline would.
            else if (followFallback)
            {
                var initializers = ResolveInitializers(syntax.BaseExpression).ToArray();
                foreach (var initializer in initializers)
                {
                    AddForwardedSecrets(initializer, syntax);
                }
                return initializers.Length > 0;
            }

            return false;
        }

        private IEnumerable<PropertyValue> ResolveInitializers(SyntaxBase syntax)
            => syntax switch
            {
                VariableAccessSyntax access when semanticModel.GetSymbolInfo(access) is VariableSymbol variable &&
                    semanticModel.Binder.TryGetCycle(variable) is null
                    => [new(variable.DeclaringVariable.Value)],
                VariableAccessSyntax => [],
                AccessExpressionSyntax access => access.TryGetPropertyName() is { } propertyName
                    ? ResolvePropertyInitializers(access.BaseExpression, propertyName).Values
                    : ResolveArrayItemInitializers(access),
                _ => [new(syntax)],
            };

        private IEnumerable<PropertyValue> ResolveArrayItemInitializers(AccessExpressionSyntax access)
            => ResolveInitializers(access.BaseExpression).SelectMany(value => value.Type is null
                ? [value]
                : ResolveTypedArrayItem(value, access));

        private IEnumerable<PropertyValue> ResolveTypedArrayItem(PropertyValue value, AccessExpressionSyntax access)
        {
            var type = GetValueType(value);
            var isSecure = value.IsSecure || type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
            if (type is UnionType union)
            {
                return union.Members.Where(member => member.Type is not NullType)
                    .SelectMany(member => ResolveTypedArrayItem(value with { Type = member.Type, IsSecure = isSecure }, access));
            }

            var itemType = type switch
            {
                TupleType tuple when semanticModel.GetTypeInfo(access.IndexExpression) is IntegerLiteralType index &&
                    index.Value >= 0 && index.Value < tuple.Items.Length => tuple.Items[(int)index.Value].Type,
                ArrayType array => array.Item.Type,
                _ => type,
            };
            return [value with { Type = itemType, Name = isSecure ? value.Name : $"{value.Name}[{access.IndexExpression}]", IsSecure = isSecure }];
        }

        private PropertyResolution ResolvePropertyInitializers(SyntaxBase syntax, string propertyName)
        {
            if (propertyInitializers.TryGetValue((syntax, propertyName), out var result))
            {
                return result;
            }

            // Cache absent properties too, so repeated spreads don't expand a diamond exponentially.
            result = syntax switch
            {
                ObjectSyntax obj => ResolveObjectPropertyInitializers(obj, propertyName),
                ParenthesizedExpressionSyntax parenthesized => ResolvePropertyInitializers(parenthesized.Expression, propertyName),
                NonNullAssertionSyntax nonNull => ResolvePropertyInitializers(nonNull.BaseExpression, propertyName),
                VariableAccessSyntax access when semanticModel.GetSymbolInfo(access) is VariableSymbol variable &&
                    semanticModel.Binder.TryGetCycle(variable) is null
                    => ResolvePropertyInitializers(variable.DeclaringVariable.Value, propertyName),
                VariableAccessSyntax access when semanticModel.GetSymbolInfo(access) is VariableSymbol cyclic &&
                    semanticModel.Binder.TryGetCycle(cyclic) is not null => new([], false),
                // Project the selected type, not the whole parameter or loop item with its unrelated members.
                VariableAccessSyntax => ResolveTypedProperty(new(syntax, semanticModel.GetTypeInfo(syntax), syntax.ToString()), propertyName),
                AccessExpressionSyntax access when access.TryGetPropertyName() is { } selectedProperty
                    => ResolveAccessPropertyInitializers(access, selectedProperty, propertyName),
                AccessExpressionSyntax access => new(ResolveArrayItemInitializers(access)
                    .SelectMany(value => value.Type is null ? [value] : ResolveTypedProperty(value, propertyName).Values).Distinct().ToArray(), false),
                FunctionCallSyntaxBase function when SemanticModelHelper.TryGetNamedFunction(semanticModel, SystemNamespaceType.BuiltInName, "union", function) is { }
                    => ResolveUnionPropertyInitializers(function, propertyName),
                _ => new([new(syntax)], false),
            };
            propertyInitializers.Add((syntax, propertyName), result);
            return result;
        }

        private PropertyResolution ResolveAccessPropertyInitializers(AccessExpressionSyntax access, string selectedProperty, string propertyName)
        {
            var selected = ResolvePropertyInitializers(access.BaseExpression, selectedProperty);
            if (!selected.IsDefinite && !selected.IsMerge && (selected.Values.Length == 0 || selected.Values.Any(value => value.Type is null)))
            {
                // An unresolved projection is not its whole source object, which may contain unrelated secrets.
                return new([new(access)], false);
            }

            if (selected.IsMerge)
            {
                return ResolveMergedPropertyInitializers(selected.Values, propertyName);
            }

            var results = selected.Values.Select(value => ResolvePropertyValue(value, propertyName)).ToArray();
            return new(results.SelectMany(result => result.Values).Distinct().ToArray(),
                results.Length > 0 && results.All(result => result.IsDefinite));
        }

        private PropertyResolution ResolvePropertyValue(PropertyValue value, string propertyName)
            => value.Type is null ? ResolvePropertyInitializers(value.Syntax, propertyName) : ResolveTypedProperty(value, propertyName);

        private PropertyResolution ResolveTypedProperty(PropertyValue value, string propertyName)
        {
            var type = GetValueType(value);
            var isSecure = value.IsSecure || type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
            IEnumerable<TypeSymbol>? members = type switch
            {
                UnionType union => union.Members.Select(member => member.Type),
                DiscriminatedObjectType discriminated => discriminated.UnionMembersByKey.Values,
                _ => null,
            };
            if (members is not null)
            {
                var results = members.Where(member => member is not NullType)
                    .Select(member => ResolveTypedProperty(value with { Type = member, IsSecure = isSecure }, propertyName)).ToArray();
                return new(results.SelectMany(result => result.Values).Distinct().ToArray(),
                    results.Length > 0 && results.All(result => result.IsDefinite));
            }

            var property = (type as ObjectType)?.Properties.GetValueOrDefault(propertyName);
            var propertyType = property?.TypeReference.Type ?? (type as ObjectType)?.AdditionalProperties?.TypeReference.Type;
            return new([new(value.Syntax, propertyType ?? LanguageConstants.Any, isSecure ? value.Name : $"{value.Name}.{propertyName}", isSecure)],
                property?.Flags.HasFlag(TypePropertyFlags.Required) == true);
        }

        private PropertyResolution ResolveObjectPropertyInitializers(ObjectSyntax obj, string propertyName)
        {
            List<PropertyValue> values = [];
            // The last definition wins. Dynamic keys and unknown spreads may replace earlier properties.
            foreach (var child in obj.Children.Reverse())
            {
                switch (child)
                {
                    case ObjectPropertySyntax property when property.HasPropertyName(propertyName):
                        values.Add(new(property.Value));
                        return new(values.Distinct().ToArray(), true);
                    case ObjectPropertySyntax property when property.TryGetKeyText() is null:
                        values.Add(new(property.Value));
                        break;
                    case SpreadExpressionSyntax spread:
                        var result = ResolvePropertyInitializers(spread.Expression, propertyName);
                        values.AddRange(result.Values);
                        if (result.IsDefinite)
                        {
                            return new(values.Distinct().ToArray(), true, result.IsMerge);
                        }
                        break;
                }
            }

            return new(values.Distinct().ToArray(), false);
        }

        private PropertyResolution ResolveUnionPropertyInitializers(FunctionCallSyntaxBase union, string propertyName)
            => ResolveMergedPropertyInitializers(union.Arguments.Reverse().Select(argument => new PropertyValue(argument.Expression)), propertyName);

        private PropertyResolution ResolveMergedPropertyInitializers(IEnumerable<PropertyValue> sources, string propertyName)
        {
            List<PropertyValue> values = [];
            var isDefinite = false;
            var mergingObjects = false;
            foreach (var source in sources)
            {
                var result = ResolvePropertyValue(source, propertyName);
                values.AddRange(result.Values.Where(value => !mergingObjects || GetValueType(value) is ObjectLikeType or AnyType or UnionType));
                isDefinite |= result.IsDefinite;
                if (result.IsDefinite)
                {
                    // Unlike a spread, union() recursively merges objects instead of replacing them.
                    if (result.Values.All(value => GetValueType(value) is not (ObjectLikeType or AnyType or UnionType)))
                    {
                        break;
                    }
                    mergingObjects = true;
                }
            }

            return new(values.Distinct().ToArray(), isDefinite, IsMerge: true);
        }

        private TypeSymbol GetValueType(PropertyValue value) => value.Type ?? semanticModel.GetTypeInfo(value.Syntax);

        private void AddForwardedSecrets(PropertyValue value, AccessExpressionSyntax access)
        {
            if (value.Type is { } type)
            {
                var paths = value.IsSecure ? [""] : TypeHelper.FindPathsToSecureTypeComponents(type);
                possibleSecrets.AddRange(paths.Select(path => new PossibleSecret(access.IndexExpression, PossibleSecretMessage(value.Name + path))));
                return;
            }

            // Keep inline findings where they are, but anchor shared initializers on this read.
            possibleSecrets.AddRange(GetSecretsIn(value.Syntax).Select(secret => access.Span.Contains(secret.Syntax.Span.Position)
                ? secret
                : secret with { Syntax = access.IndexExpression }));
        }

        private PossibleSecret[] GetSecretsIn(SyntaxBase syntax)
        {
            if (!secretsBySyntax.TryGetValue(syntax, out var secrets))
            {
                var firstSecret = possibleSecrets.Count;
                Visit(syntax);
                secrets = possibleSecrets.Skip(firstSecret).ToArray();
                possibleSecrets.RemoveRange(firstSecret, possibleSecrets.Count - firstSecret);
                secretsBySyntax.Add(syntax, secrets);
            }

            return secrets;
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
