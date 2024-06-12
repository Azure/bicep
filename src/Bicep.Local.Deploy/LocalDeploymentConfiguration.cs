// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Engine;
using Azure.Deployments.Engine.Dependencies;
using Azure.Deployments.Engine.Host.Azure.Interfaces;

namespace Bicep.Local.Deploy;

public class LocalDeploymentConfiguration : IAzureDeploymentConfiguration
{
    public LocalDeploymentConfiguration(
        IAzureDeploymentSettings settings,
        IAzureDeploymentEngineHost host,
        IPreflightEngineHost preflightEngineHost,
        IDeploymentDependency dependency,
        ITemplateExceptionHandler exceptionHandler,
        IDataProviderHolder dataProviders)
    {
        Settings = settings;
        Host = host;
        PreflightEngineHost = preflightEngineHost;
        DependencyProcessor = dependency;
        TemplateExceptionHandler = exceptionHandler;
        DataProviders = dataProviders;
    }

    public IAzureDeploymentSettings Settings { get; }

    public IAzureDeploymentEngineHost Host { get; }

    public IPreflightEngineHost PreflightEngineHost { get; }

    public ExportTemplateResourceEngine? ExportTemplateResourceEngine { get; }

    public IDeploymentDependency DependencyProcessor { get; }

    public ITemplateExceptionHandler TemplateExceptionHandler { get; }

    public IDataProviderHolder DataProviders { get; }
}
