// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

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

        public static bool TryGetSemanticModelForForeignTemplateReference(Compilation compilation,
            IForeignTemplateReference reference,
            DiagnosticBuilder.ErrorBuilderDelegate onInvalidSourceFileType,
            [NotNullWhen(true)] out ISemanticModel? semanticModel,
            [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
        {
            if (compilation.SourceFileGrouping.TryGetErrorDiagnostic(reference) is {} errorBuilder)
            {
                semanticModel = null;
                failureDiagnostic = errorBuilder(DiagnosticBuilder.ForPosition(reference.ReferenceSourceSyntax));
                return false;
            }

            // SourceFileGroupingBuilder should have already visited every module declaration and either recorded a failure or mapped it to a syntax tree.
            // So it is safe to assume that this lookup will succeed without throwing an exception.
            var sourceFile = compilation.SourceFileGrouping.TryGetSourceFile(reference) ?? throw new InvalidOperationException($"Failed to find source file for module");

            // when we inevitably add a third language ID,
            // the inclusion list style below will prevent the new language ID from being
            // automatically allowed to be referenced via module declarations
            if (sourceFile is not BicepFile and not ArmTemplateFile and not TemplateSpecFile)
            {
                semanticModel = null;
                failureDiagnostic = onInvalidSourceFileType(DiagnosticBuilder.ForPosition(reference.ReferenceSourceSyntax));
                return false;
            }

            failureDiagnostic = null;
            semanticModel = compilation.GetSemanticModel(sourceFile);
            return true;
        }
    }
}
