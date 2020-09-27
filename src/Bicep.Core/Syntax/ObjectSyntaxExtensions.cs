// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        /// <summary>
        /// Converts a valid object syntax node to a property dictionary. May throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToKnownPropertyValueDictionary(this ObjectSyntax syntax) =>
            syntax.Properties.ToImmutableDictionaryExcludingNull(p => p.TryGetKeyText(), p => p.Value, LanguageConstants.IdentifierComparer);

        public static ImmutableDictionary<string, ObjectPropertySyntax> ToNamedPropertyDictionary(this ObjectSyntax syntax) =>	
            syntax.Properties.ToImmutableDictionaryExcludingNull(p => p.TryGetKeyText(), LanguageConstants.IdentifierComparer);
    }
}