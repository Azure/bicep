// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Engine;
using Azure.Deployments.Engine.Dependencies;
using Azure.Deployments.Engine.Host.Azure;
using Azure.Deployments.Engine.Host.Azure.Definitions;
using Azure.Deployments.Engine.Host.Azure.DeploymentExpander;
using Azure.Deployments.Engine.Host.Azure.Interfaces;
using Azure.Deployments.Engine.Host.Azure.Providers;
using Azure.Deployments.Engine.Host.Azure.Validation;
using Azure.Deployments.Engine.Host.Azure.Workers;
using Azure.Deployments.Engine.Host.External;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Engine.Storage.Volatile;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Templates.Contracts;
using Azure.Deployments.Templates.Export;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Azure.Deployments.Service.Shared.Jobs;
using Microsoft.Azure.Deployments.Shared.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs;
using Microsoft.WindowsAzure.ResourceStack.Common.EventSources;
using Microsoft.WindowsAzure.ResourceStack.Common.Storage.Volatile;

namespace Bicep.Local.Deploy;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterLocalDeployServices(this IServiceCollection services, LocalExtensibilityHandler extensibilityHandler)
    {
        var eventSource = new TraceEventSource();
        services.AddSingleton<IGeneralEventSource>(eventSource);
        services.AddSingleton<IDeploymentEventSource>(eventSource);

        services.AddSingleton<IKeyVaultDataProvider, LocalKeyVaultDataProvider>();
        services.AddSingleton<IAzureDeploymentSettings, LocalDeploymentSettings>();
        services.AddSingleton<IAzureDeploymentEngineHost, LocalDeploymentEngineHost>();
        services.AddSingleton<IPreflightEngineHost, PreflightEngineHost>();
        services.AddSingleton<IDeploymentDependency, DependencyProcessor>();
        services.AddSingleton<ITemplateExceptionHandler, TemplateExceptionHandler>();

        services.AddSingleton<AzureDeploymentValidation>();
        services.AddSingleton<IAzureDeploymentConfiguration, LocalDeploymentConfiguration>();
        services.AddSingleton<AzureDeploymentEngine>();
        services.AddSingleton<IDeploymentEntityFactory, VolatileDeploymentEntityFactory>();
        services.AddSingleton<IDeploymentJobsDataProvider, VolatileDeploymentJobDataProvider>();
        services.AddSingleton<IDataProviderHolder, VolatileDataProviderHolder>();

        var jobConfiguration = new JobConfigurationBase
        {
            Location = "local",
            EventSource = eventSource,
        };
        services.AddSingleton(jobConfiguration);
        RegisterJobsAsService(services);

        services.AddSingleton<VolatileMemoryStorage>();
        services.AddSingleton<IJobInstanceResolver, JobInstanceResolver>();
        services.AddSingleton<JobCallbackFactory, DeploymentJobCallbackFactory>();
        services.AddSingleton<WorkerJobDispatcherClient>();

        services.AddSingleton<IDeploymentsRequestContext, LocalRequestContext>();
        services.AddSingleton<LocalRequestContext>();
        services.AddSingleton(extensibilityHandler);

        services.AddSingleton<LocalDeploymentEngine>();

        return services;
    }

    private static void RegisterJobsAsService(IServiceCollection services)
    {
        services.AddTransient<DeploymentFirstJob>();
        services.AddTransient<DeploymentExtensibleResourceJob>();
        services.AddTransient<DeploymentLastJob>();
    }
}
