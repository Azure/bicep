// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SymbolValidator
    {
        private delegate IEnumerable<string> GetNameSuggestions();
        private delegate ErrorDiagnostic GetMissingNameError(DiagnosticBuilder.DiagnosticBuilderInternal builder, string? suggestedName);

        public static Symbol ResolveNamespaceQualifiedFunction(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, NamespaceSymbol namespaceSymbol)
            => ResolveSymbolInternal(
                allowedFlags,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => namespaceSymbol.Type.MethodResolver.GetKnownFunctions().Keys,
                getMissingNameError: (builder, suggestedName) => suggestedName switch {
                    null => builder.FunctionDoesNotExistInNamespace(namespaceSymbol, identifierSyntax.IdentifierName),
                    _ => builder.FunctionDoesNotExistInNamespaceWithSuggestion(namespaceSymbol, identifierSyntax.IdentifierName, suggestedName),
                });

        public static Symbol ResolveObjectQualifiedFunction(Symbol? foundSymbol, IdentifierSyntax identifierSyntax, ObjectType objectType)
            => ResolveSymbolInternal(
                FunctionFlags.Default,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => objectType.MethodResolver.GetKnownFunctions().Keys,
                getMissingNameError: (builder, suggestedName) => suggestedName switch {
                    null => builder.FunctionDoesNotExistOnObject(objectType, identifierSyntax.IdentifierName),
                    _ => builder.FunctionDoesNotExistOnObjectWithSuggestion(objectType, identifierSyntax.IdentifierName, suggestedName),
                });

        public static Symbol ResolveUnqualifiedFunction(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, IEnumerable<NamespaceSymbol> namespaces)
            => ResolveSymbolInternal(
                allowedFlags,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => namespaces.SelectMany(x => x.Type.MethodResolver.GetKnownFunctions().Keys),
                getMissingNameError: (builder, suggestedName) => suggestedName switch {
                    null => builder.SymbolicNameDoesNotExist(identifierSyntax.IdentifierName),
                    _ => builder.SymbolicNameDoesNotExistWithSuggestion(identifierSyntax.IdentifierName, suggestedName),
                });

        public static Symbol ResolveUnqualifiedSymbol(Symbol? foundSymbol, IdentifierSyntax identifierSyntax, IEnumerable<NamespaceSymbol> namespaces, IEnumerable<string> declarations)
            => ResolveSymbolInternal(
                FunctionFlags.Default,
                foundSymbol,
                identifierSyntax,
                getNameSuggestions: () => namespaces.SelectMany(x => x.Type.Properties.Keys).Concat(declarations),
                getMissingNameError: (builder, suggestedName) => suggestedName switch {
                    null => builder.SymbolicNameDoesNotExist(identifierSyntax.IdentifierName),
                    _ => builder.SymbolicNameDoesNotExistWithSuggestion(identifierSyntax.IdentifierName, suggestedName),
                });

        private static Symbol ResolveSymbolInternal(FunctionFlags allowedFlags, Symbol? foundSymbol, IdentifierSyntax identifierSyntax, GetNameSuggestions getNameSuggestions, GetMissingNameError getMissingNameError)
        {
            if (foundSymbol == null)
            {
                var nameCandidates = getNameSuggestions().ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
                var suggestedName = SpellChecker.GetSpellingSuggestion(identifierSyntax.IdentifierName, nameCandidates);

                return new ErrorSymbol(getMissingNameError(DiagnosticBuilder.ForPosition(identifierSyntax), suggestedName));
            }

            switch (foundSymbol)
            {
                case FunctionSymbol functionSymbol:
                    return ResolveFunctionFlags(allowedFlags, functionSymbol, identifierSyntax);
                default:
                    return foundSymbol;
            }
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

            return functionSymbol;
        }
    }
}