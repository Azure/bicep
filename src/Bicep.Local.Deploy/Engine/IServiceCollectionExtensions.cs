// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Azure.Deployments.Core.Components;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Core.FeatureEnablement;
using Azure.Deployments.Core.Interfaces;
using Azure.Deployments.Core.Telemetry;
using Azure.Deployments.Engine;
using Azure.Deployments.Engine.Definitions;
using Azure.Deployments.Engine.Dependencies;
using Azure.Deployments.Engine.DeploymentExpander;
using Azure.Deployments.Engine.External;
using Azure.Deployments.Engine.Http;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Engine.Providers;
using Azure.Deployments.Engine.Storage.Volatile;
using Azure.Deployments.Engine.Validation;
using Azure.Deployments.Engine.Workers;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Templates.Contracts;
using Azure.Deployments.Templates.Export;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs.Configuration;
using Microsoft.WindowsAzure.ResourceStack.Common.EventSources;
using Microsoft.WindowsAzure.ResourceStack.Common.Storage.Volatile;

namespace Bicep.Local.Deploy.Engine;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterLocalDeployServices(this IServiceCollection services, LocalExtensionDispatcher extensionHostManager)
    {
        var eventSource = new TraceEventSource();
        services.AddSingleton<IGeneralEventSource>(eventSource);
        services.AddSingleton<IDeploymentEventSource>(eventSource);
        services.AddSingleton<IDeploymentMetricsReporter, NoOpDeploymentMetricsReporter>();

        services.AddSingleton<IHttpResponseReader, DefaultHttpResponseReader>();
        services.AddSingleton<IKeyVaultDataProvider, LocalKeyVaultDataProvider>();
        services.AddSingleton<IAzureDeploymentSettings, LocalDeploymentSettings>();
        services.AddSingleton<IEnablementConfigProvider, LocalEnablementConfigProvider>();
        services.AddSingleton<IAzureDeploymentEngineHost, LocalDeploymentEngineHost>();
        services.AddSingleton<IPreflightEngineHost, PreflightEngineHost>();
        services.AddSingleton<IDependencyProcessor, DependencyProcessor>();
        services.AddSingleton<ITemplateExceptionHandler, TemplateExceptionHandler>();

        services.AddSingleton<AzureDeploymentValidation>();
        services.AddSingleton<IExtensionConfigSchemaDirectoryFactory, FactBasedExtensionConfigSchemaDirectoryFactory>();
        services.AddSingleton<IAzureDeploymentConfiguration, LocalDeploymentConfiguration>();
        services.AddSingleton<AzureDeploymentEngine>();
        services.AddSingleton<IDeploymentEntityFactory, VolatileDeploymentEntityFactory>();
        services.AddSingleton<IDeploymentJobsDataProvider, VolatileDeploymentJobDataProvider>();
        services.AddSingleton<IDataProviderHolder, VolatileDataProviderHolder>();
        services.AddSingleton<IResourceTypeRegistrationProvider, ResourceTypeRegistrationProvider>();

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
        services.AddSingleton<IJobsConfigurationProvider>(new JobsConfigurationProvider(
            numWorkersPerInstanceCount: null,
            numWorkersPerProcessorCount: 8,
            numWorkersInJobDispatchingService: null,
            numPartitionsInJobTriggersQueue: 4,
            workerPulsationsThrottlingEnabledForPollRequests: false,
            workerPulsationsThrottlingEnabledForPulseRequests: false));
        services.AddSingleton<WorkerJobDispatcherClient>();

        services.AddSingleton<IDeploymentsRequestContext, LocalRequestContext>();
        services.AddSingleton<LocalRequestContext>();
        services.AddSingleton(extensionHostManager);

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
