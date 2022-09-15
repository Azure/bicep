// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Features;

public class FeatureProviderManager : IFeatureProviderManager
{
    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider();
}
