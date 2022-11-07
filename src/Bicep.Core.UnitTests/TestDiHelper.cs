// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests;

public class TestDiHelper
{
    private readonly IServiceProvider serviceProvider;

    private TestDiHelper(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public static TestDiHelper Create(Action<IServiceCollection>? registerAction = null)
    {
        var services = new ServiceCollection();
        registerAction?.Invoke(services);

        return Create(services);
    }

    public static TestDiHelper Create(IServiceCollection services)
    {
        services.AddBicepCore();
        services.AddBicepDecompiler();

        return new(services.BuildServiceProvider());
    }

    public TService Construct<TService>()
        where TService : class
    {
        return serviceProvider.GetRequiredService<TService>();
    }
}
