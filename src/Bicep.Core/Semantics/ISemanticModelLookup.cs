// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Semantics;

public interface ISemanticModelLookup
{
    ISemanticModel GetSemanticModel(ISourceFile sourceFile);

    static ISemanticModelLookup Excluding(ISemanticModelLookup inner, params ISourceFile[] toExcludeFromLookup) => toExcludeFromLookup.Length > 0
        ? new ExcludingSemanticModelLookupDecorator(inner, [.. toExcludeFromLookup])
        : inner;

    private class ExcludingSemanticModelLookupDecorator : ISemanticModelLookup
    {
        private readonly ISemanticModelLookup decorated;
        private readonly ImmutableHashSet<ISourceFile> excludedSources;

        internal ExcludingSemanticModelLookupDecorator(ISemanticModelLookup decorated, ImmutableHashSet<ISourceFile> excludedSources)
        {
            this.decorated = decorated;
            this.excludedSources = excludedSources;
        }

        public ISemanticModel GetSemanticModel(ISourceFile sourceFile)
        {
            if (excludedSources.Contains(sourceFile))
            {
                throw new InvalidOperationException(nameof(sourceFile));
            }

            return decorated.GetSemanticModel(sourceFile);
        }
    }
}
