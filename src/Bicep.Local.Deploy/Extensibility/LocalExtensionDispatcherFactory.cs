// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Engine.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Engine.Instrumentation;
using Azure.Deployments.Engine.Workers;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Local.Deploy.Extensibility;

public class LocalExtensionDispatcherFactory(
    IConfigurationManager configurationManager,
    ILocalExtensionFactory localExtensionFactory,
    IArmDeploymentProvider armDeploymentProvider)
{
    public LocalExtensionDispatcher Create()
        => new(
            configurationManager,
            localExtensionFactory,
            armDeploymentProvider);
}
