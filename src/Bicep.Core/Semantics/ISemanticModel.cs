// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface ISemanticModel
    {
        ResourceScope TargetScope { get; }

        ImmutableArray<TypeProperty> ParameterTypeProperties { get; }

        ImmutableArray<TypeProperty> OutputTypeProperties { get; }

        bool HasErrors();
    }
}
