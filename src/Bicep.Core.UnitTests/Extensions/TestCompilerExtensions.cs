// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.UnitTests.Features;
using Bicep.TextFixtures.Utils;

namespace Bicep.Core.UnitTests.Extensions
{
    public static class TestCompilerExtensions
    {
        public static TestCompiler WithFeatureOverrides(this TestCompiler testCompiler, FeatureProviderOverrides overrides) =>
            testCompiler.ConfigureServices(svc =>
            {
                svc.AddSingleton((FeatureProviderFactory)svc.Get<IFeatureProviderFactory>()); // register the impl as a singleton directly.
                svc.AddSingleton(overrides);
                svc.AddSingleton<IFeatureProviderFactory, OverriddenFeatureProviderFactory>();
            });
    }
}
