// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Features;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Az
{
    public interface IAzResourceTypeLoaderFactory
    {
        IAzResourceTypeLoader? GetResourceTypeLoader(ImportDeclarationSyntax? ids, IFeatureProvider features);
    }
}