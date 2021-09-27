// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public static class SymbolValidator
    {
        private delegate IEnumerable<string> GetNameSuggestions();
        private delegate ErrorDiagnostic GetMissingNameError(DiagnosticBuilder.DiagnosticBuilderInternal builder, string? suggestedName);

        public static Symbol ResolveNamespaceQualifiedFunction(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, NamespaceType namespaceType)
            => ResolveSymbolInternal(
                allowedFlags,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () =>
                {
                    var knowFunctionNames = namespaceType.MethodResolver.GetKnownFunctions().Keys;

                    return allowedFlags.HasAnyDecoratorFlag()
                        ? knowFunctionNames.Concat(namespaceType.DecoratorResolver.GetKnownDecoratorFunctions().Keys)
                        : knowFunctionNames;
                },
                getMissingNameError: (builder, suggestedName) => suggestedName switch
                {
                    null => builder.FunctionDoesNotExistInNamespace(namespaceType, identifierSyntax.IdentifierName),
                    _ => builder.FunctionDoesNotExistInNamespaceWithSuggestion(namespaceType, identifierSyntax.IdentifierName, suggestedName),
                });

        public static Symbol ResolveObjectQualifiedFunctionWithoutValidatingFlags(Symbol? foundSymbol, IdentifierSyntax identifierSyntax, ObjectType objectType)
        {
            // The method is not used during binding, so we should not perform validations for FunctionFlags.
            var allowedFlags = foundSymbol is FunctionSymbol functionSymbol ? functionSymbol.FunctionFlags : FunctionFlags.Default;

            if (objectType is NamespaceType namespaceType)
            {
                return ResolveNamespaceQualifiedFunction(allowedFlags, foundSymbol, identifierSyntax, namespaceType);
            }

            return ResolveSymbolInternal(
                allowedFlags,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => objectType.MethodResolver.GetKnownFunctions().Keys,
                getMissingNameError: (builder, suggestedName) => suggestedName switch
                {
                    null => builder.FunctionDoesNotExistOnObject(objectType, identifierSyntax.IdentifierName),
                    _ => builder.FunctionDoesNotExistOnObjectWithSuggestion(objectType, identifierSyntax.IdentifierName, suggestedName),
                });
        }

        public static Symbol ResolveUnqualifiedFunction(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, NamespaceResolver namespaceResolver)
            => ResolveSymbolInternal(
                allowedFlags,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => namespaceResolver.GetKnownFunctionNames(includeDecorators: allowedFlags.HasAnyDecoratorFlag()),
                getMissingNameError: (builder, suggestedName) => suggestedName switch
                {
                    null => builder.SymbolicNameDoesNotExist(identifierSyntax.IdentifierName),
                    _ => builder.SymbolicNameDoesNotExistWithSuggestion(identifierSyntax.IdentifierName, suggestedName),
                });

        public static Symbol ResolveUnqualifiedSymbol(Symbol? foundSymbol, IdentifierSyntax identifierSyntax, NamespaceResolver namespaceResolver, IEnumerable<string> declarations)
            => ResolveSymbolInternal(
                FunctionFlags.Default,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => namespaceResolver.GetKnownPropertyNames(),
                getMissingNameError: (builder, suggestedName) => suggestedName switch
                {
                    null => builder.SymbolicNameDoesNotExist(identifierSyntax.IdentifierName),
                    _ => builder.SymbolicNameDoesNotExistWithSuggestion(identifierSyntax.IdentifierName, suggestedName),
                });

        private static Symbol ResolveSymbolInternal(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, GetNameSuggestions getNameSuggestions, GetMissingNameError getMissingNameError)
        {
            if (foundSymbol is null)
            {
                var nameCandidates = getNameSuggestions().ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
                var suggestedName = SpellChecker.GetSpellingSuggestion(identifierSyntax.IdentifierName, nameCandidates);

                return new ErrorSymbol(getMissingNameError(DiagnosticBuilder.ForPosition(identifierSyntax), suggestedName));
            }

            return foundSymbol switch
            {
                FunctionSymbol functionSymbol => ResolveFunctionFlags(allowedFlags, functionSymbol, identifierSyntax),
                _ => foundSymbol,
            };
        }

        private static Symbol ResolveFunctionFlags(FunctionFlags allowedFlags, FunctionSymbol functionSymbol, IPositionable span)
        {
            var functionFlags = functionSymbol.Overloads.Select(overload => overload.Flags).Aggregate((x, y) => x | y);

            if (functionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly) && !allowedFlags.HasFlag(FunctionFlags.ParamDefaultsOnly))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).FunctionOnlyValidInParameterDefaults(functionSymbol.Name));
            }

            if (functionFlags.HasFlag(FunctionFlags.RequiresInlining) && !allowedFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).FunctionOnlyValidInResourceBody(functionSymbol.Name));
            }

            if (functionFlags.HasAnyDecoratorFlag() && allowedFlags.HasAllDecoratorFlags())
            {
                return functionSymbol;
            }

            if (!functionFlags.HasAnyDecoratorFlag() && allowedFlags.HasAnyDecoratorFlag())
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsDecorator(functionSymbol.Name));
            }

            if (!functionFlags.HasFlag(FunctionFlags.ParameterDecorator) && allowedFlags.HasFlag(FunctionFlags.ParameterDecorator))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsParameterDecorator(functionSymbol.Name));
            }

            if (!functionFlags.HasFlag(FunctionFlags.VariableDecorator) && allowedFlags.HasFlag(FunctionFlags.VariableDecorator))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsVariableDecorator(functionSymbol.Name));
            }

            if (!functionFlags.HasFlag(FunctionFlags.ResourceDecorator) && allowedFlags.HasFlag(FunctionFlags.ResourceDecorator))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsResourceDecorator(functionSymbol.Name));
            }

            if (!functionFlags.HasFlag(FunctionFlags.ModuleDecorator) && allowedFlags.HasFlag(FunctionFlags.ModuleDecorator))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsModuleDecorator(functionSymbol.Name));
            }

            if (!functionFlags.HasFlag(FunctionFlags.OutputDecorator) && allowedFlags.HasFlag(FunctionFlags.OutputDecorator))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).CannotUseFunctionAsOutputDecorator(functionSymbol.Name));
            }

            return functionSymbol;
        }
    }
}
