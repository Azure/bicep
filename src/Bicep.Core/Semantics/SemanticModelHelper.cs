// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

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

        public static Result<ISemanticModel, ErrorDiagnostic> TryGetModelForArtifactReference(IArtifactFileLookup sourceFileLookup,
            IArtifactReferenceSyntax reference,
            ISemanticModelLookup semanticModelLookup)
        {
            return TryGetSourceFile(sourceFileLookup, reference).Transform(semanticModelLookup.GetSemanticModel);
        }

        public static Result<ISemanticModel, ErrorDiagnostic> TryGetTemplateModelForArtifactReference(IArtifactFileLookup sourceFileLookup,
            IArtifactReferenceSyntax reference,
            DiagnosticBuilder.ErrorBuilderDelegate onInvalidSourceFileType,
            ISemanticModelLookup semanticModelLookup)
        {
            if (!TryGetSourceFile(sourceFileLookup, reference).IsSuccess(out var sourceFile, out var error))
            {
                return new(error);
            }

            // when we inevitably add a third language ID,
            // the inclusion list style below will prevent the new language ID from being
            // automatically allowed to be referenced via module declarations
            if (sourceFile is not (BicepFile or ArmTemplateFile or TemplateSpecFile))
            {
                return new(onInvalidSourceFileType(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
            }

            return new(semanticModelLookup.GetSemanticModel(sourceFile));
        }

        private static Result<ISourceFile, ErrorDiagnostic> TryGetSourceFile(IArtifactFileLookup sourceFileLookup, IArtifactReferenceSyntax reference)
        {
            if (!sourceFileLookup.TryGetSourceFile(reference).IsSuccess(out var sourceFile, out var errorBuilder))
            {
                return new(errorBuilder(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
            }

            return new(sourceFile);
        }
    }
}
