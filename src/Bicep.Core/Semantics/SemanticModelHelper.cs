// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;

namespace Bicep.Core.Semantics
{
    public static class SemanticModelHelper
    {
        public static IEnumerable<FunctionCallSyntaxBase> GetFunctionsByName(SemanticModel model, string @namespace, string functionName, SyntaxBase syntax)
        {
            return SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(syntax)
                .Where(s => s.NameEquals(functionName))
                .Where(s => SemanticModelHelper.TryGetFunctionInNamespace(model, @namespace, s) is { });
        }

        public static FunctionCallSyntaxBase? TryGetNamedFunction(SemanticModel model, string @namespace, string functionName, SyntaxBase syntax)
        {
            if (syntax is FunctionCallSyntaxBase functionCall &&
                functionCall.NameEquals(functionName) &&
                SemanticModelHelper.TryGetFunctionInNamespace(model, @namespace, functionCall) is { })
            {
                return functionCall;
            }

            return null;
        }

        public static FunctionCallSyntaxBase? TryGetFunctionInNamespace(SemanticModel semanticModel, string @namespace, SyntaxBase syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol function &&
                function.DeclaringObject is NamespaceType namespaceType &&
                LanguageConstants.IdentifierComparer.Equals(namespaceType.ExtensionName, @namespace))
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

                return LanguageConstants.IdentifierComparer.Equals(namespaceType.ExtensionName, @namespace) &&
                    LanguageConstants.IdentifierComparer.Equals(functionSymbol.Name, decoratorName);
            });
        }
    }
}
