// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Core.Interfaces;
using Azure.Deployments.Engine;
using Azure.Deployments.Engine.Dependencies;
using Azure.Deployments.Engine.Interfaces;

namespace Bicep.Local.Deploy.Engine;

public class LocalDeploymentConfiguration : IAzureDeploymentConfiguration
{
    public LocalDeploymentConfiguration(
        IAzureDeploymentSettings settings,
        IAzureDeploymentEngineHost host,
        IPreflightEngineHost preflightEngineHost,
        IDependencyProcessor dependency,
        ITemplateExceptionHandler exceptionHandler,
        IDataProviderHolder dataProviders,
        IResourceTypeRegistrationProvider resourceTypeRegistrationProvider)
    {
        Settings = settings;
        Host = host;
        PreflightEngineHost = preflightEngineHost;
        DependencyProcessor = dependency;
        TemplateExceptionHandler = exceptionHandler;
        DataProviders = dataProviders;
        ResourceTypeRegistrationProvider = resourceTypeRegistrationProvider;
    }

    public IAzureDeploymentSettings Settings { get; }

    public IAzureDeploymentEngineHost Host { get; }

    public IPreflightEngineHost PreflightEngineHost { get; }

    public ExportTemplateResourceEngine? ExportTemplateResourceEngine { get; }

    public IDependencyProcessor DependencyProcessor { get; }

    public ITemplateExceptionHandler TemplateExceptionHandler { get; }

    public IDataProviderHolder DataProviders { get; }

    public IResourceTypeRegistrationProvider ResourceTypeRegistrationProvider { get; }
}
