// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;
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

    public ServiceBuilder()
    {
        this.services = new ServiceCollection()
            .AddBicepCore()
            .AddBicepDecompiler()
            .WithWorkspace(new Workspace());
    }

    public static IDependencyHelper Create(Action<IServiceCollection>? registerAction = null)
    {
        registerAction ??= services => { };

        return new ServiceBuilder().WithRegistration(registerAction).Build();
    }

    public ServiceBuilder WithRegistration(Action<IServiceCollection> registerAction)
    {
        registerAction(services);

        return this;
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
