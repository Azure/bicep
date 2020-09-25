// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        /// <summary>
        /// Converts a valid object syntax node to a property dictionary. May throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToKnownPropertyValueDictionary(this ObjectSyntax syntax) =>
            syntax.Properties.Where(p => p.HasKnownKey()).ToImmutableDictionary(p => p.GetKeyText(), p => p.Value, LanguageConstants.IdentifierComparer);
    }
}