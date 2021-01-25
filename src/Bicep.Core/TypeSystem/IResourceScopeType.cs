// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceScopeType
    {
        ResourceScope ResourceScopeType { get; }
    }
}
