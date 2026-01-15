// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests;

public interface IDependencyHelper
{
    public TService Construct<TService>()
        where TService : class;
}

public static class IDependencyHelperExtensions
{
    public static BicepCompiler GetCompiler(this IDependencyHelper helper)
        => helper.Construct<BicepCompiler>();

    public static BicepDecompiler GetDecompiler(this IDependencyHelper helper)
        => helper.Construct<BicepDecompiler>();
}

public class ServiceBuilder
{
    private readonly IServiceCollection services;

    private FeatureProviderOverrides? FeatureOverrides { get; set; }

    public ServiceBuilder()
    {
        this.services = new ServiceCollection()
            .AddSingleton<IEnvironment>(TestEnvironment.Default)
            .AddBicepCore()
            .AddBicepDecompiler()
            .AddMockHttpClient(PublicModuleIndexHttpClientMocks.Create([]).Object)
            .WithWorkspace(new ActiveSourceFileSet());
    }

    public static IDependencyHelper Create(Action<IServiceCollection>? registerAction = null)
    {
        registerAction ??= services => { };

        return new ServiceBuilder().WithRegistration(registerAction).Build();
    }

    public static ServiceBuilder CreateWithServices(Action<IServiceCollection> registerAction)
    {
        ServiceBuilder builder = new();

        builder.WithRegistration(registerAction);

        return builder;
    }

    public static ServiceBuilder CreateWithFeatures(FeatureProviderOverrides overrides)
        => CreateWithServices(x => x.WithFeatureOverrides(overrides));

    public ServiceBuilder WithRegistration(Action<IServiceCollection> registerAction)
    {
        registerAction(services);

        return this;
    }

    public ServiceBuilder WithFeatureOverrides(FeatureProviderOverrides overrides)
    {
        var resultFeatures = FeatureOverrides = overrides;

        return WithRegistration(x => x.WithFeatureOverrides(resultFeatures));
    }

    public ServiceBuilder WithFeaturesOverridden(Func<FeatureProviderOverrides, FeatureProviderOverrides> overrides)
    {
        var resultFeatures = FeatureOverrides = overrides(FeatureOverrides ?? new FeatureProviderOverrides());

        return WithRegistration(x => x.WithFeatureOverrides(resultFeatures));
    }

    public IDependencyHelper Build()
    {
        return new ServiceBuilderInternal(services.BuildServiceProvider());
    }

    private class ServiceBuilderInternal : IDependencyHelper
    {
        private readonly IServiceProvider provider;

        public ServiceBuilderInternal(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public TService Construct<TService>()
            where TService : class
        {
            return provider.GetRequiredService<TService>();
        }
    }
}
