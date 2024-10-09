// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public interface IFactCache<TType, TFact>
    where TType : notnull
{
    TFact Get(TType value);
}

public interface IFactCache<TFact>
{
    TFact Get();
}

public interface IFactsCache
{
    IFactCache<SyntaxBase, DeprecationMetadata?> DeprecationMetadata { get; }

    IFactCache<SyntaxBase, string?> Description { get; }
}

public class FactsCache : IFactsCache
{
    private readonly SemanticModel model;

    public FactsCache(SemanticModel model)
    {
        this.model = model;
        this.DeprecationMetadata = new FactCache<SyntaxBase, DeprecationMetadata?>(GetDeprecationMetadata);
        this.Description = new FactCache<SyntaxBase, string?>(GetDescription);
    }

    private class FactCache<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]TFact> : IFactCache<TFact>
    {
        private readonly Lazy<TFact> cache;

        public FactCache(Func<TFact> lookupFunc)
        {
            cache = new(lookupFunc, LazyThreadSafetyMode.PublicationOnly);
        }

        public TFact Get()
            => cache.Value;
    }

    private class FactCache<TType, TFact> : IFactCache<TType, TFact>
        where TType : notnull
    {
        public FactCache(Func<TType, TFact> lookupFunc)
        {
            this.lookupFunc = lookupFunc;
        }

        private readonly ConcurrentDictionary<TType, TFact> cache = new();
        private readonly Func<TType, TFact> lookupFunc;

        public TFact Get(TType key)
            => cache.GetOrAdd(key, lookupFunc);
    }

    public IFactCache<SyntaxBase, DeprecationMetadata?> DeprecationMetadata { get; }

    public IFactCache<SyntaxBase, string?> Description { get; }

    private DeprecationMetadata? GetDeprecationMetadata(SyntaxBase syntax)
    {
        if (syntax is not DecorableSyntax decorable)
        {
            return null;
        }

        var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(model.Binder, model.GetDeclaredType, decorable, SystemNamespaceType.BuiltInName, LanguageConstants.DeprecatedDecoratorName);
        var reason = TryGetNormalizedStringValue(decorator?.Arguments.FirstOrDefault()?.Expression);

        return decorator is {} ? new(reason) : null;
    }

    private string? GetDescription(SyntaxBase syntax)
    {
        if (syntax is not DecorableSyntax decorable)
        {
            return null;
        }

        var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(model.Binder, model.GetDeclaredType, decorable, SystemNamespaceType.BuiltInName, LanguageConstants.MetadataDescriptionPropertyName);
        return TryGetNormalizedStringValue(decorator?.Arguments.FirstOrDefault()?.Expression);
    }

    private static string? TryGetNormalizedStringValue(SyntaxBase? syntax)
    {
        if (syntax is StringSyntax stringSyntax && stringSyntax.TryGetLiteralValue() is string value)
        {
            return stringSyntax.IsVerbatimString() ? StringUtils.NormalizeIndent(value) : value;
        }

        return null;
    }
}