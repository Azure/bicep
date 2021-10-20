// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Configuration;
using System.Threading;

namespace Bicep.LanguageServer.Providers
{
    public interface IAzResourceProvider
    {
        Task<JsonElement> GetGenericResource(RootConfiguration configuration, ResourceId resourceId, string apiVersion, CancellationToken cancellationToken);
    }
}
