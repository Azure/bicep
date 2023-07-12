// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Handlers;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentHelper
    {
        Task<BicepDeploymentStartResponse> StartDeploymentAsync(IDeploymentCollectionProvider deploymentCollectionProvider, ArmClient armClient, string documentPath, string template, string id, string scope, string location, string deploymentId, string portalUrl, string deploymentName, JsonElement parametersFileContents, IDeploymentOperationsCache deploymentOperationsCache);
    }
}
