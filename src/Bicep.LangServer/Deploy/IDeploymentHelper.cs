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
        /// <summary>
        /// Starts a deployment at provided target scope and returns <see cref="BicepDeploymentStartResponse"/>.
        /// </summary>
        /// <param name="deploymentCollectionProvider">deployment collection provider</param>
        /// <param name="armClient">arm client</param>
        /// <param name="documentPath">path to bicep file used in deployment</param>
        /// <param name="template">template used in deployment</param>
        /// <param name="id">id string to create the ResourceIdentifier from</param>
        /// <param name="scope">target scope</param>
        /// <param name="location">location to store the deployment data</param>
        /// <param name="deploymentId">deployment id</param>
        /// <param name="portalUrl">azure management portal URL</param>
        /// <param name="deploymentName">deployment name</param>
        /// <param name="parametersFileContents">contents of parameter file used in deployment</param>
        /// <param name="deploymentOperationsCache">deployment operations cache that needs to be updated</param>
        /// <returns><see cref="BicepDeploymentStartResponse"/></returns>
        Task<BicepDeploymentStartResponse> StartDeploymentAsync(
            IDeploymentCollectionProvider deploymentCollectionProvider, 
            ArmClient armClient, 
            string documentPath, 
            string template, 
            string id, 
            string scope, 
            string location, 
            string deploymentId, 
            string portalUrl, 
            string deploymentName, 
            JsonElement parametersFileContents, 
            IDeploymentOperationsCache deploymentOperationsCache);
    }
}
