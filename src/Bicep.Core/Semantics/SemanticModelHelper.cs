// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public static class SemanticModelHelper
    {
        public static FunctionCallSyntaxBase? TryGetFunctionInNamespace(SemanticModel semanticModel, string @namespace, SyntaxBase syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol function &&
                function.DeclaringObject is NamespaceType namespaceType &&
                LanguageConstants.IdentifierComparer.Equals(namespaceType.ProviderName, @namespace))
            {
                return syntax as FunctionCallSyntaxBase;
            }

            return null;
        }

        public static DecoratorSyntax? TryGetDecoratorInNamespace(SemanticModel semanticModel, DecorableSyntax syntax, string @namespace, string decoratorName)
            => TryGetDecoratorInNamespace(semanticModel.Binder, semanticModel.TypeManager.GetDeclaredType, syntax, @namespace, decoratorName);

        public static DecoratorSyntax? TryGetDecoratorInNamespace(IBinder binder, Func<SyntaxBase, TypeSymbol?> getDeclaredTypeFunc, DecorableSyntax syntax, string @namespace, string decoratorName)
        {
            return syntax.Decorators.FirstOrDefault(decorator =>
            {
                if (SymbolHelper.TryGetSymbolInfo(binder, getDeclaredTypeFunc, decorator.Expression) is not FunctionSymbol functionSymbol ||
                    functionSymbol.DeclaringObject is not NamespaceType namespaceType)
                {
                    return false;
                }

                return LanguageConstants.IdentifierComparer.Equals(namespaceType.ProviderName, @namespace) &&
                    LanguageConstants.IdentifierComparer.Equals(functionSymbol.Name, decoratorName);
            });
        }

        public static string? TryGetDescription(SemanticModel semanticModel, DecorableSyntax decorable)
            => TryGetDescription(semanticModel.Binder, semanticModel.TypeManager.GetDeclaredType, decorable);

        public static string? TryGetDescription(IBinder binder, Func<SyntaxBase, TypeSymbol?> getDeclaredTypeFunc, DecorableSyntax decorable)
        {
            var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder,
                getDeclaredTypeFunc,
                decorable,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.MetadataDescriptionPropertyName);

            if (decorator is not null &&
                decorator.Arguments.FirstOrDefault()?.Expression is StringSyntax stringSyntax
                && stringSyntax.TryGetLiteralValue() is string description)
            {
                return description;
            }

            return null;
        }
    }
}
